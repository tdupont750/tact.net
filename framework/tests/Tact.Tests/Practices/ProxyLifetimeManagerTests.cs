using Tact.Diagnostics.Implementation;
using Tact.Practices.Implementation;
using Xunit;

namespace Tact.Tests.Practices
{
    public class ProxyLifetimeManagerTests
    {
        [Fact]
        public void SingletonProxy()
        {
            using (var resolver = new TactContainer(new InMemoryLog()))
            {
                resolver.RegisterSingleton<IOne, One>();
                resolver.RegisterProxy<ITwo, IOne>();

                var two = resolver.Resolve<ITwo>();
                var one = resolver.Resolve<IOne>();

                Assert.Same(one, two);
            }
        }

        [Fact]
        public void PerScopeProxy()
        {
            using (var resolver = new TactContainer(new InMemoryLog()))
            {
                resolver.RegisterPerScope<IOne, One>();
                resolver.RegisterProxy<ITwo, IOne>();

                var twoA = resolver.Resolve<ITwo>();
                var oneA = resolver.Resolve<IOne>();

                Assert.Same(oneA, twoA);

                using (var scope = resolver.BeginScope())
                {
                    var twoB = scope.Resolve<ITwo>();
                    var oneB = scope.Resolve<IOne>();

                    Assert.Same(oneB, twoB);
                    Assert.NotSame(oneA, oneB);
                }
            }
        }

        [Fact]
        public void KeyProxy()
        {
            using (var resolver = new TactContainer(new InMemoryLog()))
            {
                resolver.RegisterSingleton<IOne, One>("Hello");
                resolver.RegisterProxy<ITwo, IOne>("Hello", "World");

                var two = resolver.Resolve<ITwo>("World");
                var one = resolver.Resolve<IOne>("Hello");

                Assert.Same(one, two);
            }
        }

        private interface ITwo
        {
        }

        private interface IOne
        {
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private class One : IOne, ITwo
        {
        }
    }
}
