using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tact.Threading
{
    public static class Using
    {
        public static Task<TOutput> Async<TInput, TOutput>(
            TInput disposable,
            Func<TInput, Task<TOutput>> func)
            where TInput : IAsyncDisposable
        {
            return Async(disposable, CancellationToken.None, (arg, token) => func(arg));
        }

        public static async Task<TOutput> Async<TInput, TOutput>(
            TInput disposable,
            CancellationToken cancelToken,
            Func<TInput, CancellationToken, Task<TOutput>> func)
            where TInput : IAsyncDisposable
        {
            if (disposable == null)
                throw new ArgumentNullException(nameof(disposable));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            try
            {
                return await func(disposable, cancelToken).ConfigureAwait(false);
            }
            finally
            {
                await disposable.DisposeAsync(cancelToken).ConfigureAwait(false);
            }
        }

        public static Task Async<T>(
            T disposable,
            Func<T, Task> func)
            where T : IAsyncDisposable
        {
            return Async(disposable, CancellationToken.None, (arg, token) => func(arg));
        }

        public static async Task Async<T>(
            T disposable,
            CancellationToken cancelToken,
            Func<T, CancellationToken, Task> func)
            where T : IAsyncDisposable
        {
            if (disposable == null)
                throw new ArgumentNullException(nameof(disposable));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

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