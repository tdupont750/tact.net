using Demo.Rpc.Models;
using Demo.Rpc.Services;
using System;
using System.Threading.Tasks;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Hosts;
using Tact.Rpc.Models;

namespace Demo.Rpc.Hosts.Implementation
{
    [RegisterSingleton(typeof(IWebSocketEndpoint), ServiceName)]
    public class HelloServiceEndpoint : IWebSocketEndpoint
    {
        private const string ServiceName = "HelloService";

        private readonly IHelloService _helloService;

        public HelloServiceEndpoint(IHelloService helloService)
        {
            _helloService = helloService;
        }

        public bool CanHandle(RemoteCallInfo callInfo, out Type modelType)
        {
            if (!ServiceName.Equals(callInfo.Service, StringComparison.OrdinalIgnoreCase))
            {
                modelType = null;
                return false;
            }

            switch (callInfo.Method.ToLowerInvariant())
            {
                case "sayhello":
                    modelType = typeof(HelloRequest);
                    return true;

                default:
                    modelType = null;
                    return false;
            }
        }

        public async Task<object> HandleAsync(RemoteCallInfo callInfo, object model)
        {
            switch (callInfo.Method.ToLowerInvariant())
            {
                case "sayhello":
                    return await _helloService
                        .SayHelloAsync((HelloRequest) model)
                        .ConfigureAwait(false);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
