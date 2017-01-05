using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Tact.Collections
{
    public class ObjectPool<T> : IDisposable
    {
        private readonly Func<T> _factory;
        private readonly T[] _pool;
        private readonly int _maxSize;

        private volatile int _currentTicket;

        private int _ticketCounter = -1;
        private int _index = -1;
        private bool _isDisposed;

        public ObjectPool(int maxSize, Func<T> factory)
        {
            if (maxSize == 0)
                throw new ArgumentException("Max size is required", nameof(maxSize));

            if (factory == null)
                throw new ArgumentNullException(nameof(factory));

            _factory = factory;
            _pool = new T[maxSize];
            _maxSize = maxSize - 1;
        }

        public T Acquire()
        {
            var nextTicket = EnterLock();

            if (_isDisposed)
            {
                _currentTicket = nextTicket;
                throw new ObjectDisposedException(nameof(ObjectPool<T>));
            }

            if (_index == -1)
            {
                _currentTicket = nextTicket;
                return _factory();
            }

            var value = _pool[_index--];
            _currentTicket = nextTicket;
            return value;
        }
        
        public bool TryAcquire(out T value)
        {
            var nextTicket = EnterLock();

            if (_isDisposed)
            {
                _currentTicket = nextTicket;
                throw new ObjectDisposedException(nameof(ObjectPool<T>));
            }

            if (_index == -1)
            {
                _currentTicket = nextTicket;
                value = default(T);
                return false;
            }

            value = _pool[_index--];
            _currentTicket = nextTicket;
            return true;
        }

        public bool Release(T value)
        {
            var nextTicket = EnterLock();
            var result = !_isDisposed && _index != _maxSize;

            if (result)
                _pool[++_index] = value;

            _currentTicket = nextTicket;
            return result;
        }

        public UsableValue Use()
        {
            var value = Acquire();
            return new UsableValue(this, value);
        }

        public void Dispose()
        {
            var nextTicket = EnterLock();

            try
            {
                if (_isDisposed)
                    return;

                foreach (var value in _pool)
                {
                    var disposable = value as IDisposable;
                    if (disposable == null) break;
                    disposable.Dispose();
                }

                Array.Clear(_pool, 0, _pool.Length);
            }
            finally
            {
                _isDisposed = true;
                _currentTicket = nextTicket;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int EnterLock()
        {
            var myTicket = Interlocked.Increment(ref _ticketCounter);

            if (myTicket != _currentTicket)
                while (myTicket != _currentTicket)
                {
                }

            return myTicket + 1;
        }

        public struct UsableValue : IDisposable
        {
            private readonly ObjectPool<T> _pool;

            public readonly T Value;

            public UsableValue(ObjectPool<T> pool, T value)
            {
                _pool = pool;
                Value = value;
            }

            public void Dispose()
            {
                _pool.Release(Value);
            }
        }
    }
}
