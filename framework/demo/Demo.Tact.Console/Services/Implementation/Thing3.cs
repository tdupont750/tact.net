using System;
using Demo.Tact.Console.Configuration;

namespace Demo.Tact.Console.Services.Implementation
{
    [RegisterSingleton(typeof(IThing), "3"), RegisterCondition]
    public class Thing3 : IThing
    {
        public int Number { get; } = 3;

        public class RegisterCondition : Attribute, IRegisterConditionAttribute
        {
            public bool ShouldRegister(IContainer container, Type toType)
            {
                var config = container.Resolve<DemoConfig>();
                return config.Thing3 ?? false;
            }
        }
    }
}