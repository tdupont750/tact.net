using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Tact.Practices;
using Tact.Rpc.Configuration;
using Tact.Rpc.Models;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Services.Base
{
    public abstract class WebSocketClientBase
    {
        private readonly string _serviceName;
        private readonly string _hostUrl;
        private readonly ISerializer _serializer;
        private readonly ClientWebSocket _client;
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentDictionary<string, Tuple<Type, TaskCompletionSource<object>>> _responseMap;

        private volatile Task _receiveTask;
        private volatile bool _isConnected;

        protected WebSocketClientBase(IResolver resolver, string serviceName)
        {
            var config = resolver.Resolve<WebSocketClientConfig>(serviceName);

            _serviceName = serviceName;
            _hostUrl = config.Url;
            _serializer = resolver.Resolve<ISerializer>(config.Serializer);

            _client = new ClientWebSocket();
            _client.Options.SetRequestHeader("content-type", _serializer.ContentType);

            _semaphore = new SemaphoreSlim(1, 1);
            _responseMap = new ConcurrentDictionary<string, Tuple<Type, TaskCompletionSource<object>>>();
        }

        protected async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, string method)
        {
            if (!_isConnected)
                using (await _semaphore.UseAsync().ConfigureAwait(false))
                    if (!_isConnected)
                    {
                        var uri = new Uri(_hostUrl);
                        await _client
                            .ConnectAsync(uri, CancellationToken.None)
                            .ConfigureAwait(false);

                        _receiveTask = ReadLoopAsync(CancellationToken.None);
                        _isConnected = true;
                    }

            var callInfo = new RemoteCallInfo
            {
                Service = _serviceName,
                Method = method,
                Id = Guid.NewGuid().ToString()
            };

            var tcs = new TaskCompletionSource<object>();
            _responseMap.TryAdd(callInfo.Id, Tuple.Create(typeof(TResponse), tcs));

            var callInfoBytes = _serializer.SerializeToBytes(callInfo);
            var callInfoSegment = new ArraySegment<byte>(callInfoBytes);
            await _client
                .SendAsync(callInfoSegment, WebSocketMessageType.Binary, false, CancellationToken.None)
                .ConfigureAwait(false);

            var requestBytes = _serializer.SerializeToBytes(request);
            var requestSegment = new ArraySegment<byte>(requestBytes);
            await _client
                .SendAsync(requestSegment, WebSocketMessageType.Binary, true, CancellationToken.None)
                .ConfigureAwait(false);

            return (TResponse)await tcs.Task.ConfigureAwait(false);
        }

        private async Task ReadLoopAsync(CancellationToken cancelToken)
        {
            var buffer = new byte[1024];
            using (var memoryStream = new MemoryStream())
                while (_client.State == WebSocketState.Open)
                {
                    var segment = new ArraySegment<byte>(buffer);

                    var received = await _client
                        .ReceiveAsync(segment, CancellationToken.None)
                        .ConfigureAwait(false);

                    memoryStream.Write(segment.Array, 0, segment.Count);

                    if (!received.EndOfMessage)
                        continue;
                    
                    memoryStream.Position = 0;

                    var callInfo = await _serializer
                        .DeserializeAsync<RemoteCallInfo>(memoryStream)
                        .ConfigureAwait(false);

                    if (_responseMap.TryGetValue(callInfo.Id, out Tuple<Type, TaskCompletionSource<object>> responseInfo))
                    {
                        var result = await _serializer
                            .DeserializeAsync(responseInfo.Item1, memoryStream)
                            .ConfigureAwait(false);

                        responseInfo.Item2.SetResult(result);
                    }

                    memoryStream.Position = 0;
                }
        }
    }
}
