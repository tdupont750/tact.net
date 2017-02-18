using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tact.Collections;
using Tact.Diagnostics;
using Tact.Practices.LifetimeManagers;
using Tact.Practices.ResolutionHandlers;

namespace Tact.Practices.Base
{
    public abstract class ContainerBase : IContainer
    {
        private const int PoolSize = 1000;

        private static readonly ObjectPool<Stack<Type>> StackPool 
            = new ObjectPool<Stack<Type>>(PoolSize, () => new Stack<Type>(), stack => stack.Clear());

        private static readonly ObjectPool<Dictionary<Type, ILifetimeManager>> MapPool 
            = new ObjectPool<Dictionary<Type, ILifetimeManager>>(PoolSize, () => new Dictionary<Type, ILifetimeManager>(), map => map.Clear());

        private static readonly ObjectPool<Dictionary<Type, Dictionary<string, ILifetimeManager>>> MultiMapPool
            = new ObjectPool<Dictionary<Type, Dictionary<string, ILifetimeManager>>>(PoolSize, () => new Dictionary<Type, Dictionary<string, ILifetimeManager>>(), map => map.Clear());

        private static readonly ObjectPool<Dictionary<string, ILifetimeManager>> KeyMapPool
            = new ObjectPool<Dictionary<string, ILifetimeManager>>(PoolSize, () => new Dictionary<string, ILifetimeManager>(), map => map.Clear());

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private Dictionary<Type, ILifetimeManager> _lifetimeManagerMap = MapPool.Acquire();
        private Dictionary<Type, Dictionary<string, ILifetimeManager>> _multiRegistrationMap = MultiMapPool.Acquire();
        private bool _isDisposed;

        protected readonly ILog Log;
        protected readonly int? MaxDisposeParallelization;

        protected abstract IReadOnlyList<IResolutionHandler> ResolutionHandlers { get; }

        protected ContainerBase(ILog log, int? maxDisposeParallelization)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            Log = log;
            MaxDisposeParallelization = maxDisposeParallelization;
            
            this.RegisterInstance(log);
        }

        public void Dispose()
        {
            DisposeAsync(CancellationToken.None).WaitIfNeccessary();
        }

        public Task DisposeAsync(CancellationToken cancelToken)
        {
            using (_lock.UseWriteLock())
            {
                if (_isDisposed) return Task.CompletedTask;
                _isDisposed = true;
            }

            // Get all managers that need to be disposed.
            var lifetimeManagersToDispose = _multiRegistrationMap.Values
                .SelectMany(v => v.Values)
                .Concat(_lifetimeManagerMap.Values)
                .Where(lm => lm.RequiresDispose(this))
                .ToArray();

            // Return the multi registration entry maps to the pool
            foreach (var keyMap in _multiRegistrationMap)
                KeyMapPool.Release(keyMap.Value);

            // Return the multi registration map to the pool
            MultiMapPool.Release(_multiRegistrationMap);
            _multiRegistrationMap = null;

            // Return the registration map to the pool
            MapPool.Release(_lifetimeManagerMap);
            _lifetimeManagerMap = null;

            // Bail if there is nothing to dispose
            if (lifetimeManagersToDispose.Length == 0)
                return Task.CompletedTask;

            // Dispose async
            return lifetimeManagersToDispose.WhenAll(
                cancelToken,
                (manager, token) => manager.DisposeAsync(this, cancelToken),
                MaxDisposeParallelization);
        }

        public object Resolve(Type type)
        {
            using (var stack = StackPool.Use())
                return Resolve(type, stack);
        }

        public bool TryResolve(Type type, out object result)
        {
            using (var stack = StackPool.Use())
                return TryResolve(type, stack, false, out result);
        }

        public object Resolve(Type type, Stack<Type> stack)
        {
            object result;
            if (TryResolve(type, stack, true, out result))
                return result;

            throw new InvalidOperationException("No matching registrations found");
        }

        public bool TryResolve(Type type, Stack<Type> stack, out object result)
        {
            return TryResolve(type, stack, false, out result);
        }

        public object Resolve(Type type, string key)
        {
            using (var stack = StackPool.Use())
                return Resolve(type, key, stack);
        }

        public bool TryResolve(Type type, string key, out object result)
        {
            using (var stack = StackPool.Use())
                return TryResolve(type, key, stack, false, out result);
        }

        public object Resolve(Type type, string key, Stack<Type> stack)
        {
            object result;
            if (TryResolve(type, key, stack, true, out result))
                return result;

            throw new InvalidOperationException("No matching registrations found");
        }

        public bool TryResolve(Type type, string key, Stack<Type> stack, out object result)
        {
            return TryResolve(type, key, stack, false, out result);
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            using (var stack = StackPool.Use())
                return ResolveAll(type, stack);
        }

        public IEnumerable<object> ResolveAll(Type type, Stack<Type> stack)
        {
            var instances = new List<object>();

            using (EnterPush(type, stack))
            using (_lock.UseReadLock())
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                Dictionary<string, ILifetimeManager> registrations;
                if (_multiRegistrationMap.TryGetValue(type, out registrations))
                {
                    foreach (var lifetimeManager in registrations.Values)
                    {
                        var instance = lifetimeManager.Resolve(stack);
                        instances.Add(instance);
                    }
                }
                else 
                {
                    foreach (var resolutionHandler in ResolutionHandlers)
                    {
                        object instance;
                        if (resolutionHandler.TryResolve(this, type, stack, false, out instance))
                            instances.Add(instance);
                    }
                }
            }

            return instances;
        }

        public IContainer BeginScope()
        {
            var scope = CreateScope();
            InitializeScope(this, scope);
            return scope;
        }

        IResolver IResolver.BeginScope()
        {
            return BeginScope();
        }

        public void Register(Type fromType, ILifetimeManager lifetimeManager)
        {
            using (_lock.UseWriteLock())
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                ILifetimeManager previous;
                if (_lifetimeManagerMap.TryGetValue(fromType, out previous))
                {
                    Log.Debug("Type: {0} - {1} - Replaced {2}", fromType.Name, lifetimeManager.Description,
                        previous.Description);
                }
                else
                    Log.Debug("Type: {0} - {1}", fromType.Name, lifetimeManager.Description);

                _lifetimeManagerMap[fromType] = lifetimeManager;
            }
        }

        public void Register(Type fromType, string key, ILifetimeManager lifetimeManager)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Required", nameof(key));

            using (_lock.UseWriteLock())
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                Dictionary<string, ILifetimeManager> registrations;
                if (_multiRegistrationMap.TryGetValue(fromType, out registrations))
                    registrations[key] = lifetimeManager;
                else
                {
                    var keyMap = KeyMapPool.Acquire();
                    keyMap.Add(key, lifetimeManager);
                    _multiRegistrationMap[fromType] = keyMap;
                }

                Log.Debug("Type: {0} - Key: {1} - {2}", fromType.Name, key, lifetimeManager.Description);
            }
        }

        public object CreateInstance(Type type, Stack<Type> stack)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var constructor = type.EnsureSingleCostructor();
            var parameterTypes = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
            var arguments = new object[parameterTypes.Length];
            for (var i = 0; i < parameterTypes.Length; i++)
            {
                var parameterType = parameterTypes[i];
                arguments[i] = Resolve(parameterType, stack);
            }

            return Activator.CreateInstance(type, arguments);
        }

        protected abstract ContainerBase CreateScope();

        private bool TryResolve(Type type, Stack<Type> stack, bool canThrow, out object result)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (stack == null)
                throw new ArgumentNullException(nameof(stack));

            using (EnterPush(type, stack))
            using (_lock.UseReadLock())
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                ILifetimeManager lifetimeManager;
                if (_lifetimeManagerMap.TryGetValue(type, out lifetimeManager))
                {
                    result = lifetimeManager.Resolve(stack);
                    return true;
                }

                foreach (var handler in ResolutionHandlers)
                    if (handler.TryResolve(this, type, stack, canThrow, out result))
                        return true;

                result = null;
                return false;
            }
        }

        private bool TryResolve(Type type, string key, Stack<Type> stack, bool canThrow, out object result)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (stack == null)
                throw new ArgumentNullException(nameof(stack));

            using (EnterPush(type, stack))
            using (_lock.UseReadLock())
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                Dictionary<string, ILifetimeManager> registrations;
                if (_multiRegistrationMap.TryGetValue(type, out registrations))
                    foreach (var lifetimeManager in registrations)
                        if (lifetimeManager.Key == key)
                        {
                            result = lifetimeManager.Value.Resolve(stack);
                            return true;
                        }

                foreach (var resolutionHandler in ResolutionHandlers)
                    if (resolutionHandler.TryResolve(this, type, stack, canThrow, out result))
                        return true;
            }

            result = null;
            return false;
        }

        private static void InitializeScope(ContainerBase source, ContainerBase target)
        {
            using (source._lock.UseReadLock())
            using (target._lock.UseWriteLock())
            {
                if (source._isDisposed)
                    throw new ObjectDisposedException(source.GetType().Name);

                if (target._isDisposed)
                    throw new ObjectDisposedException(target.GetType().Name);

                foreach (var pair in source._lifetimeManagerMap)
                {
                    var clone = pair.Value.BeginScope(target);
                    target._lifetimeManagerMap[pair.Key] = clone;
                }

                foreach (var pair in source._multiRegistrationMap)
                {
                    var clones = KeyMapPool.Acquire();

                    foreach (var lifetimeManager in pair.Value)
                    {
                        var clone = lifetimeManager.Value.BeginScope(target);
                        clones[lifetimeManager.Key] = clone;
                    }

                    target._multiRegistrationMap[pair.Key] = clones;
                }
            }
        }
        
        private static IDisposable EnterPush(Type type, Stack<Type> stack)
        {
            if (stack.Contains(type))
                throw new InvalidOperationException("Recursive resolution detected");

            return new DisposablePush(type, stack);
        }

        private struct DisposablePush : IDisposable
        {
            private readonly Type _type;
            private readonly Stack<Type> _stack;

            public DisposablePush(Type type, Stack<Type> stack)
            {
                _type = type;
                _stack = stack;

                stack.Push(type);
            }

            public void Dispose()
            {
                var type = _stack.Pop();

                if (type != _type)
                    throw new InvalidOperationException("Stack became unsynchronized");
            }
        }
    }
}