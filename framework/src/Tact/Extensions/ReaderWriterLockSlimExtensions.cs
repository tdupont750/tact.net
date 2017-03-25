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
            if (lockSlim == null)
                throw new ArgumentNullException(nameof(lockSlim));

            var result = lockSlim.TryEnterReadLock(timeout);
            if (!result)
                throw new TimeoutException("Unable to obtain a read lock");

            return new ReaderWriterLockSlimWrapper(lockSlim, LockType.Read);
        }

        public static IDisposable UseReadLock(this ReaderWriterLockSlim lockSlim)
        {
            if (lockSlim == null)
                throw new ArgumentNullException(nameof(lockSlim));

            lockSlim.EnterReadLock();
            return new ReaderWriterLockSlimWrapper(lockSlim, LockType.Read);
        }

        public static IDisposable UseUpgradeableReadLock(this ReaderWriterLockSlim lockSlim, int millisecondsTimeout)
        {
            return lockSlim.UseUpgradeableReadLock(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public static IDisposable UseUpgradeableReadLock(this ReaderWriterLockSlim lockSlim, TimeSpan timeout)
        {
            if (lockSlim == null)
                throw new ArgumentNullException(nameof(lockSlim));

            var result = lockSlim.TryEnterUpgradeableReadLock(timeout);
            if (!result)
                throw new TimeoutException("Unable to obtain a read lock");

            return new ReaderWriterLockSlimWrapper(lockSlim, LockType.Upgradable);
        }

        public static IDisposable UseUpgradeableReadLock(this ReaderWriterLockSlim lockSlim)
        {
            if (lockSlim == null)
                throw new ArgumentNullException(nameof(lockSlim));

            lockSlim.EnterUpgradeableReadLock();
            return new ReaderWriterLockSlimWrapper(lockSlim, LockType.Upgradable);
        }

        public static IDisposable UseWriteLock(this ReaderWriterLockSlim lockSlim, int millisecondsTimeout)
        {
            return lockSlim.UseWriteLock(TimeSpan.FromMilliseconds(millisecondsTimeout));
        }

        public static IDisposable UseWriteLock(this ReaderWriterLockSlim lockSlim, TimeSpan timeout)
        {
            if (lockSlim == null)
                throw new ArgumentNullException(nameof(lockSlim));

            var result = lockSlim.TryEnterWriteLock(timeout);
            if (!result)
                throw new TimeoutException("Unable to obtain a write lock");

            return new ReaderWriterLockSlimWrapper(lockSlim, LockType.Write);
        }

        public static IDisposable UseWriteLock(this ReaderWriterLockSlim lockSlim)
        {
            if (lockSlim == null)
                throw new ArgumentNullException(nameof(lockSlim));

            lockSlim.EnterWriteLock();
            return new ReaderWriterLockSlimWrapper(lockSlim, LockType.Write);
        }

        private enum LockType : short
        {
            Read,
            Write,
            Upgradable
        }

        private struct ReaderWriterLockSlimWrapper : IDisposable
        {
            private readonly ReaderWriterLockSlim _lockSlim;
            private readonly LockType _lockType;

            public ReaderWriterLockSlimWrapper(ReaderWriterLockSlim lockSlim, LockType lockType)
            {
                _lockSlim = lockSlim;
                _lockType = lockType;
            }

            public void Dispose()
            {
                switch (_lockType)
                {
                    case LockType.Read:
                        _lockSlim.ExitReadLock();
                        break;

                    case LockType.Write:
                        _lockSlim.ExitWriteLock();
                        break;

                    case LockType.Upgradable:
                        _lockSlim.EnterUpgradeableReadLock();
                        break;
                }
            }
        }
    }
}