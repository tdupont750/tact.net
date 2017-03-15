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
        private readonly RegistrationMaps _maps;

        private bool _isDisposed;

        protected readonly ILog Log;
        protected readonly int? MaxDisposeParallelization;

        protected abstract IReadOnlyList<IResolutionHandler> ResolutionHandlers { get; }

        protected ContainerBase(ILog log, int? maxDisposeParallelization)
            : this(log, maxDisposeParallelization, RegistrationMaps.Create())
        {
            this.RegisterInstance(log);
        }

        protected ContainerBase(ILog log, int? maxDisposeParallelization, ContainerBase parentScope)
            : this(log, maxDisposeParallelization, parentScope._maps.Clone())
        {
            _maps.InitalizeScope(this);
        }

        private ContainerBase(ILog log, int? maxDisposeParallelization, RegistrationMaps maps)
        {
            _maps = maps ?? throw new ArgumentNullException(nameof(maps));
            Log = log ?? throw new ArgumentNullException(nameof(log));
            MaxDisposeParallelization = maxDisposeParallelization;
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

            var lifetimeManagersToDispose = _maps.GetLifetimeManagersToDispose(this);

            // Bail if there is nothing to dispose
            if (lifetimeManagersToDispose.Count == 0)
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

                if (_maps.MultiRegistrationMap.TryGetValue(type, out Dictionary<string, ILifetimeManager> registrations))
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

        public abstract IContainer BeginScope();

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

                if (_maps.RegistrationMap.TryGetValue(fromType, out ILifetimeManager previous))
                    Log.Debug("Type: {0} - {1} - Replaced {2}", fromType.Name, lifetimeManager.Description, previous.Description);
                else
                    Log.Debug("Type: {0} - {1}", fromType.Name, lifetimeManager.Description);

                _maps.RegistrationMap[fromType] = lifetimeManager;

                if (lifetimeManager.IsScoped && !_maps.ScopedKeys.Contains(fromType))
                    _maps.ScopedKeys.Add(fromType);

                if (lifetimeManager.IsDisposable && !_maps.DisposableKeys.Contains(fromType))
                    _maps.DisposableKeys.Add(fromType);
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

                if (_maps.MultiRegistrationMap.TryGetValue(fromType, out Dictionary<string, ILifetimeManager> registrations))
                    registrations[key] = lifetimeManager;
                else
                    _maps.MultiRegistrationMap[fromType] = new Dictionary<string, ILifetimeManager>
                    {
                        { key, lifetimeManager }
                    };

                if (lifetimeManager.IsScoped && !_maps.MultiScopedKeys.Contains(fromType))
                    _maps.MultiScopedKeys.Add(fromType);

                if (lifetimeManager.IsDisposable && !_maps.MultiDisposableKeys.Contains(fromType))
                    _maps.MultiDisposableKeys.Add(fromType);

                Log.Debug("Type: {0} - Key: {1} - {2}", fromType.Name, key, lifetimeManager.Description);
            }
        }

        public object CreateInstance(Type type, Stack<Type> stack)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var constructor = type.EnsureSingleCostructor();
            var parameterTypes = constructor.GetParameterTypes();
            var arguments = new object[parameterTypes.Count];
            for (var i = 0; i < parameterTypes.Count; i++)
            {
                var parameterType = parameterTypes[i];
                arguments[i] = Resolve(parameterType, stack);
            }

            return constructor.EfficientInvoke(arguments);
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

                if (_maps.RegistrationMap.TryGetValue(type, out ILifetimeManager lifetimeManager))
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

                if (_maps.MultiRegistrationMap.TryGetValue(type, out Dictionary<string, ILifetimeManager> registrations))
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

        protected class RegistrationMaps
        {
            public readonly Dictionary<Type, ILifetimeManager> RegistrationMap;
            public readonly Dictionary<Type, Dictionary<string, ILifetimeManager>> MultiRegistrationMap;
            public readonly List<Type> ScopedKeys;
            public readonly List<Type> MultiScopedKeys;
            public readonly List<Type> DisposableKeys;
            public readonly List<Type> MultiDisposableKeys;

            private RegistrationMaps(
                Dictionary<Type, ILifetimeManager> registrationMap,
                Dictionary<Type, Dictionary<string, ILifetimeManager>> multiRegistrationMap,
                List<Type> scopedKeys,
                List<Type> multiScopedKeys,
                List<Type> disposableKeys,
                List<Type> multiDisposableKeys)
            {
                RegistrationMap = registrationMap;
                MultiRegistrationMap = multiRegistrationMap;
                ScopedKeys = scopedKeys;
                MultiScopedKeys = multiScopedKeys;
                DisposableKeys = disposableKeys;
                MultiDisposableKeys = multiDisposableKeys;
            }

            public static RegistrationMaps Create()
            {
                return new RegistrationMaps(
                    new Dictionary<Type, ILifetimeManager>(),
                    new Dictionary<Type, Dictionary<string, ILifetimeManager>>(),
                    new List<Type>(),
                    new List<Type>(),
                    new List<Type>(),
                    new List<Type>());
            }

            public RegistrationMaps Clone()
            {
                var registrationMap = new Dictionary<Type, ILifetimeManager>(RegistrationMap);

                var multiRegistrationMap = new Dictionary<Type, Dictionary<string, ILifetimeManager>>();
                foreach (var pair in MultiRegistrationMap)
                    multiRegistrationMap[pair.Key] = new Dictionary<string, ILifetimeManager>(pair.Value);

                var scopedKeys = ScopedKeys.ToList();
                var multiScopedKeys = MultiScopedKeys.ToList();

                var disposableKeys = DisposableKeys.ToList();
                var mutliDisposableKeys = MultiDisposableKeys.ToList();

                return new RegistrationMaps(
                    registrationMap,
                    multiRegistrationMap,
                    scopedKeys,
                    multiScopedKeys,
                    disposableKeys, 
                    mutliDisposableKeys);
            }

            public void InitalizeScope(IContainer scope)
            {
                for (var i = 0; i < ScopedKeys.Count; i++)
                {
                    var type = ScopedKeys[i];
                    RegistrationMap[type] = RegistrationMap[type].BeginScope(scope);
                }

                for (var i = 0; i < MultiScopedKeys.Count; i++)
                {
                    var type = MultiScopedKeys[i];
                    var values = MultiRegistrationMap[type];
                    var clones = MultiRegistrationMap[type] = new Dictionary<string, ILifetimeManager>(values.Count);
                    foreach (var pair in values)
                        clones[pair.Key] = pair.Value.BeginScope(scope);
                }
            }

            public IReadOnlyList<ILifetimeManager> GetLifetimeManagersToDispose(IContainer scope)
            {
                // Creating this collection with foreach is 15% faster than linq.
                var lifetimeManagersToDispose = new List<ILifetimeManager>();

                foreach (var key in MultiDisposableKeys)
                    foreach (var pair in MultiRegistrationMap[key])
                        if (pair.Value.RequiresDispose(scope))
                            lifetimeManagersToDispose.Add(pair.Value);

                foreach (var key in DisposableKeys)
                {
                    var lifetimeManager = RegistrationMap[key];
                    if (lifetimeManager.RequiresDispose(scope))
                        lifetimeManagersToDispose.Add(lifetimeManager);
                }

                return lifetimeManagersToDispose;
            }
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