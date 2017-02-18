using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tact.Threading;

namespace Tact.Practices.LifetimeManagers.Implementation
{
    public class InstanceLifetimeManager : ILifetimeManager
    {
        private readonly object _scope;
        private readonly object _instance;

        public InstanceLifetimeManager(object instance, object scope)
        {
            _scope = scope;
            _instance = instance;
        }

        public string Description => $"Instance: {_instance.GetType().Name}";

        public ILifetimeManager BeginScope(IContainer scope)
        {
            return this;
        }

        public object Resolve(Stack<Type> stack)
        {
            return _instance;
        }

        public Task DisposeAsync(IContainer scope, CancellationToken cancelToken)
        {
            return RequiresDispose(scope)
                ? Disposable.Async(_instance, cancelToken)
                : Task.CompletedTask;
        }

        public bool RequiresDispose(IContainer scope)
        {
            return ReferenceEquals(_scope, scope) 
                && _instance is IDisposable;
        }
    }
}