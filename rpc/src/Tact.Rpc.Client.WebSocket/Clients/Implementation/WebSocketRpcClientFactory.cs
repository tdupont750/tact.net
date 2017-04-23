using Tact.Diagnostics;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Configuration;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Clients.Implementation
{
    [RegisterSingleton(typeof(IRpcClientFactory), WebSocketClientConfig.ProtocolName)]
    public class WebSocketRpcClientFactory : IRpcClientFactory
    {
        public IRpcClient GetRpcClient(ISerializer serializer, ILog log, IRpcClientConfig config)
        {
            var webSocketConifig = (WebSocketClientConfig)config;
            return new WebSocketRpcClient(serializer, log, webSocketConifig);
        }
    }
}
