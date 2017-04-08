using Demo.Rpc.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Clients.Implementation
{
    public class HttpRpcClient : IRpcClient
    {
        private readonly HttpClientConfig _clientConfig;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;
        
        public HttpRpcClient(ISerializer serializer, HttpClientConfig clientConfig)
        {
            _clientConfig = clientConfig;
            _serializer = serializer;
            _httpClient = new HttpClient();
        }

        public async Task<TResponse> SendAsync<TRequest, TResponse>(string service, string method, TRequest request)
        {
            // TODO Make this better
            var uri = $"{_clientConfig.Url}rpc/{service}/{method}";
            var seralizedRequest = _serializer.SerializeToString(request);

            using (var content = new StringContent(seralizedRequest, _serializer.Encoding, _serializer.ContentType))
            using (var response = await _httpClient.PostAsync(uri, content).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var seralizedResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var responseType = typeof(TResponse);
                return (TResponse)_serializer.Deserialize(responseType, seralizedResponse);
            }
        }
    }
}
