using System;
using Tact.Diagnostics.Implementation;
using Tact.Practices.Implementation;
using Xunit;

namespace Tact.Tests.Practices
{
    public class SingletonLifetimeManagerTests
    {
        [Fact]
        public void RegisterSingletonInstance()
        {
            IOne a, b, c, d;

            using (var resolver = new Container(new InMemoryLog()))
            {
                a = new One();
                resolver.RegisterInstance(a);

                b = resolver.Resolve<IOne>();

                using (var child = resolver.BeginScope())
                {
                    c = child.Resolve<IOne>();
                    d = child.Resolve<IOne>();
                }

                Assert.Same(a, b);
                Assert.Same(a, c);
                Assert.Same(a, d);

                Assert.False(a.IsDisposed);
            }

            Assert.True(a.IsDisposed);
        }

        [Fact]
        public void RegisterSingleton()
        {
            IOne a, b, c, d;

            using (var resolver = new Container(new InMemoryLog()))
            {
                resolver.RegisterSingleton<IOne, One>();

                a = resolver.Resolve<IOne>();
                b = resolver.Resolve<IOne>();

                using (var child = resolver.BeginScope())
                {
                    c = child.Resolve<IOne>();
                    d = child.Resolve<IOne>();
                }

                Assert.Same(a, b);
                Assert.Same(a, c);
                Assert.Same(a, d);

                Assert.False(a.IsDisposed);
            }

            Assert.True(a.IsDisposed);
        }

        private interface IOne
        {
            bool IsDisposed { get; }
        }

        private class One : IOne, IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}