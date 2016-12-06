using System;
using Tact.Diagnostics.Implementation;
using Tact.Practices.Implementation;
using Xunit;

namespace Tact.Tests.Practices
{
    public class TransientLifetimeManagerTests
    {
        [Fact]
        public void RegisterTransient()
        {
            IOne a, b, c, d;

            using (var resolver = new TactContainer(new InMemoryLog()))
            {
                resolver.RegisterTransient<IOne, One>();

                a = resolver.Resolve<IOne>();
                b = resolver.Resolve<IOne>();

                using (var child = resolver.BeginScope())
                {
                    c = child.Resolve<IOne>();
                    d = child.Resolve<IOne>();
                }
            }

            Assert.NotSame(a, b);
            Assert.NotSame(a, c);
            Assert.NotSame(a, d);

            Assert.False(a.IsDisposed);
            Assert.False(b.IsDisposed);
            Assert.False(c.IsDisposed);
            Assert.False(d.IsDisposed);
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
