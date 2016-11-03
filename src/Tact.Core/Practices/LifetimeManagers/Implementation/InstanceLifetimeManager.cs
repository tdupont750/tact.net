using System;
using System.Collections.Generic;

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

        public ILifetimeManager Clone(IContainer scope)
        {
            return this;
        }

        public object Resolve(Stack<Type> stack)
        {
            return _instance;
        }

        public void Dispose(IContainer scope)
        {
            if (!ReferenceEquals(_scope, scope))
                return;

            var disposable = _instance as IDisposable;
            disposable?.Dispose();
        }
    }
}