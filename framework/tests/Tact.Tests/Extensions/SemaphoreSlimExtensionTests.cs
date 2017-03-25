using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tact.Tests.Extensions
{
    public class SemaphoreSlimExtensionTests
    {
        [Fact]
        public async Task UseAsync()
        {
            using (var semaphore = new SemaphoreSlim(2))
            {
                Assert.Equal(2, semaphore.CurrentCount);
                using (await semaphore.UseAsync().ConfigureAwait(false))
                {
                    Assert.Equal(1, semaphore.CurrentCount);
                    using (await semaphore.UseAsync().ConfigureAwait(false))
                    {
                        Assert.Equal(0, semaphore.CurrentCount);

                        using (var cts = new CancellationTokenSource())
                        {
                            cts.Cancel();
                            await Assert.ThrowsAsync<TaskCanceledException>(() => semaphore.UseAsync(cts.Token)).ConfigureAwait(false);
                            Assert.Equal(0, semaphore.CurrentCount);
                        }
                        
                        // ReSharper disable once MethodSupportsCancellation
                        await Assert.ThrowsAsync<TimeoutException>(() => semaphore.UseAsync(20)).ConfigureAwait(false);
                        Assert.Equal(0, semaphore.CurrentCount);
                    }

                    Assert.Equal(1, semaphore.CurrentCount);
                }

                Assert.Equal(2, semaphore.CurrentCount);
            }
        }
    }
}
