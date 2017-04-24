using Tact.Rpc.Practices;

namespace Demo.Rpc.Models
{
    [RpcModel]
    public class HelloResponse
    {
        public string Message { get; set; }
    }
}
