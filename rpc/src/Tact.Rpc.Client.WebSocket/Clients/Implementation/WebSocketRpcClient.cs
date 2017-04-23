using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Tact.Diagnostics;
using Tact.Rpc.Configuration;
using Tact.Rpc.Models;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Clients.Implementation
{
    public class WebSocketRpcClient : IRpcClient, IDisposable
    {
        private readonly WebSocketClientConfig _config;
        private readonly ISerializer _serializer;
        private readonly ILog _log;
        private readonly ClientWebSocket _client;
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentDictionary<string, Tuple<Type, TaskCompletionSource<object>>> _responseMap;
        private readonly CancellationTokenSource _cancelSource;

        private volatile Task _receiveTask;
        private volatile bool _isConnected;

        public WebSocketRpcClient(ISerializer serializer, ILog log, WebSocketClientConfig config)
        {
            _config = config;
            _serializer = serializer;
            _log = log;

            _client = new ClientWebSocket();
            _client.Options.SetRequestHeader("content-type", _serializer.ContentType);

            _semaphore = new SemaphoreSlim(1, 1);
            _responseMap = new ConcurrentDictionary<string, Tuple<Type, TaskCompletionSource<object>>>();
            _cancelSource = new CancellationTokenSource();
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(string service, string method, TRequest request, CancellationToken cancelToken)
        {
            var timeoutSource = new CancellationTokenSource(30000);
            var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, _cancelSource.Token, cancelToken);

            var callInfo = new RemoteCallInfo
            {
                Service = service,
                Method = method,
                Id = Guid.NewGuid().ToString()
            };

            try
            {
                if (!_isConnected)
                using (await _semaphore.UseAsync(linkedSource.Token).ConfigureAwait(false))
                    if (!_isConnected)
                    {
                        var uri = new Uri(_config.Url);
                        await _client
                            .ConnectAsync(uri, linkedSource.Token)
                            .ConfigureAwait(false);

                        _receiveTask = ReadLoopAsync();
                        _isConnected = true;
                    }

                var tcs = new TaskCompletionSource<object>();
                _responseMap.TryAdd(callInfo.Id, Tuple.Create(typeof(TResponse), tcs));

                var callInfoBytes = _serializer.SerializeToBytes(callInfo);
                var callInfoSegment = new ArraySegment<byte>(callInfoBytes);
                await _client
                    .SendAsync(callInfoSegment, WebSocketMessageType.Binary, false, linkedSource.Token)
                    .ConfigureAwait(false);

                var requestBytes = _serializer.SerializeToBytes(request);
                var requestSegment = new ArraySegment<byte>(requestBytes);
                await _client
                    .SendAsync(requestSegment, WebSocketMessageType.Binary, true, linkedSource.Token)
                    .ConfigureAwait(false);

                return (TResponse)await tcs.Task.ConfigureAwait(false);
            }
            finally
            {
                linkedSource.Dispose();
                timeoutSource.Dispose();
                _responseMap.TryRemove(callInfo.Id, out Tuple<Type, TaskCompletionSource<object>> _);
            }
        }

        public void Dispose()
        {
            _cancelSource.Cancel();
            _client?.Dispose();
            _receiveTask?.WaitIfNeccessary();
        }

        private async Task ReadLoopAsync()
        {
            var buffer = new byte[1024];
            using (var memoryStream = new MemoryStream())
                while (_client.State == WebSocketState.Open && !_cancelSource.IsCancellationRequested)
                {
                    try
                    {
                        var segment = new ArraySegment<byte>(buffer);

                        var received = await _client
                            .ReceiveAsync(segment, _cancelSource.Token)
                            .ConfigureAwait(false);

                        memoryStream.Write(segment.Array, 0, segment.Count);

                        if (!received.EndOfMessage)
                            continue;

                        memoryStream.Position = 0;

                        var callInfo = await _serializer
                            .DeserializeAsync<RemoteCallInfo>(memoryStream)
                            .ConfigureAwait(false);

                        if (_responseMap.TryGetValue(callInfo.Id,
                            out Tuple<Type, TaskCompletionSource<object>> responseInfo))
                        {
                            var result = await _serializer
                                .DeserializeAsync(responseInfo.Item1, memoryStream)
                                .ConfigureAwait(false);

                            responseInfo.Item2.SetResult(result);
                        }

                        memoryStream.Position = 0;
                    }
                    catch (TaskCanceledException) when (_cancelSource.IsCancellationRequested)
                    {
                        // Ignore
                    }
                    catch (Exception ex)
                    {
                        _log.Error(ex);
                    }
                }
        }
    }
}
