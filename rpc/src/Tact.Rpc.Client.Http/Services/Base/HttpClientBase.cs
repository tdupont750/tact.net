using Demo.Rpc.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using Tact;
using Tact.Practices;
using Tact.Rpc.Serialization;

namespace Demo.Rpc.Services.Base
{
    public abstract class HttpClientBase
    {
        private readonly string _serviceName;
        private readonly string _hostUrl;
        private readonly ISerializer _serializer;
        private readonly HttpClient _httpClient;

        protected HttpClientBase(IResolver resolver, string serviceName)
        {
            var config = resolver.Resolve<HttpClientConfig>(serviceName);

            _serviceName = serviceName;
            _hostUrl = config.Url;
            _serializer = resolver.Resolve<ISerializer>(config.Serializer);
            _httpClient = new HttpClient();
        }

        protected HttpClientBase(string serviceName, string hostUrl, ISerializer serializer)
        {
            _serviceName = serviceName;
            _hostUrl = hostUrl;
            _serializer = serializer;
            _httpClient = new HttpClient();
        }

        protected async Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, string method)
        {
            var uri = $"{_hostUrl}rpc/{_serviceName}/{method}";
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
