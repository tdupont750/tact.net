using System;
using Tact.Configuration;

namespace Tact.Tests.Console.Configuration
{
    public class ConfigurationFactory : IConfigurationFactory
    {
        public object CreateObject(Type type)
        {
            if (type == typeof(DemoConfig))
                return new DemoConfig
                {
                    IsEnable = true,
                    SomeString = "Hello world!",
                    Thing1 = 1,
                    Thing2 = true,
                    Thing3 = false
                };

            throw new ArgumentException("Only Supports ThingConfig", nameof(type));
        }
    }
}