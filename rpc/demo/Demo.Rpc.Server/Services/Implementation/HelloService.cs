using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Rpc.Models;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Services.Implementation
{
    [RpcServiceImplementation(typeof(IHelloService))]
    public class HelloService : IHelloService
    {
        public Task<HelloResponse> SayHelloAsync(HelloRequest helloRequest)
        {
            return Task.FromResult(new HelloResponse
            {
                Messages = new List<string> { $"Hello, {helloRequest.Name}!", "...and goodbye!" }
            });
        }
    }
}
