using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tact.Core.Tests.Extensions
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
    }
}