using System;
using System.Collections.Generic;
using System.Linq;
using Tact.Diagnostics.Implementation;
using Tact.Practices.Implementation;
using Xunit;

namespace Tact.Tests.Practices
{
    public class ResolutionHandlerTests
    {
        [Fact]
        public void ThrowOnFail()
        {
            using (var resolver = new Container(new InMemoryLog()))
                Assert.Throws<InvalidOperationException>(() => resolver.Resolve<IOne>());
        }

        [Fact]
        public void DoNotThrowOnFail()
        {
            using (var resolver = new Container(new InMemoryLog(), false))
                Assert.Null(resolver.Resolve<IOne>());
        }

        [Fact]
        public void LazyResolve()
        {
            using (var resolver = new Container(new InMemoryLog(), false))
            {
                resolver.RegisterSingleton<IOne, One>();

                var a = resolver.Resolve<Three>();
                var b = resolver.Resolve<Three>();

                Assert.False(a.LazyOne.IsValueCreated);
                Assert.False(b.LazyOne.IsValueCreated);

                Assert.IsType<One>(a.LazyOne.Value);

                Assert.True(a.LazyOne.IsValueCreated);
                Assert.False(b.LazyOne.IsValueCreated);

                Assert.IsType<One>(b.LazyOne.Value);

                Assert.True(a.LazyOne.IsValueCreated);
                Assert.True(b.LazyOne.IsValueCreated);

                Assert.Same(a.LazyOne.Value, b.LazyOne.Value);
            }
        }

        [Fact]
        public void EnumerableResolve()
        {
            using (var resolver = new Container(new InMemoryLog(), false))
            {
                resolver.RegisterSingleton<ITwo, One>("Two");

                var a = resolver.Resolve<Four>();
                Assert.IsType<One>(a.Twos.Single());
            }
        }

        [Fact]
        public void FuncResolve()
        {
            using (var resolver = new Container(new InMemoryLog(), false))
            {
                resolver.RegisterSingleton<ITwo, One>();

                var a = resolver.Resolve<Five>();
                var b = a.Func();
                var c = a.Func();

                Assert.IsType<One>(b);
                Assert.Same(b, c);
            }
        }

        [Fact]
        public void ConstructorRequired()
        {
            using (var resolver = new Container(new InMemoryLog()))
            {
                Assert.Throws<InvalidOperationException>(() => resolver.RegisterSingleton<Six>());
                Assert.Throws<InvalidOperationException>(() => resolver.RegisterPerScope<Six>());
                Assert.Throws<InvalidOperationException>(() => resolver.RegisterTransient<Six>());
            }
        }

        [Fact]
        public void ClassRequired()
        {
            using (var resolver = new Container(new InMemoryLog()))
            {
                Assert.Throws<InvalidOperationException>(() => resolver.RegisterSingleton<IOne>());
                Assert.Throws<InvalidOperationException>(() => resolver.RegisterPerScope<IOne>());
                Assert.Throws<InvalidOperationException>(() => resolver.RegisterTransient<IOne>());
            }
        }

        private interface IOne
        {
            bool IsDisposed { get; }
        }

        private interface ITwo
        {
            bool IsDisposed { get; }
        }

        private class One : IOne, ITwo, IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        private class Three
        {
            public Three(Lazy<IOne> lazyOne)
            {
                LazyOne = lazyOne;
            }

            public Lazy<IOne> LazyOne { get; }
        }

        private class Four
        {
            public Four(IEnumerable<ITwo> twos)
            {
                Twos = twos;
            }

            public IEnumerable<ITwo> Twos { get; }
        }

        private class Five
        {
            public Five(Func<ITwo> func)
            {
                Func = func;
            }

            public Func<ITwo> Func { get; }
        }

        private class Six
        {
            private Six()
            {
            }
        }

        [Fact]
        public void PreventRecursion()
        {
            using (var resolver = new Container(new InMemoryLog()))
            {
                resolver.RegisterTransient<Seven>();
                Assert.Throws<InvalidOperationException>(() => resolver.Resolve<Seven>());
            }

        }

        private class Seven
        {
            // ReSharper disable once UnusedParameter.Local
            public Seven(Seven seven)
            {
            }
        }
    }
}