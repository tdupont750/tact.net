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
            using (var resolver = new TactContainer(new InMemoryLog()))
                Assert.Throws<InvalidOperationException>(() => resolver.Resolve<IOne>());
        }
        
        [Fact]
        public void LazyResolve()
        {
            using (var resolver = new TactContainer(new InMemoryLog()))
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
            using (var resolver = new TactContainer(new InMemoryLog()))
            {
                resolver.RegisterSingleton<ITwo, One>("Two");

                var a = resolver.Resolve<Four>();
                Assert.IsType<One>(a.Twos.Single());
            }
        }

        [Fact]
        public void FuncResolve()
        {
            using (var resolver = new TactContainer(new InMemoryLog()))
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
        public void PreventRecursion()
        {
            using (var resolver = new TactContainer(new InMemoryLog()))
            {
                resolver.RegisterTransient<Seven>();
                Assert.Throws<InvalidOperationException>(() => resolver.Resolve<Seven>());
            }
        }

        [Fact]
        public void GenericClassResolve()
        {
            using (var resolver = new TactContainer(new InMemoryLog()))
            {
                Assert.Throws<InvalidOperationException>(() => resolver.Resolve<Seven>());

                var eightInt = resolver.Resolve<Eight<int>>();
                Assert.Equal(typeof(int), eightInt.Type);

                var eightBool = resolver.Resolve<Eight<bool>>();
                Assert.Equal(typeof(bool), eightBool.Type);
            }
        }

        [Fact]
        public void GenericInterfaceResolve()
        {
            using (var resolver = new TactContainer(new InMemoryLog()))
            {
                resolver.RegisterTransient(typeof(IEight<>), typeof(Eight<>));

                Assert.Throws<InvalidOperationException>(() => resolver.Resolve<Seven>());

                var eightInt = resolver.Resolve<IEight<int>>();
                Assert.Equal(typeof(int), eightInt.Type);

                var eightBool = resolver.Resolve<IEight<bool>>();
                Assert.Equal(typeof(bool), eightBool.Type);
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

        private class Seven
        {
            // ReSharper disable once UnusedParameter.Local
            public Seven(Seven seven)
            {
            }
        }

        private interface IEight<T>
        {
            Type Type { get; }
        }

        private class Eight<T> : IEight<T>
        {
            public Type Type => typeof(T);
        }
    }
}