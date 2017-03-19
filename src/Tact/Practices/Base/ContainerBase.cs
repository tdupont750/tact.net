using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tact.Diagnostics;
using Tact.Practices.LifetimeManagers;
using Tact.Practices.ResolutionHandlers;
using Tact.Practices.ResolutionHandlers.Implementation;

namespace Tact.Practices.Base
{
    public abstract class ContainerBase : IContainer
    {
        private const int StackSize = 16;

        protected static List<IResolutionHandler> CreateDefaultHandlers(
            bool resolveLazy = true,
            bool resolveFunc = true,
            bool resolveUnregistered = true,
            bool resolveEnumerable = true,
            bool resolveCollection = true,
            bool resolveList = true,
            bool resolveGenerics = true)
        {
            var resolutionHandlers = new List<IResolutionHandler>();

            if (resolveLazy)
                resolutionHandlers.Add(new LazyResolutionHandler());

            if (resolveEnumerable || resolveCollection || resolveList)
                resolutionHandlers.Add(new EnumerableResolutionHandler(resolveEnumerable, resolveCollection, resolveList));

            if (resolveFunc)
                resolutionHandlers.Add(new FuncResolutionHandler());

            if (resolveUnregistered)
                resolutionHandlers.Add(new UnregisteredResolutionHandler());

            if (resolveGenerics)
                resolutionHandlers.Add(new GenericResolutionHandler());

            return resolutionHandlers;
        }

        private static readonly ConcurrentDictionary<Type, ConstructorInfo> ConstructorMap = new ConcurrentDictionary<Type, ConstructorInfo>();

        private static int _keySeed;

        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private readonly RegistrationMaps _maps;

        private bool _isDisposed;

        protected readonly ILog Log;
        protected readonly int? MaxDisposeParallelization;
        protected readonly bool IncludeUnkeyedInResovleAll;

        protected abstract IReadOnlyList<IResolutionHandler> ResolutionHandlers { get; }

        protected ContainerBase(ILog log, int? maxDisposeParallelization, bool includeUnkeyedInResovleAll)
            : this(log, maxDisposeParallelization, includeUnkeyedInResovleAll, RegistrationMaps.Create())
        {
            this.RegisterInstance(log);
        }

        protected ContainerBase(ILog log, int? maxDisposeParallelization, bool includeUnkeyedInResovleAll, ContainerBase parentScope)
            : this(log, maxDisposeParallelization, includeUnkeyedInResovleAll, parentScope._maps.Clone())
        {
            _maps.InitalizeScope(this);
        }

        private ContainerBase(ILog log, int? maxDisposeParallelization, bool includeUnkeyedInResovleAll, RegistrationMaps maps)
        {
            _maps = maps ?? throw new ArgumentNullException(nameof(maps));
            Log = log ?? throw new ArgumentNullException(nameof(log));
            MaxDisposeParallelization = maxDisposeParallelization;
            IncludeUnkeyedInResovleAll = includeUnkeyedInResovleAll;
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

        public abstract IContainer BeginScope();

        IResolver IResolver.BeginScope()
        {
            return BeginScope();
        }

        public object Resolve(Type type, string key = null)
        {
            var stack = new Stack<Type>(StackSize);
            return TryResolve(out object result, stack, type, key, true)
                ? result
                : throw NewNoRegistrationFoundException(stack, type);
        }

        public object Resolve(Stack<Type> stack, Type type, string key = null)
        {
            return TryResolve(out object result, stack, type, key, true)
                ? result
                : throw NewNoRegistrationFoundException(stack, type);
        }

        public bool TryResolve(out object result, Type type, string key = null)
        {
            var stack = new Stack<Type>(StackSize);
            return TryResolve(out result, stack, type, key, false);
        }

        public bool TryResolve(out object result, Stack<Type> stack, Type type, string key = null)
        {
            return TryResolve(out result, stack, type, key, false);
        }

        public IEnumerable<object> ResolveAll(Type type)
        {
            var stack = new Stack<Type>(StackSize);
            return ResolveAll(stack, type);
        }
        
        public IEnumerable<object> ResolveAll(Stack<Type> stack, Type type)
        {
            var lifetimeManagers = new List<ILifetimeManager>();
            using (_lock.UseReadLock())
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                if (_maps.RegistrationMap.TryGetValue(type, out Dictionary<string, ILifetimeManager> registrations))
                    foreach (var pair in registrations)
                        if (IncludeUnkeyedInResovleAll || pair.Key != string.Empty)
                            lifetimeManagers.Add(pair.Value);
            }

            var instances = new List<object>();
            using (new DisposablePush(stack, type))
            {
                foreach (var lifetimeManager in lifetimeManagers)
                    instances.Add(lifetimeManager.Resolve(this, stack));

                foreach (var resolutionHandler in ResolutionHandlers)
                    if (resolutionHandler.TryResolve(out object instance, this, stack, type, string.Empty, false))
                        instances.Add(instance);
            }

            return instances;
        }
        
        public void Register(ILifetimeManager lifetimeManager, Type fromType, string key = null)
        {
            if (key == null)
                key = string.Empty;

            using (_lock.UseWriteLock())
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                if (_maps.RegistrationMap.TryGetValue(fromType, out Dictionary<string, ILifetimeManager> registrations))
                {
                    if (IncludeUnkeyedInResovleAll && registrations.ContainsKey(key) && key == string.Empty)
                        key = $"_unkeyed_{Interlocked.Increment(ref _keySeed)}";

                    var clone = _maps.RegistrationMap[fromType] = new Dictionary<string, ILifetimeManager>(registrations);
                    clone[key] = lifetimeManager;
                }
                else
                    _maps.RegistrationMap[fromType] = new Dictionary<string, ILifetimeManager>
                    {
                        { key, lifetimeManager }
                    };

                _maps.TryAddKey(lifetimeManager, fromType);
            }

            if (Log.IsEnabled(LogLevel.Debug))
            {
                if (string.IsNullOrWhiteSpace(key))
                    Log.Debug("Type: {0} - {1}", fromType.Name, lifetimeManager.Description);
                else
                    Log.Debug("Type: {0} - Key: {1} - {2}", fromType.Name, key, lifetimeManager.Description);
            }
        }

        public object CreateInstance(Stack<Type> stack, Type type)
        {
            if (stack == null)
                throw new ArgumentNullException(nameof(stack));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            TryCreateInstance(out object result, stack, type, true);
            return result;
        }

        public bool TryCreateInstance(out object result, Stack<Type> stack, Type type)
        {
            if (stack == null)
                throw new ArgumentNullException(nameof(stack));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return TryCreateInstance(out result, stack, type, false);
        }

        public bool CanResolve(Stack<Type> stack, Type type, string key = null)
        {
            return TryResolve(out object result, stack, type, key, false, true);
        }

        public bool CanCreateInstance(Stack<Type> stack, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (stack == null)
                throw new ArgumentNullException(nameof(stack));

            return GetConstructor(stack, type) != null;
        }

        public bool TryResolveGenericType(out object result, Stack<Type> stack, Type genericType, Type[] genericArguments, string key = null)
        {
            if (genericType == null)
                throw new ArgumentNullException(nameof(genericType));

            if (genericArguments == null)
                throw new ArgumentNullException(nameof(genericArguments));

            if (stack == null)
                throw new ArgumentNullException(nameof(stack));

            if (key == null)
                key = string.Empty;

            ILifetimeManager lifetimeManager = null;

            using (_lock.UseReadLock())
            {
                if (_isDisposed)
                    throw new ObjectDisposedException(GetType().Name);

                if (_maps.RegistrationMap.TryGetValue(genericType, out Dictionary<string, ILifetimeManager> registrations))
                    foreach (var pair in registrations)
                        if (pair.Key == key)
                        {
                            lifetimeManager = pair.Value;
                            break;
                        }
            }

            if (lifetimeManager == null)
            {
                result = null;
                return false;
            }

            var type = genericType.MakeGenericType(genericArguments);
            var previousType = stack.Pop();
            if (previousType != type)
                throw new InvalidOperationException();
            
            var clone = lifetimeManager.CloneWithGenericArguments(genericArguments);
            Register(clone, type, key);
            result = Resolve(stack, type, key);
            
            stack.Push(previousType);
            return true;
        }

        private ConstructorInfo GetConstructor(Stack<Type> stack, Type type)
        {
            return ConstructorMap.GetOrAdd(type, t =>
            {
                return t
                    .GetTypeInfo()
                    .GetConstructors()
                    .OrderByDescending(c => c.GetParameterTypes().Count)
                    .FirstOrDefault(c =>
                    {
                        var parameterTypes = c.GetParameterTypes();
                        for (var i = 0; i < parameterTypes.Count; i++)
                            if (!CanResolve(stack, parameterTypes[i]))
                                return false;

                        return true;
                    });
            });
        }

        private bool TryResolve(out object result, Stack<Type> stack, Type type, string key, bool canThrow, bool returnNull = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            
            if (stack == null)
                throw new ArgumentNullException(nameof(stack));

            if (key == null)
                key = string.Empty;

            using (new DisposablePush(stack, type))
            {
                ILifetimeManager lifetimeManager = null;

                using (_lock.UseReadLock())
                {
                    if (_isDisposed)
                        throw new ObjectDisposedException(GetType().Name);

                    if (_maps.RegistrationMap.TryGetValue(type, out Dictionary<string, ILifetimeManager> registrations))
                        registrations.TryGetValue(key, out lifetimeManager);
                }

                if (lifetimeManager != null)
                {
                    result = returnNull ? null : lifetimeManager.Resolve(this, stack);
                    return true;
                }

                foreach (var handler in ResolutionHandlers)
                    if (handler.TryResolve(out result, this, stack, type, key, canThrow))
                        return true;
            }

            result = null;
            return false;
        }

        private bool TryCreateInstance(out object result, Stack<Type> stack, Type type, bool canThrow)
        {
            var constructor = GetConstructor(stack, type);
            if (constructor == null)
            {
                if (canThrow)
                    throw new InvalidOperationException();

                result = null;
                return false;
            }

            var parameterTypes = constructor.GetParameterTypes();
            var arguments = new object[parameterTypes.Count];
            for (var i = 0; i < parameterTypes.Count; i++)
            {
                var parameterType = parameterTypes[i];
                arguments[i] = Resolve(stack, parameterType);
            }

            try
            {
                result = constructor.EfficientInvoke(arguments);
                return true;
            }
            catch (Exception) when (!canThrow)
            {
                result = null;
                return false;
            }
        }

        private static InvalidOperationException NewNoRegistrationFoundException(Stack<Type> stack, Type type)
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
            public readonly Dictionary<Type, Dictionary<string, ILifetimeManager>> RegistrationMap;

            private List<Type> _scopedKeys;
            private List<Type> _disposableKeys;

            private RegistrationMaps(
                Dictionary<Type, Dictionary<string, ILifetimeManager>> registrationMap,
                List<Type> scopedKeys,
                List<Type> disposableKeys)
            {
                RegistrationMap = registrationMap;
                _scopedKeys = scopedKeys;
                _disposableKeys = disposableKeys;
            }

            public static RegistrationMaps Create()
            {
                return new RegistrationMaps(
                    new Dictionary<Type, Dictionary<string, ILifetimeManager>>(),
                    new List<Type>(),
                    new List<Type>());
            }

            public RegistrationMaps Clone()
            {
                var registrationMap = new Dictionary<Type, Dictionary<string, ILifetimeManager>>(RegistrationMap);

                return new RegistrationMaps(
                    registrationMap,
                    _scopedKeys,
                    _disposableKeys);
            }

            public void TryAddKey(ILifetimeManager lifetimeManager, Type type)
            {
                if (lifetimeManager.IsScoped && !_scopedKeys.Contains(type))
                {
                    _scopedKeys = _scopedKeys.ToList();
                    _scopedKeys.Add(type);
                }

                if (lifetimeManager.IsDisposable && !_disposableKeys.Contains(type))
                {
                    _disposableKeys = _disposableKeys.ToList();
                    _disposableKeys.Add(type);
                }
            }

            public void InitalizeScope(IContainer scope)
            {
                foreach(var type in _scopedKeys)
                {
                    var values = RegistrationMap[type];
                    var clones = RegistrationMap[type] = new Dictionary<string, ILifetimeManager>(values.Count);
                    foreach (var pair in values)
                        clones[pair.Key] = pair.Value.BeginScope(scope);
                }
            }

            public IReadOnlyList<ILifetimeManager> GetLifetimeManagersToDispose(IContainer scope)
            {
                // Creating this collection with foreach is 15% faster than linq.
                var lifetimeManagersToDispose = new List<ILifetimeManager>();

                foreach (var key in _disposableKeys)
                    foreach (var pair in RegistrationMap[key])
                        if (pair.Value.RequiresDispose(scope))
                            lifetimeManagersToDispose.Add(pair.Value);
                
                return lifetimeManagersToDispose;
            }
        }

        private struct DisposablePush : IDisposable
        {
            private readonly Stack<Type> _stack;
            private readonly Type _type;

            public DisposablePush(Stack<Type> stack, Type type)
            {
                if (stack.Contains(type))
                    throw new InvalidOperationException("Recursive resolution detected");

                _stack = stack;
                _type = type;

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