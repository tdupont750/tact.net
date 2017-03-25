using Demo.Rpc.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Serialization;

namespace Demo.Rpc.Services.Implementation
{
    [RegisterSingleton(typeof(IHttpConnection))]
    public class HttpConnection : IHttpConnection, IDisposable
    {
        private readonly ISerializer _serializer;
        private readonly HttpClientConfig _httpClientConfig;
        private readonly HttpClient _httpClient;

        private bool _isDisposed;

        public HttpConnection(ISerializer serializer, HttpClientConfig httpClientConfig)
        {
            _serializer = serializer;
            _httpClientConfig = httpClientConfig;
            _httpClient = new HttpClient();
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _httpClient.Dispose();
            _isDisposed = true;
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, string service, string method)
        {
            var uri = $"{_httpClientConfig.HostUrl}rpc/{service}/{method}";
            var seralizedRequest = _serializer.Serialize(request);

            using (var content = new StringContent(seralizedRequest, _serializer.Encoding, _serializer.MediaType))
            using (var response = await _httpClient.PostAsync(uri, content).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var seralizedResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return _serializer.Deserialize<TResponse>(seralizedResponse);
            }
        }
    }
}
