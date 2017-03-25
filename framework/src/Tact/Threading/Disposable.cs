using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tact.Threading
{
    public static class Disposable
    {
        public static Task Async(
            IEnumerable<object> disposables,
            CancellationToken cancelToken = default(CancellationToken),
            int? maxParallelization = null)
        {
            return disposables.WhenAll(cancelToken, (o, token) => Async(o, token), maxParallelization);
        }

        public static Task Async(
            object disposable,
            CancellationToken cancelToken = default(CancellationToken),
            bool runStandardDisposeAsync = true)
        {
            var asyncDisposable = disposable as IAsyncDisposable;
            if (asyncDisposable != null)
                return asyncDisposable.DisposeAsync(cancelToken);

            var standardDisposable = disposable as IDisposable;
            if (standardDisposable != null)
            {
                if (runStandardDisposeAsync)
                    return Task.Run(() => standardDisposable.Dispose(), cancelToken);

                standardDisposable.Dispose();
            }

            return Task.CompletedTask;
        }
    }
}