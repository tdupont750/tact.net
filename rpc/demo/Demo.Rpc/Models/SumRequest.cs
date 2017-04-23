using Tact.Rpc.Practices;

namespace Demo.Rpc.Models
{
    [RpcModel]
    public class SumRequest
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
