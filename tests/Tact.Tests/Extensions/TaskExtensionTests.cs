using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tact.Tests.Extensions
{
    public class TaskExtensionTests
    {
        [Fact]
        public async Task IgnoreCancellation()
        {
            using (var source = new CancellationTokenSource())
            {
                var task = Task.Delay(100, source.Token);
                source.Cancel();
                // ReSharper disable once MethodSupportsCancellation
                await task.IgnoreCancellation().ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task IgnoreCancellationWithToken()
        {
            using (var source = new CancellationTokenSource())
            {
                var task = Task.Delay(100, source.Token);
                source.Cancel();
                await task.IgnoreCancellation(source.Token).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task IgnoreCancellationWithInvalidToken()
        {
            using (var source1 = new CancellationTokenSource())
            using (var source2 = new CancellationTokenSource())
            {
                var task = Task.Delay(100, source1.Token);
                source1.Cancel();
                await Assert
                    .ThrowsAsync<TaskCanceledException>(() => task.IgnoreCancellation(source2.Token))
                    .ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task IgnoreCancellationWithException()
        {
            var task = Run(async () =>
            {
                // ReSharper disable once MethodSupportsCancellation
                await Task.Delay(100).ConfigureAwait(false);
                throw new InvalidOperationException();
            });
            await Assert
                .ThrowsAsync<InvalidOperationException>(() => task.IgnoreCancellation())
                .ConfigureAwait(false);
        }

        [Fact]
        public async Task GenericIgnoreCancellation()
        {
            using (var source = new CancellationTokenSource())
            {
                var task = Run(async () =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    await Task.Delay(100, source.Token).ConfigureAwait(false);
                    return 2;
                }) ;
                source.Cancel();
                // ReSharper disable once MethodSupportsCancellation
                var result = await task.IgnoreCancellation().ConfigureAwait(false);
                Assert.Equal(0, result);
            }
        }

        [Fact]
        public async Task GenericIgnoreCancellationWithToken()
        {
            using (var source = new CancellationTokenSource())
            {
                var task = Run(async () =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    await Task.Delay(100, source.Token).ConfigureAwait(false);
                    return 2;
                });
                source.Cancel();
                await task.IgnoreCancellation(source.Token).ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task GenericIgnoreCancellationWithInvalidToken()
        {
            using (var source1 = new CancellationTokenSource())
            using (var source2 = new CancellationTokenSource())
            {
                var task = Run(async () =>
                {
                    // ReSharper disable once AccessToDisposedClosure
                    await Task.Delay(100, source1.Token).ConfigureAwait(false);
                    return 2;
                });
                source1.Cancel();
                await Assert
                    .ThrowsAsync<TaskCanceledException>(() => task.IgnoreCancellation(source2.Token))
                    .ConfigureAwait(false);
            }
        }

        [Fact]
        public async Task GenericIgnoreCancellationWithException()
        {
            var task = Run<int>(async () =>
            {
                // ReSharper disable once MethodSupportsCancellation
                await Task.Delay(100).ConfigureAwait(false);
                throw new InvalidOperationException();
            });
            await Assert
                .ThrowsAsync<InvalidOperationException>(() => task.IgnoreCancellation())
                .ConfigureAwait(false);
        }

        [Fact]
        public void GetResult()
        {
            const int value = 42;
            var task = (Task) Task.FromResult(value);

            var result = task.GetResult();
            Assert.Equal(value, result);

            var genericResult = task.GetResult<int>();
            Assert.Equal(value, genericResult);
        }

        private static Task Run(Func<Task> action) => action();
        private static Task<T> Run<T>(Func<Task<T>> action) => action();
    }
}
