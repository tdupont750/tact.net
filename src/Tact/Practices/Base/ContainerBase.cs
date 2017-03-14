using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly Dictionary<Type, ILifetimeManager> _lifetimeManagerMap;
        private readonly Dictionary<Type, Dictionary<string, ILifetimeManager>> _multiRegistrationMap;
        private readonly List<Type> _scopedKeys;
        private readonly List<Type> _multiScopedKeys;

        private bool _isDisposed;

        protected readonly ILog Log;
        protected readonly int? MaxDisposeParallelization;

        protected abstract IReadOnlyList<IResolutionHandler> ResolutionHandlers { get; }

        protected ContainerBase(ILog log, int? maxDisposeParallelization)
            : this(
                  log,
                  maxDisposeParallelization,
                  new Dictionary<Type, ILifetimeManager>(),
                  new Dictionary<Type, Dictionary<string, ILifetimeManager>>(),
                  new List<Type>(),
                  new List<Type>())
        {
        }

        protected ContainerBase(
            ILog log, 
            int? maxDisposeParallelization, 
            Dictionary<Type, ILifetimeManager> lifetimeManagerMap, 
            Dictionary<Type, Dictionary<string, ILifetimeManager>> multiRegistrationMap,
            List<Type> scopedKeys,
            List<Type> multiScopedKeys)
        {
            _lifetimeManagerMap = lifetimeManagerMap;
            _multiRegistrationMap = multiRegistrationMap;
            _scopedKeys = scopedKeys;
            _multiScopedKeys = multiScopedKeys;

            Log = log ?? throw new ArgumentNullException(nameof(log));
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
            var stack = new Stack<Type>();
            return Resolve(type, stack);
        }

        public bool TryResolve(Type type, out object result)
        {
            var stack = new Stack<Type>();
            return TryResolve(type, stack, false, out result);
        }

        public object Resolve(Type type, Stack<Type> stack)
        {
            if (TryResolve(type, stack, true, out object result))
                return result;

            throw NewNoRegistrationFoundException(type, stack);
        }

        public bool TryResolve(Type type, Stack<Type> stack, out object result)
        {
            return TryResolve(type, stack, false, out result);
        }

        public object Resolve(Type type, string key)
        {
            var stack = new Stack<Type>();
            return Resolve(type, key, stack);
        }

        public bool TryResolve(Type type, string key, out object result)
        {
            var stack = new Stack<Type>();
            return TryResolve(type, key, stack, false, out result);
        }

        public object Resolve(Type type, string key, Stack<Type> stack)
        {
            if (TryResolve(type, key, stack, true, out object result))
                return result;

            throw NewNoRegistrationFoundException(type, stack);
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
                if (_isDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                if (_multiRegistrationMap.TryGetValue(type, out Dictionary<string, ILifetimeManager> registrations))
                    foreach (var lifetimeManager in registrations.Values)
                    {
                        var instance = lifetimeManager.Resolve(this, stack);
                        instances.Add(instance);
                    }
                else
                    foreach (var resolutionHandler in ResolutionHandlers)
                        if (resolutionHandler.TryResolve(this, type, stack, false, out object instance))
                            instances.Add(instance);
            }

            return instances;
        }

        public IContainer BeginScope()
        {
            var scope = CreateScope();
            InitializeScope(scope);
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

                if (_lifetimeManagerMap.TryGetValue(fromType, out ILifetimeManager previous))
                    Log.Debug("Type: {0} - {1} - Replaced {2}", fromType.Name, lifetimeManager.Description, previous.Description);
                else
                    Log.Debug("Type: {0} - {1}", fromType.Name, lifetimeManager.Description);

                _lifetimeManagerMap[fromType] = lifetimeManager;

                if (lifetimeManager.IsScoped && !_scopedKeys.Contains(fromType))
                    _scopedKeys.Add(fromType);
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

                if (_multiRegistrationMap.TryGetValue(fromType, out Dictionary<string, ILifetimeManager> registrations))
                    registrations[key] = lifetimeManager;
                else
                    _multiRegistrationMap[fromType] = new Dictionary<string, ILifetimeManager>
                    {
                        { key, lifetimeManager }
                    };

                if (lifetimeManager.IsScoped && !_multiScopedKeys.Contains(fromType))
                    _multiScopedKeys.Add(fromType);

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

        protected void CloneMaps(
            out Dictionary<Type, ILifetimeManager> lifetimeManagerMap,
            out Dictionary<Type, Dictionary<string, ILifetimeManager>> multiRegistrationMap,
            out List<Type> scopedKeys,
            out List<Type> multiScopedKeys)
        {
            using (_lock.UseReadLock())
            {
                lifetimeManagerMap = new Dictionary<Type, ILifetimeManager>(_lifetimeManagerMap);

                multiRegistrationMap = new Dictionary<Type, Dictionary<string, ILifetimeManager>>();
                foreach (var pair in _multiRegistrationMap)
                    multiRegistrationMap[pair.Key] = new Dictionary<string, ILifetimeManager>(pair.Value);

                scopedKeys = new List<Type>(_scopedKeys.Count);
                scopedKeys.AddRange(_scopedKeys);

                multiScopedKeys = new List<Type>(_multiScopedKeys.Count);
                multiScopedKeys.AddRange(_multiScopedKeys);
            }
        }

        private static void InitializeScope(ContainerBase scope)
        {
            for (var i = 0; i < scope._scopedKeys.Count; i++)
            {
                var type = scope._scopedKeys[i];
                scope._lifetimeManagerMap[type] = scope._lifetimeManagerMap[type].BeginScope(scope);
            }

            for (var i = 0; i < scope._multiScopedKeys.Count; i++)
            {
                var type = scope._multiScopedKeys[i];
                var values = scope._multiRegistrationMap[type];
                var clones = scope._multiRegistrationMap[type] = new Dictionary<string, ILifetimeManager>(values.Count);
                foreach (var pair in values)
                    clones[pair.Key] = pair.Value.BeginScope(scope);
            }
        }

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

                if (_lifetimeManagerMap.TryGetValue(type, out ILifetimeManager lifetimeManager))
                {
                    result = lifetimeManager.Resolve(this, stack);
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

                if (_multiRegistrationMap.TryGetValue(type, out Dictionary<string, ILifetimeManager> registrations))
                    foreach (var lifetimeManager in registrations)
                        if (lifetimeManager.Key == key)
                        {
                            result = lifetimeManager.Value.Resolve(this, stack);
                            return true;
                        }

                foreach (var resolutionHandler in ResolutionHandlers)
                    if (resolutionHandler.TryResolve(this, type, stack, canThrow, out result))
                        return true;
            }

            result = null;
            return false;
        }
                
        private static IDisposable EnterPush(Type type, Stack<Type> stack)
        {
            if (stack.Contains(type))
                throw new InvalidOperationException("Recursive resolution detected");

            return new DisposablePush(type, stack);
        }

        private static InvalidOperationException NewNoRegistrationFoundException(Type type, Stack<Type> stack)
        {
            var sb = new StringBuilder();

            sb.Append("No matching registrations found - Type: ");
            sb.Append(type.Name);
            sb.Append(" - Stack: ");

            if (stack.Count == 0)
                sb.Append("None");
            else
            {
                var t = stack.Pop();
                sb.Append(t.Name);

                while (stack.Count > 0)
                {
                    t = stack.Pop();
                    sb.Append(", ");
                    sb.Append(t.Name);
                }   
            }

            var message = sb.ToString();
            return new InvalidOperationException(message);
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