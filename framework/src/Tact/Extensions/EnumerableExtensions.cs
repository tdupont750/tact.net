using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tact
{
    public static class EnumerableExtensions
    {
        public static Task WhenAll<T>(
            this IEnumerable<T> enumerable,
            Func<T, Task> func,
            int? maxParallelization = null)
        {
            return enumerable.WhenAll(CancellationToken.None, (item, index, token) => func(item), maxParallelization);
        }

        public static Task WhenAll<T>(
            this IEnumerable<T> enumerable,
            Func<T, int, Task> func,
            int? maxParallelization = null)
        {
            return enumerable.WhenAll(CancellationToken.None, (item, index, token) => func(item, index), maxParallelization);
        }

        public static Task WhenAll<T>(
            this IEnumerable<T> enumerable,
            CancellationToken cancelToken,
            Func<T, CancellationToken, Task> func,
            int? maxParallelization = null)
        {
            return enumerable.WhenAll(cancelToken, (item, index, token) => func(item, token), maxParallelization);
        }

        public static async Task WhenAll<T>(
            this IEnumerable<T> enumerable,
            CancellationToken cancelToken, 
            Func<T, int, CancellationToken, Task> func, 
            int? maxParallelization = null)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var exceptions = new ConcurrentQueue<Exception>();
            var maxCount = maxParallelization ?? Environment.ProcessorCount;
            var tasks = new List<Task>(maxCount);
            
            using (var enumerator = new EnumeratorInfo<T>(enumerable))
            {
                for (var i = 0; i < maxCount; i++)
                {
                    T item;
                    int index;
                    if (!enumerator.TryGetNext(out item, out index))
                        break;

                    var task = RunLoopAsync(enumerator, item, index, exceptions, func, cancelToken);
                    tasks.Add(task);
                }

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            if (exceptions.Count > 0)
                throw new AggregateException(exceptions);
        }

        private static async Task RunLoopAsync<T>(
            EnumeratorInfo<T> enumerator, 
            T item,
            int index,
            ConcurrentQueue<Exception> exceptions,
            Func<T, int, CancellationToken, Task> func,
            CancellationToken cancelToken)
        {
            do
            {
                if (exceptions.Count > 0)
                    return;

                try
                {
                    await func(item, index, cancelToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.Enqueue(ex);
                    return;
                }
            }
            while (enumerator.TryGetNext(out item, out index));
        }

        private class EnumeratorInfo<T> : IDisposable
        {
            private readonly IEnumerator<T> _enumerator;

            private readonly object _lock = new object();

            private int _index = -1;

            public EnumeratorInfo(IEnumerable<T> enumerable)
            {
                _enumerator = enumerable.GetEnumerator();
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool TryGetNext(out T item, out int index)
            {
                lock (_lock)
                {
                    if (_enumerator.MoveNext())
                    {
                        index = Interlocked.Increment(ref _index);
                        item = _enumerator.Current;
                        return true;
                    }

                    index = -1;
                    item = default(T);
                    return false;
                }
            }
        }
    }
}