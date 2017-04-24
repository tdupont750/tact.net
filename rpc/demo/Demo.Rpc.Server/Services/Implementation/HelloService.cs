using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Rpc.Models;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Services.Implementation
{
    [RpcServiceImplementation(typeof(IHelloService))]
    public class HelloService : IHelloService
    {
        public Task<HelloResponseCollection> SayHelloAsync(HelloRequest helloRequest)
        {
            return Task.FromResult(new HelloResponseCollection
            {
                Messages = new List<HelloResponse>
                {
                    new HelloResponse { Message = $"Hello, {helloRequest.Name}!" },
                    new HelloResponse { Message = $"...and goodbye!" }
                }
            });
        }
    }
}
