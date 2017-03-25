using System;
using System.Threading.Tasks;
using Demo.Rpc.Models;
using Tact.Practices.LifetimeManagers.Attributes;
using Demo.Rpc.Configuration;
using Tact.Practices.LifetimeManagers;
using Tact;

namespace Demo.Rpc.Services.Implementation
{
    [RegisterCondition, RegisterSingleton(typeof(IHelloService))]
    public class HelloServiceHttpClient : IHelloService
    {
        private const string ServiceName = "HelloService";

        private readonly IHttpConnection _httpConnection;

        public HelloServiceHttpClient(IHttpConnection httpConnection)
        {
            _httpConnection = httpConnection;
        }

        public Task<HelloResponse> SayHelloAsync(HelloRequest helloRequest)
        {
            return _httpConnection.SendAsync<HelloRequest, HelloResponse>(helloRequest, ServiceName, "SayHello");
        }

        public class RegisterConditionAttribute : Attribute, IRegisterConditionAttribute
        {
            public bool ShouldRegister(Tact.Practices.IContainer container, Type toType)
            {
                return container.TryResolve(out HttpClientConfig config) && config.IsEnabled;
            }
        }
    }
}
