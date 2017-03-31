using System;
using System.Threading.Tasks;
using Tact.Practices;
using Tact.Rpc.Configuration;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Services.Base
{
    public abstract class WebSocketClientBase
    {
        private readonly string _serviceName;
        private readonly string _hostUrl;
        private readonly ISerializer _serializer;

        protected WebSocketClientBase(IResolver resolver, string serviceName)
        {
            var config = resolver.Resolve<WebSocketClientConfig>(serviceName);

            _serviceName = serviceName;
            _hostUrl = config.Url;
            _serializer = resolver.Resolve<ISerializer>(config.Serializer);
        }

        protected Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, string method)
        {
            throw new NotImplementedException();
        }
    }
}
