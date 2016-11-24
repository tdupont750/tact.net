using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tact.Tests.Extensions
{
    public class EnumerableExtensionTests
    {
        [Fact]
        public async Task WhenAll()
        {
            var @lock = new object();
            var expectedCount = Environment.ProcessorCount;
            var currentCount = 0;
            var maxCount = 0;

            var task = Enumerable.Range(0, expectedCount * 2).WhenAll(async i =>
            {
                lock (@lock)
                {
                    currentCount++;
                    maxCount = Math.Max(currentCount, maxCount);
                }

                await Task.Delay(50).ConfigureAwait(false);

                lock (@lock)
                    currentCount--;

            });

            Assert.Equal(currentCount, expectedCount);

            await task.ConfigureAwait(false);

            Assert.Equal(0, currentCount);
            Assert.Equal(maxCount, expectedCount);
        }

        public async Task BailAfterFirstException()
        {
            const int maxCount = 100;
            var count = 0;

             var ex = await Assert
                .ThrowsAsync<AggregateException>(() => Enumerable
                    .Range(0, maxCount)
                    .WhenAll(async v =>
                    {
                        Interlocked.Increment(ref count);
                        await Task.Delay(15 + (v % 15)).ConfigureAwait(false);

                        if (v == 50)
                            throw new InvalidOperationException();
                    }))
                .ConfigureAwait(false);

            Assert.True(count < maxCount);
            Assert.IsType<InvalidOperationException>(ex.InnerException);
        }
    }
}