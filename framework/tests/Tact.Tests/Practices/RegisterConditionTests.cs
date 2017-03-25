using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Tact.Configuration;
using Tact.Configuration.Attributes;
using Tact.Diagnostics.Implementation;
using Tact.Practices;
using Tact.Practices.Implementation;
using Tact.Practices.LifetimeManagers;
using Tact.Practices.LifetimeManagers.Attributes;
using Xunit;

namespace Tact.Tests.Practices
{
    public class RegisterConditionTests
    {
        [Fact]
        public void ShouldRegisterTrue()
        {
            var logger = new InMemoryLog();
            using (var container = new TactContainer(logger))
            {
                var map = new Dictionary<string, string>
                {
                    {"TestConfig:ShouldRegister", "true"}
                };
                var configBuilder = new ConfigurationBuilder();
                configBuilder.AddInMemoryCollection(map);
                var config = configBuilder.Build();

                var types = new[] { typeof(TestConfig), typeof(Test) };
                container.ConfigureByAttribute(config, types);
                container.RegisterByAttribute(types);

                container.Resolve<ITest>();
            }
        }

        [Fact]
        public void ShouldRegisterFalse()
        {
            var logger = new InMemoryLog();
            using (var container = new TactContainer(logger))
            {
                var map = new Dictionary<string, string>
                {
                    {"TestConfig.ShouldRegister", "false"}
                };
                var configBuilder = new ConfigurationBuilder();
                configBuilder.AddInMemoryCollection(map);
                var config = configBuilder.Build();

                var types = new[] { typeof(TestConfig), typeof(Test) };
                container.ConfigureByAttribute(config, types);
                container.RegisterByAttribute(types);

                Assert.Throws<InvalidOperationException>(() => container.Resolve<ITest>());
            }
        }

        [RegisterConfiguration]
        public class TestConfig
        {
            public bool ShouldRegister { get; set; }
        }

        public class TestRegisterConditionAttribute : Attribute, IRegisterConditionAttribute
        {
            public bool ShouldRegister(IContainer container, Type toType)
            {
                var config = container.Resolve<TestConfig>();
                return config.ShouldRegister;
            }
        }

        public interface ITest
        {
        }

        [RegisterSingleton(typeof(ITest))]
        [TestRegisterCondition]
        public class Test : ITest
        {   
        }
    }
}
