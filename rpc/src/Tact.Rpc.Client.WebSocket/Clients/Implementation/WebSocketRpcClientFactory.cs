using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Configuration;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Clients.Implementation
{
    [RegisterSingleton(typeof(IRpcClientFactory), WebSocketClientConfig.ProtocolName)]
    public class WebSocketRpcClientFactory : IRpcClientFactory
    {
        public IRpcClient GetRpcClient(ISerializer serializer, IRpcClientConfig config)
        {
            var webSocketConifig = (WebSocketClientConfig)config;
            return new WebSocketRpcClient(serializer, webSocketConifig);
        }
    }
}
