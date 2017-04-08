using Tact.Rpc.Configuration;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Clients
{
    public interface IRpcClientFactory
    {
        IRpcClient GetRpcClient(ISerializer serializer, IRpcClientConfig config);
    }
}
