using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tact
{
    public static class CollectionExtensions
    {
        public static Task<IList<TOutput>> WhenAll<TInput, TOutput>(
            this ICollection<TInput> collection,
            Func<TInput, Task<TOutput>> func,
            int? maxParallelization = null)
        {
            return collection.WhenAll(CancellationToken.None, (input, index, token) => func(input), maxParallelization);
        }

        public static Task<IList<TOutput>> WhenAll<TInput, TOutput>(
            this ICollection<TInput> collection,
            Func<TInput, int, Task<TOutput>> func,
            int? maxParallelization = null)
        {
            return collection.WhenAll(CancellationToken.None, (input, index, token) => func(input, index), maxParallelization);
        }

        public static Task<IList<TOutput>> WhenAll<TInput, TOutput>(
            this ICollection<TInput> collection,
            CancellationToken cancelToken,
            Func<TInput, CancellationToken, Task<TOutput>> func,
            int? maxParallelization = null)
        {
            return collection.WhenAll(cancelToken, (input, index, token) => func(input, token), maxParallelization);
        }

        public static async Task<IList<TOutput>> WhenAll<TInput, TOutput>(
            this ICollection<TInput> collection,
            CancellationToken cancelToken,
            Func<TInput, int, CancellationToken, Task<TOutput>> func,
            int? maxParallelization = null)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            if (func == null)
                throw new ArgumentNullException(nameof(func));

            var results = new TOutput[collection.Count];

            await collection
                .WhenAll(
                    cancelToken,
                    async (item, index, token) =>
                    {
                        results[index] = await func(item, index, cancelToken).ConfigureAwait(false);
                    },
                    maxParallelization)
                .ConfigureAwait(false);

            return results;
        }
    }
}