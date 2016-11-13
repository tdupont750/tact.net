using System;
using System.Threading;
using System.Threading.Tasks;
using Tact.Threading;
using Xunit;

namespace Tact.Core.Tests.Threading
{
    public class UsingTests
    {
        [Fact]
        public async Task UsingTest()
        {
            var i = 0;

            var asyncDisposable = await Using.Async(new AsyncDisposableClass(), async adc =>
            {
                Assert.False(adc.IsDisposed);
                await Task.Delay(20);
                i++;
                return adc;
            });

            Assert.NotNull(asyncDisposable);
            Assert.True(asyncDisposable.IsDisposed);
            Assert.Equal(1, i);
        }

        [Fact]
        public async Task UsingThrows()
        {
            AsyncDisposableClass asyncDisposable = null;
            var i = 0;

            await Assert.ThrowsAsync<InvalidProgramException>(() => Using.Async(new AsyncDisposableClass(), async adc =>
            {
                Assert.False(adc.IsDisposed);
                asyncDisposable = adc;
                await Task.Delay(20);
                i++;
                throw new InvalidProgramException();
            }));

            Assert.NotNull(asyncDisposable);
            Assert.True(asyncDisposable.IsDisposed);
            Assert.Equal(1, i);
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
    }
}