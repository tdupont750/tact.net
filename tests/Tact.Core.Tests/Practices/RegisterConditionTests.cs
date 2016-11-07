using System;
using Tact.Configuration;
using Tact.Configuration.Attributes;
using Tact.Diagnostics.Implementation;
using Tact.Practices;
using Tact.Practices.Implementation;
using Tact.Practices.LifetimeManagers;
using Tact.Practices.LifetimeManagers.Attributes;
using Xunit;

namespace Tact.Core.Tests.Practices
{
    public class RegisterConditionTests
    {
        [Fact]
        public void ShouldRegisterTrue()
        {
            var logger = new InMemoryLog();
            using (var container = new Container(logger))
            {
                var configurationFactory = new TestConfigurationFactory(true);

                var types = new[] { typeof(TestConfig), typeof(Test) };
                container.RegisterConfigurationByAttribute(configurationFactory, types);
                container.RegisterByAttribute(types);

                container.Resolve<ITest>();
            }
        }

        [Fact]
        public void ShouldRegisterFalse()
        {
            var logger = new InMemoryLog();
            using (var container = new Container(logger))
            {
                var configurationFactory = new TestConfigurationFactory(false);

                var types = new[] { typeof(TestConfig), typeof(Test) };
                container.RegisterConfigurationByAttribute(configurationFactory, types);
                container.RegisterByAttribute(types);

                Assert.Throws<InvalidOperationException>(() => container.Resolve<ITest>());
            }
        }

        public class TestConfigurationFactory : IConfigurationFactory
        {
            private readonly bool _shouldRegister;

            public TestConfigurationFactory(bool shouldRegister)
            {
                _shouldRegister = shouldRegister;
            }

            public object CreateObject(Type type)
            {
                return new TestConfig
                {
                    ShouldRegister = _shouldRegister
                };
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
