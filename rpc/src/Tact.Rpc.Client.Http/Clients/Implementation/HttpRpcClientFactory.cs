using Demo.Rpc.Configuration;
using System;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Configuration;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Clients.Implementation
{
    [RegisterSingleton(typeof(IRpcClientFactory), HttpClientConfig.ProtocolName)]
    public class HttpRpcClientFactory : IRpcClientFactory
    {
        public IRpcClient GetRpcClient(ISerializer serializer, IRpcClientConfig config)
        {
            var httpConfig = (HttpClientConfig)config;
            return new HttpRpcClient(serializer, httpConfig);
        }
    }
}
