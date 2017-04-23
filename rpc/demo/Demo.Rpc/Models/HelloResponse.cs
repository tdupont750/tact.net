using System.Collections.Generic;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Models
{
    [RpcModel]
    public class HelloResponse
    {
        public List<string> Messages { get; set; }
    }
}
