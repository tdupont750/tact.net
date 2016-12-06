using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tact.Diagnostics;
using Tact.Practices.LifetimeManagers;
using Tact.Practices.ResolutionHandlers;

namespace Tact.Practices.Base
{
    public abstract class ContainerBase : IContainer
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly Dictionary<Type, ILifetimeManager> _lifetimeManagerMap = new Dictionary<Type, ILifetimeManager>();
        private readonly Dictionary<Type, Dictionary<string, ILifetimeManager>> _multiRegistrationMap = new Dictionary<Type, Dictionary<string, ILifetimeManager>>();
        protected readonly ILog Log;

        private int _isDisposed;

        protected abstract IList<IResolutionHandler> ResolutionHandlers { get; }

        protected ContainerBase(ILog log)
        {
            if (log == null)
                throw new ArgumentNullException(nameof(log));

            Log = log;

            this.RegisterInstance(log);
        }

        public void Dispose()
        {
            DisposeAsync(CancellationToken.None).Wait();
        }

        public Task DisposeAsync(CancellationToken cancelToken)
        {
            var isDisposed = Interlocked.Increment(ref _isDisposed);
            if (isDisposed != 1) return Task.CompletedTask;

            ILifetimeManager[] lifetimeManagers;

            using (_lock.UseReadLock())
            {
                lifetimeManagers = _multiRegistrationMap.Values
                    .SelectMany(v => v.Values)
                    .Concat(_lifetimeManagerMap.Values)
                    .ToArray();
            }

            return lifetimeManagers.WhenAll(cancelToken, (manager, token) => manager.DisposeAsync(this, cancelToken));
        }

        public object Resolve(Type type)
        {
            var stack = new Stack<Type>();
            object result;
            TryResolve(type, stack, true, out result);
            return result;
        }

        public bool TryResolve(Type type, out object result)
        {
            var stack = new Stack<Type>();
            return TryResolve(type, stack, false, out result);
        }

        public object Resolve(Type type, Stack<Type> stack)
        {
            object result;
            TryResolve(type, stack, true, out result);
            return result;
        }

        public bool TryResolve(Type type, Stack<Type> stack, out object result)
        {
            return TryResolve(type, stack, false, out result);
        }

        public object Resolve(Type type, string key)
        {
            var stack = new Stack<Type>();
            object result;
            TryResolve(type, key, stack, true, out result);
            return result;
        }

        public bool TryResolve(Type type, string key, out object result)
        {
            var stack = new Stack<Type>();
            return TryResolve(type, key, stack, false, out result);
        }

        public object Resolve(Type type, string key, Stack<Type> stack)
        {
            object result;
            TryResolve(type, key, stack, true, out result);
            return result;
        }

        public bool TryResolve(Type type, string key, Stack<Type> stack, out object result)
        {
            return TryResolve(type, key, stack, false, out result);
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            var stack = new Stack<Type>();
            return ResolveAll(type, stack);
        }

        public IEnumerable<object> ResolveAll(Type type, Stack<Type> stack)
        {
            var instances = new List<object>();

            using (EnterPush(type, stack))
            using (_lock.UseReadLock())
            {
                if (_multiRegistrationMap.ContainsKey(type))
                {
                    var registrations = _multiRegistrationMap[type];
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

        public IResolver BeginScope()
        {
            var scope = CreateScope();
            InitializeScope(this, scope);
            return scope;
        }

        public void Register(Type fromType, ILifetimeManager lifetimeManager)
        {
            using (_lock.UseWriteLock())
            {
                if (_lifetimeManagerMap.ContainsKey(fromType))
                {
                    var previous = _lifetimeManagerMap[fromType];
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
                if (_multiRegistrationMap.ContainsKey(fromType))
                    _multiRegistrationMap[fromType][key] = lifetimeManager;
                else
                    _multiRegistrationMap[fromType] = new Dictionary<string, ILifetimeManager> {{key, lifetimeManager}};

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
                if (_lifetimeManagerMap.ContainsKey(type))
                {
                    result = _lifetimeManagerMap[type].Resolve(stack);
                    return true;
                }

                foreach (var handler in ResolutionHandlers)
                {
                    if (handler.TryResolve(this, type, stack, canThrow, out result))
                        return true;
                }

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
                if (_multiRegistrationMap.ContainsKey(type))
                {
                    var registrations = _multiRegistrationMap[type];
                    foreach (var lifetimeManager in registrations)
                        if (lifetimeManager.Key == key)
                        {
                            result = lifetimeManager.Value.Resolve(stack);
                            return true;
                        }
                }

                foreach (var resolutionHandler in ResolutionHandlers)
                {
                    if (resolutionHandler.TryResolve(this, type, stack, canThrow, out result))
                        return true;
                }
            }

            result = null;
            return false;
        }

        private static void InitializeScope(ContainerBase source, ContainerBase target)
        {
            using (source._lock.UseReadLock())
            using (target._lock.UseWriteLock())
            {
                foreach (var pair in source._lifetimeManagerMap)
                {
                    var clone = pair.Value.BeginScope(target);
                    target._lifetimeManagerMap[pair.Key] = clone;
                }

                foreach (var pair in source._multiRegistrationMap)
                {
                    var clones = new Dictionary<string, ILifetimeManager>();

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