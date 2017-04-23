using System;
using Demo.Rpc.Configuration;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tact.Diagnostics;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Clients.Implementation
{
    public class HttpRpcClient : IRpcClient, IDisposable
    {
        private readonly HttpClientConfig _clientConfig;
        private readonly ISerializer _serializer;
        private readonly ILog _log;
        private readonly HttpClient _httpClient;
        
        public HttpRpcClient(ISerializer serializer, ILog log, HttpClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
            _serializer = serializer;
            _log = log;
            _httpClient = new HttpClient();
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(string service, string method, TRequest request, CancellationToken cancelToken)
        {
            try
            {
                // TODO Make this better
                var uri = $"{_clientConfig.Url}rpc/{service}/{method}";
                var seralizedRequest = _serializer.SerializeToString(request);

                using (var content = new StringContent(seralizedRequest, _serializer.Encoding, _serializer.ContentType))
                using (var response = await _httpClient.PostAsync(uri, content, cancelToken).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();

                    var seralizedResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var responseType = typeof(TResponse);
                    return (TResponse)_serializer.Deserialize(responseType, seralizedResponse);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                throw;
            }
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
