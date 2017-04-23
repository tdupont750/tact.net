using Demo.Rpc.Configuration;
using Tact.Diagnostics;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Clients.Implementation
{
    [RegisterSingleton(typeof(IRpcClientFactory), HttpClientConfig.ProtocolName)]
    public class HttpRpcClientFactory : IRpcClientFactory
    {
        public IRpcClient GetRpcClient(ISerializer serializer, ILog log, IRpcClientConfig config)
        {
            var httpConfig = (HttpClientConfig)config;
            return new HttpRpcClient(serializer, log, httpConfig);
        }
    }
}
