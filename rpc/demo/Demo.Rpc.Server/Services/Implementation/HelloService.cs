using System.Threading.Tasks;
using Demo.Rpc.Models;
using Tact.Practices.LifetimeManagers.Attributes;
using System;
using Tact.Practices.LifetimeManagers;
using Demo.Rpc.Configuration;
using Tact;

namespace Demo.Rpc.Services.Implementation
{
    [RegisterCondition, RegisterSingleton(typeof(IHelloService))]
    public class HelloService : IHelloService
    {
        public Task<HelloResponse> SayHelloAsync(HelloRequest helloRequest)
        {
            return Task.FromResult(new HelloResponse
            {
                Message = $"Hello, {helloRequest.Name}!"
            });
        }

        public class RegisterConditionAttribute : Attribute, IRegisterConditionAttribute
        {
            public bool ShouldRegister(Tact.Practices.IContainer container, Type toType)
            {
                return container.TryResolve(out DemoServerConfig config) && config.IsEnabled;
            }
        }
    }
}
