using Tact.Practices.LifetimeManagers.Attributes;

namespace Tact.AspNetCore.Demo.Services.Implementation
{
    [RegisterSingleton(typeof(IHelloService))]
    public class HelloService : IHelloService
    {
        public string SayHello(string name)
        {
            return $"Hello, {name}!";
        }
    }
}
