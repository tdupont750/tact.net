using System.Collections.Generic;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Models
{
    [RpcModel(Order = 2)]
    public class HelloResponseCollection
    {
        public List<HelloResponse> Messages { get; set; }
    }
}