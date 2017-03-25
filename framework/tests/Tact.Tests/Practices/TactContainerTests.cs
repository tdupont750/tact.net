using System.Linq;
using Tact.Diagnostics.Implementation;
using Tact.Practices.Implementation;
using Xunit;

namespace Tact.Tests.Practices
{
    public class TactContainerTests
    {
        [Fact]
        public void IncludeUnkeyedInResolveAll()
        {
            using (var resolver = new TactContainer(new InMemoryLog(), includeUnkeyedInResolveAll: true))
            {
                resolver.RegisterSingleton<IOne, One>();

                var a = resolver.ResolveAll<IOne>().ToList();
                Assert.Equal(1, a.Count);

                resolver.RegisterSingleton<IOne, One>("Doh");

                var b = resolver.ResolveAll<IOne>().ToList();
                Assert.Equal(2, b.Count);

                resolver.RegisterSingleton<IOne, One>("Ray");

                var c = resolver.ResolveAll<IOne>().ToList();
                Assert.Equal(3, c.Count);
            }
        }

        [Fact]
        public void DoNotIncludeUnkeyedInResolveAll()
        {
            using (var resolver = new TactContainer(new InMemoryLog()))
            {
                resolver.RegisterSingleton<IOne, One>();

                var a = resolver.ResolveAll<IOne>().ToList();
                Assert.Equal(0, a.Count);

                resolver.RegisterSingleton<IOne, One>("Doh");

                var b = resolver.ResolveAll<IOne>().ToList();
                Assert.Equal(1, b.Count);

                resolver.RegisterSingleton<IOne, One>("Ray");

                var c = resolver.ResolveAll<IOne>().ToList();
                Assert.Equal(2, c.Count);
            }
        }

        private interface IOne { }

        private class One : IOne { }
    }
}
