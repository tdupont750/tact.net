using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tact.Threading;

namespace Tact.Practices.LifetimeManagers.Implementation
{
    public class SingletonLifetimeManager : ILifetimeManager
    {
        private readonly object _lock = new object();
        private readonly Type _toType;
        private readonly IContainer _scope;
        private readonly Func<IResolver, object> _factory;

        private volatile object _instance;

        public SingletonLifetimeManager(Type toType, IContainer scope, Func<IResolver, object> factory = null)
        {
            _toType = toType;
            _scope = scope;
            _factory = factory;
        }

        public virtual string Description => $"Singleton: {_toType.Name}";

        public virtual ILifetimeManager BeginScope(IContainer scope)
        {
            return this;
        }

        public object Resolve(Stack<Type> stack)
        {
            if (_instance != null)
                return _instance;

            lock (_lock)
            {
                if (_instance != null)
                    return _instance;

                return _instance = _factory?.Invoke(_scope) ?? _scope.CreateInstance(_toType, stack);
            }
        }

        public Task DisposeAsync(IContainer scope, CancellationToken cancelToken)
        {
            return RequiresDispose(scope) 
                ? Disposable.Async(_instance, cancelToken)
                : Task.CompletedTask;
        }

        public bool RequiresDispose(IContainer scope)
        {
            if (_instance == null)
                lock (_lock)
                    if (_instance == null)
                        return false;

            if (!ReferenceEquals(_scope, scope))
                return false;

            if (ReferenceEquals(_instance, scope))
                return false;

            return _instance is IDisposable;
        }
    }
}
