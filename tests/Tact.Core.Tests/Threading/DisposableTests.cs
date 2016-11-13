using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tact.Threading;
using Xunit;

namespace Tact.Core.Tests.Threading
{
    public class DisposableTests
    {
        [Fact]
        public async Task AsyncDisposableTest()
        {
            var asyncDisposable = new AsyncDisposableClass();
            Assert.False(asyncDisposable.IsDisposed);
            var task = Disposable.Async(asyncDisposable);
            Assert.False(asyncDisposable.IsDisposed);
            await task.ConfigureAwait(false);
            Assert.True(asyncDisposable.IsDisposed);
        }

        [Fact]
        public async Task DisposableTest()
        {
            var disposable = new DisposableClass();
            Assert.False(disposable.IsDisposed);
            var task = Disposable.Async(disposable);
            Assert.False(disposable.IsDisposed);
            await task.ConfigureAwait(false);
            Assert.True(disposable.IsDisposed);
        }

        [Fact]
        public async Task NonDisposableTest()
        {
            var nonDisposable = new NonDisposableClass();
            Assert.False(nonDisposable.IsDisposed);
            var task = Disposable.Async(nonDisposable);
            Assert.False(nonDisposable.IsDisposed);
            await task.ConfigureAwait(false);
            Assert.False(nonDisposable.IsDisposed);
        }

        public class AsyncDisposableClass : IAsyncDisposable
        {
            public bool IsDisposed { get; private set; }

            public async Task DisposeAsync(CancellationToken cancelToken)
            {
                await Task.Delay(20, cancelToken).ConfigureAwait(false);
                IsDisposed = true;
            }

            public void Dispose()
            {
                DisposeAsync(CancellationToken.None).Wait();
            }
        }

        public class DisposableClass : IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                Task.Delay(50).Wait();
                IsDisposed = true;
            }
        }

        public class NonDisposableClass
        {
            public bool IsDisposed { get; set; }
        }
    }
}
