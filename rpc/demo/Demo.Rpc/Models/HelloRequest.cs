using Tact.Rpc.Practices;

namespace Demo.Rpc.Models
{
    [RpcModel]
    public class HelloRequest
    {
        public string Name { get; set; }
    }
}
