using System;
using Tact.Configuration;
using Tact.Practices;
using Tact.Tests.Console.Configuration;

namespace Tact.Tests.Console
{
    [Initialize]
    public static class StaticHelper
    {
        public static string StaticString { get; private set; }
        
        public class InitializeAttribute : Attribute, IInitializeAttribute
        {
            public void Initialize(IContainer container)
            {
                var config = container.Resolve<DemoConfig>();
                StaticString = config.SomeString;
            }
        }
    }
}