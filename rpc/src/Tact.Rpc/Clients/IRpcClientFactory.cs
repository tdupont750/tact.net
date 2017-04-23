using Tact.Diagnostics;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Clients
{
    public interface IRpcClientFactory
    {
        IRpcClient GetRpcClient(ISerializer serializer, ILog log, IRpcClientConfig config);
    }
}
