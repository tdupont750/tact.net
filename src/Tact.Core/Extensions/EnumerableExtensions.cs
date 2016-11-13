using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tact
{
    public static class EnumerableExtensions
    {
        public static Task<IEnumerable<TOutput>> WhenAll<TInput, TOutput>(
            this IEnumerable<TInput> enumerable,
            Func<TInput, Task<TOutput>> func,
            int? maxParallelization = null)
        {
            return enumerable.WhenAll(CancellationToken.None, (input, token) => func(input), maxParallelization);
        }

        public static async Task<IEnumerable<TOutput>> WhenAll<TInput, TOutput>(
            this IEnumerable<TInput> enumerable,
            CancellationToken cancelToken,
            Func<TInput, CancellationToken, Task<TOutput>> func,
            int? maxParallelization = null)
        {
            var results = new ConcurrentQueue<TOutput>();

            await enumerable
                .WhenAll(
                    cancelToken,
                    async (item, token) =>
                    {
                        var result = await func(item, cancelToken).ConfigureAwait(false);
                        results.Enqueue(result);
                    },
                    maxParallelization)
                .ConfigureAwait(false);

            return results;
        }

        public static Task WhenAll<T>(
            this IEnumerable<T> enumerable,
            Func<T, Task> func,
            int? maxParallelization = null)
        {
            return enumerable.WhenAll(CancellationToken.None, (item, token) => func(item), maxParallelization);
        }

        public static async Task WhenAll<T>(
            this IEnumerable<T> enumerable,
            CancellationToken cancelToken, 
            Func<T, CancellationToken, Task> func, 
            int? maxParallelization = null)
        {
            var exceptions = new ConcurrentQueue<Exception>();
            var maxCount = maxParallelization ?? Environment.ProcessorCount;
            var tasks = new List<Task>(maxCount);
            
            using (var enumerator = enumerable.GetEnumerator())
            {
                for (var i = 0; i < maxCount; i++)
                {
                    T item;
                    if (!TryGetNext(enumerator, out item))
                        break;

                    var task = RunLoopAsync(enumerator, item, exceptions, func, cancelToken);
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        private static async Task RunLoopAsync<T>(
            IEnumerator<T> enumerator, 
            T item,
            ConcurrentQueue<Exception> exceptions,
            Func<T, CancellationToken, Task> func,
            CancellationToken cancelToken)
        {
            do
            {
                try
                {
                    await func(item, cancelToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                }
            }
            while (TryGetNext(enumerator, out item));
        }

        private static bool TryGetNext<T>(IEnumerator<T> enumerator, out T item)
        {
            lock (enumerator)
            {
                if (enumerator.MoveNext())
                {
                    item = enumerator.Current;
                    return true;
                }

                item = default(T);
                return false;
            }
        }
    }
}