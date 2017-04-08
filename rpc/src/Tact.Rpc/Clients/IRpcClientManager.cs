namespace Tact.Rpc.Clients
{
    public interface IRpcClientManager
    {
        IRpcClient GetRpcClient(string service);
    }
}
