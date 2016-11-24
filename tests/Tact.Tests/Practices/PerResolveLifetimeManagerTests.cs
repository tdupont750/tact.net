using Tact.Diagnostics.Implementation;
using Tact.Practices.Implementation;
using Xunit;

namespace Tact.Tests.Practices
{
    public class PerResolveLifetimeManagerTests
    {
        [Fact]
        public void RegisterPerResolve()
        {
            using (var resolver = new Container(new InMemoryLog()))
            {
                resolver.RegisterPerResolve<IOne, One>();
                resolver.RegisterPerResolve<ITwo, Two>();
                resolver.RegisterSingleton<IThree, Three>();

                var a = resolver.Resolve<IOne>();
                Assert.Same(a.Two.Three, a.Three);

                var b = resolver.Resolve<IOne>();
                Assert.Same(b.Two.Three, b.Three);

                Assert.NotSame(a, b);
                Assert.NotSame(a.Two, b.Two);

                Assert.Same(a.Three, b.Two.Three);
            }
        }

        private interface IOne
        {
            ITwo Two { get; }
            IThree Three { get; }
        }

        private interface ITwo
        {
            IThree Three { get; }
        }

        private interface IThree
        {
        }

        private class One : IOne
        {
            public ITwo Two { get; }
            public IThree Three { get; }

            public One(ITwo two, IThree three)
            {
                Two = two;
                Three = three;
            }
        }

        private class Two : ITwo
        {
            public IThree Three { get; }

            public Two(IThree three)
            {
                Three = three;
            }
        }

        private class Three : IThree
        {
        }
    }
}