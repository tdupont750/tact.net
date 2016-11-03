using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tact
{
    public static class TaskExtensions
    {
        public static Task IgnoreCancellation(this Task task, CancellationToken token)
        {
            // ReSharper disable once MethodSupportsCancellation
            return task
                .ContinueWith(t =>
                {
                    if (t.IsCanceled && token.IsCancellationRequested)
                        return Task.FromResult(true);

                    if (t.IsFaulted
                        && token.IsCancellationRequested
                        && t.Exception.InnerExceptions.All(e => e is TaskCanceledException))
                        return Task.FromResult(true);

                    return t;
                })
                .Unwrap();
        }

        public static Task IgnoreCancellation(this Task task)
        {
            return task
                .ContinueWith(t =>
                {
                    if (t.IsCanceled)
                        return Task.FromResult(true);

                    if (t.IsFaulted
                        && t.Exception.InnerExceptions.All(e => e is TaskCanceledException))
                        return Task.FromResult(true);

                    return t;
                })
                .Unwrap();
        }

        public static Task<T> IgnoreCancellation<T>(this Task<T> task, CancellationToken token)
        {
            // ReSharper disable once MethodSupportsCancellation
            return task
                .ContinueWith(t =>
                {
                    if (t.IsCanceled && token.IsCancellationRequested)
                        return Task.FromResult(default(T));

                    if (t.IsFaulted
                        && token.IsCancellationRequested
                        && t.Exception.InnerExceptions.All(e => e is TaskCanceledException))
                        return Task.FromResult(default(T));

                    return t;
                })
                .Unwrap();
        }

        public static Task<T> IgnoreCancellation<T>(this Task<T> task)
        {
            return task
                .ContinueWith(t =>
                {
                    if (t.IsCanceled)
                        return Task.FromResult(default(T));

                    if (t.IsFaulted
                        && t.Exception.InnerExceptions.All(e => e is TaskCanceledException))
                        return Task.FromResult(default(T));

                    return t;
                })
                .Unwrap();
        }
    }
}
