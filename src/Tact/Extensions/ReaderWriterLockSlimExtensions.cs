using System;
using System.Threading;

namespace Tact
{
    public static class ReaderWriterLockSlimExtensions
    {
        public static IDisposable UseReadLock(this ReaderWriterLockSlim lockSlim, int millisecondsTimeout)
        {
            return lockSlim.UseReadLock(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public static IDisposable UseReadLock(this ReaderWriterLockSlim lockSlim, TimeSpan timeout)
        {
            var result = lockSlim.TryEnterReadLock(timeout);
            if (!result) throw new TimeoutException("Unable to obtain a read lock");
            return new ReaderWriterLockSlimWrapper(lockSlim, false);
        }

        public static IDisposable UseReadLock(this ReaderWriterLockSlim lockSlim)
        {
            lockSlim.EnterReadLock();
            return new ReaderWriterLockSlimWrapper(lockSlim, false);
        }

        public static IDisposable UseWriteLock(this ReaderWriterLockSlim lockSlim, int millisecondsTimeout)
        {
            return lockSlim.UseWriteLock(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public static IDisposable UseWriteLock(this ReaderWriterLockSlim lockSlim, TimeSpan timeout)
        {
            var result = lockSlim.TryEnterWriteLock(timeout);
            if (!result) throw new TimeoutException("Unable to obtain a write lock");
            return new ReaderWriterLockSlimWrapper(lockSlim, true);
        }

        public static IDisposable UseWriteLock(this ReaderWriterLockSlim lockSlim)
        {
            lockSlim.EnterWriteLock();
            return new ReaderWriterLockSlimWrapper(lockSlim, true);
        }

        private struct ReaderWriterLockSlimWrapper : IDisposable
        {
            private readonly ReaderWriterLockSlim _lockSlim;
            private readonly bool _isWrite;

            public ReaderWriterLockSlimWrapper(ReaderWriterLockSlim lockSlim, bool isWrite)
            {
                _lockSlim = lockSlim;
                _isWrite = isWrite;
            }

            public void Dispose()
            {
                if (_isWrite)
                    _lockSlim.ExitWriteLock();
                else
                    _lockSlim.ExitReadLock();
            }
        }
    }
}