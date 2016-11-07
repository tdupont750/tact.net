using System;
using Tact.Diagnostics.Implementation;
using Tact.Practices.Implementation;
using Tact.Practices.LifetimeManagers.Attributes;
using Xunit;

namespace Tact.Core.Tests.Practices
{
    public class RegisterByAttributesTests
    {
        [Fact]
        public void RegisterByAttribute()
        {
            var log = new InMemoryLog();
            using (var container = new Container(log))
            {
                Assert.Throws<InvalidOperationException>(() => container.Resolve<ITester>());

                container.RegisterByAttribute(typeof(Tester));
                var tester = container.Resolve<ITester>();
                Assert.IsType<Tester>(tester);
            }
        }

        public interface ITester
        {
        }

        [RegisterSingleton(typeof(ITester))]
        public class Tester : ITester
        {
        }
    }
}
