using System;
using Demo.Tact.Console.Configuration;

namespace Demo.Tact.Console.Services.Implementation
{
    [RegisterSingleton(typeof(IThing), "2"), RegisterCondition]
    public class Thing2 : IThing
    {
        public int Number { get; } = 2;

        public class RegisterCondition : Attribute, IRegisterConditionAttribute
        {
            public bool ShouldRegister(IContainer container, Type toType)
            {
                var config = container.Resolve<DemoConfig>();
                return config.Thing2 ?? false;
            }
        }
    }
}