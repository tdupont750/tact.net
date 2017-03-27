using System.Threading.Tasks;
using Demo.Rpc.Models;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Services.Implementation
{
    [RegisterServiceCondition(nameof(HelloService)), RegisterSingleton(typeof(IHelloService))]
    public class HelloService : IHelloService
    {
        public Task<HelloResponse> SayHelloAsync(HelloRequest helloRequest)
        {
            return Task.FromResult(new HelloResponse
            {
                Message = $"Hello, {helloRequest.Name}!"
            });
        }
    }
}
