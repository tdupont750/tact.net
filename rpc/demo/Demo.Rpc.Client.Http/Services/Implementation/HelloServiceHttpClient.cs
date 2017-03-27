using System.Threading.Tasks;
using Demo.Rpc.Models;
using Tact.Practices.LifetimeManagers.Attributes;
using Demo.Rpc.Services.Base;
using Tact.Practices;
using Tact.Rpc.Serialization;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Services.Implementation
{
    [RegisterHttpClientCondition(ServiceName), RegisterSingleton(typeof(IHelloService))]
    public class HelloServiceHttpClient : HttpClientBase, IHelloService
    {
        private const string ServiceName = "HelloService";

        public HelloServiceHttpClient(IResolver resolver)
            : base(resolver, ServiceName)
        {
        }

        private HelloServiceHttpClient(string hostUrl, ISerializer serializer)
            : base(ServiceName, hostUrl, serializer)
        {
        }

        public HelloServiceHttpClient Create(string hostUrl, ISerializer serializer)
        {
            return new HelloServiceHttpClient(hostUrl, serializer);
        }

        public Task<HelloResponse> SayHelloAsync(HelloRequest helloRequest)
        {
            return SendAsync<HelloRequest, HelloResponse>(helloRequest, "SayHello");
        }
    }
}
