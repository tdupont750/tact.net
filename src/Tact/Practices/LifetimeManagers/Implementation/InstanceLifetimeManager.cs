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
        private readonly string _typeName;

        public InstanceLifetimeManager(object instance, object scope)
        {
            _scope = scope;
            _instance = instance;
            _typeName = instance.GetType().Name;
        }

        public string Description => string.Concat("Instance: ", _typeName);

        public bool IsDisposable => _instance is IDisposable;

        public bool IsScoped => false;

        public ILifetimeManager BeginScope(IContainer scope)
        {
            throw new NotImplementedException();
        }

        public object Resolve(IContainer scope, Stack<Type> stack)
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