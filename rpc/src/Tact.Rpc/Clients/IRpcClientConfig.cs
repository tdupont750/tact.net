namespace Tact.Rpc.Clients
{
    public interface IRpcClientConfig
    {
        bool IsEnabled { get; }

        string Protocol { get; }

        string Serializer { get; }
    }
}
