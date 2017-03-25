using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tact
{
    public static class SemaphoreSlimExtensions
    {
        public static Task<IDisposable> UseAsync(this SemaphoreSlim semaphore, int millisecondsTimeout, CancellationToken cancelToken = default(CancellationToken))
        {
            return semaphore.UseAsync(TimeSpan.FromMilliseconds(millisecondsTimeout), cancelToken);
        }

        public static async Task<IDisposable> UseAsync(this SemaphoreSlim semaphore, TimeSpan timeout, CancellationToken cancelToken = default(CancellationToken))
        {
            if (semaphore == null)
                throw new ArgumentNullException(nameof(semaphore));

            var result = await semaphore.WaitAsync(timeout, cancelToken).ConfigureAwait(false);
            if (!result)
                throw new TimeoutException("Unable to obtain a lock");

            return new SemaphoreSlimWrapper(semaphore);
        }

        public static async Task<IDisposable> UseAsync(this SemaphoreSlim semaphore, CancellationToken cancelToken = default(CancellationToken))
        {
            if (semaphore == null)
                throw new ArgumentNullException(nameof(semaphore));

            await semaphore.WaitAsync(cancelToken).ConfigureAwait(false);

            return new SemaphoreSlimWrapper(semaphore);
        }

        private struct SemaphoreSlimWrapper : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;

            public SemaphoreSlimWrapper(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
            }

            public void Dispose()
            {
                _semaphore.Release();
            }
        }
    }
}
