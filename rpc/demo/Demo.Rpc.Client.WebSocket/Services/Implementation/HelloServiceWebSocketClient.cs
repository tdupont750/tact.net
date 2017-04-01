using System.Threading.Tasks;
using Demo.Rpc.Models;
using Tact.Practices;
using Tact.Rpc.Services.Base;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Services.Implementation
{
    [RegisterWebSocketClientCondition(ServiceName), RegisterSingleton(typeof(IHelloService))]
    public class HelloServiceWebSocketClient : WebSocketClientBase, IHelloService
    {
        private const string ServiceName = "HelloService";

        public HelloServiceWebSocketClient(IResolver resolver) 
            : base(resolver, ServiceName)
        {
        }

        public Task<HelloResponse> SayHelloAsync(HelloRequest helloRequest)
        {
            return SendAsync<HelloRequest, HelloResponse>(helloRequest, "SayHello");
        }
    }
}
