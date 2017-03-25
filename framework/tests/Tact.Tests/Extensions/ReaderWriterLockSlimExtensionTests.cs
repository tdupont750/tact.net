using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tact.Tests.Extensions
{
    public class ReaderWriterLockSlimExtensionTests
    {
        [Fact]
        public void Use()
        {
            using (var lockSlim = new ReaderWriterLockSlim())
            {
                Assert.Equal(0, lockSlim.CurrentReadCount);
                using (lockSlim.UseReadLock())
                {
                    Assert.Equal(1, lockSlim.CurrentReadCount);

                    Task.Run(() =>
                        {
                            using (lockSlim.UseReadLock())
                                Assert.Equal(2, lockSlim.CurrentReadCount);
                        })
                        .Wait();

                    Assert.Equal(1, lockSlim.CurrentReadCount);
                    Task.Run(() => { Assert.Throws<TimeoutException>(() => lockSlim.UseWriteLock(20)); }).Wait();
                    Assert.Equal(1, lockSlim.CurrentReadCount);
                }

                Assert.Equal(0, lockSlim.CurrentReadCount);
                using (lockSlim.UseWriteLock())
                {
                    Assert.Equal(0, lockSlim.CurrentReadCount);
                    Task.Run(() => { Assert.Throws<TimeoutException>(() => lockSlim.UseReadLock(20)); }).Wait();
                    Assert.Equal(0, lockSlim.CurrentReadCount);
                    Task.Run(() => { Assert.Throws<TimeoutException>(() => lockSlim.UseWriteLock(20)); }).Wait();
                    Assert.Equal(0, lockSlim.CurrentReadCount);
                }

                Assert.Equal(0, lockSlim.CurrentReadCount);
            }
        }
    }
}