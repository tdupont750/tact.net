using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tact
{
    public static class CancellationTokenExtensions
    {
        public static CancellationTokenTask AsTask(this CancellationToken cancelToken)
        {
            return new CancellationTokenTask(cancelToken);
        }

        public struct CancellationTokenTask : IDisposable
        {
            private readonly CancellationToken _cancelToken;
            private readonly TaskCompletionSource<bool> _taskCompletionSource;
            private readonly CancellationTokenRegistration _registration;

            public CancellationTokenTask(CancellationToken cancelToken)
            {
                _cancelToken = cancelToken;
                var source = _taskCompletionSource = new TaskCompletionSource<bool>();
                _registration = cancelToken.Register(() => source.TrySetCanceled(cancelToken));
            }

            public Task Task => _taskCompletionSource.Task;

            public void Dispose()
            {
                _taskCompletionSource.TrySetResult(true);

                if (!_cancelToken.IsCancellationRequested)
                {
                    // ReSharper disable once ImpureMethodCallOnReadonlyValueField
                    _registration.Dispose();
                }
            }

            public static implicit operator Task(CancellationTokenTask wrapper) => wrapper.Task;
        }
    }
}
