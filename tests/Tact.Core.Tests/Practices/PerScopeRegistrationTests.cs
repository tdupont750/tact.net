using System;
using Tact.Diagnostics.Implementation;
using Tact.Practices.Implementation;
using Xunit;

namespace Tact.Core.Tests.Practices
{
    public class PerScopeRegistrationTests
    {
        [Fact]
        public void RegisterPerScope()
        {
            IOne a, b, c, d, e, f, g, h;

            using (var resolver = new Container(new InMemoryLog()))
            {
                resolver.RegisterPerScope<IOne, One>();

                a = resolver.Resolve<IOne>();
                b = resolver.Resolve<IOne>();

                Assert.Same(a, b);
                Assert.False(a.IsDisposed);

                using (var child1 = resolver.BeginScope())
                {
                    c = child1.Resolve<IOne>();
                    d = child1.Resolve<IOne>();

                    Assert.NotSame(c, a);
                    Assert.Same(c, d);
                    Assert.False(c.IsDisposed);

                    using (var child2 = resolver.BeginScope())
                    {
                        e = child2.Resolve<IOne>();
                        f = child2.Resolve<IOne>();

                        Assert.NotSame(e, a);
                        Assert.NotSame(e, c);
                        Assert.Same(e, f);
                        Assert.False(e.IsDisposed);
                    }

                    using (var child3 = resolver.BeginScope())
                    {
                        g = child3.Resolve<IOne>();
                        h = child3.Resolve<IOne>();

                        Assert.NotSame(g, a);
                        Assert.NotSame(g, c);
                        Assert.NotSame(g, d);
                        Assert.Same(g, h);
                        Assert.False(g.IsDisposed);
                    }

                    Assert.False(a.IsDisposed);
                    Assert.False(c.IsDisposed);
                    Assert.True(e.IsDisposed);
                    Assert.True(g.IsDisposed);
                }
            }

            Assert.True(a.IsDisposed);
            Assert.True(c.IsDisposed);
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