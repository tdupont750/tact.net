using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tact.Threading
{
    public static class Using
    {
        public static async Task<TOutput> Async<TInput, TOutput>(
            TInput disposable,
            Func<TInput, CancellationToken, Task<TOutput>> func,
            CancellationToken cancelToken = default(CancellationToken))
            where TInput : IAsyncDisposable
        {
            try
            {
                return await func(disposable, cancelToken).ConfigureAwait(false);
            }
            finally
            {
                await disposable.DisposeAsync(cancelToken).ConfigureAwait(false);
            }
        }

        public static async Task Async<T>(
            T disposable,
            Func<T, CancellationToken, Task> func,
            CancellationToken cancelToken = default(CancellationToken))
            where T : IAsyncDisposable
        {
            try
            {
                await func(disposable, cancelToken).ConfigureAwait(false);
            }
            finally
            {
                await disposable.DisposeAsync(cancelToken).ConfigureAwait(false);
            }
        }
    }
}