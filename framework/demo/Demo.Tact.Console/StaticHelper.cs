using System;
using Demo.Tact.Console.Configuration;
using Tact;
using Tact.Configuration;
using Tact.Practices;

namespace Demo.Tact.Console
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