using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Tests.Console.Configuration;

namespace Tact.Tests.Console.Services.Implementation
{
    [RegisterSingleton(typeof(IThing), "1")]
    public class Thing1 : IThing
    {
        public Thing1(DemoConfig config)
        {
            Number = config.Thing1;
        }

        public int Number { get; }
    }
}