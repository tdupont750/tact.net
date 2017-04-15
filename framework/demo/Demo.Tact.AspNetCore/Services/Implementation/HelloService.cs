using Tact.Practices.LifetimeManagers.Attributes;

namespace Demo.Tact.AspNetCore.Services.Implementation
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
