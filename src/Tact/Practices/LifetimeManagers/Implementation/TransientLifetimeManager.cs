using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tact.Practices.LifetimeManagers.Implementation
{
    public class TransientLifetimeManager : ILifetimeManager
    {
        private readonly Type _toType;
        private readonly IContainer _scope;
        private readonly Func<IResolver, object> _factory;

        public TransientLifetimeManager(Type toType, IContainer scope, Func<IResolver, object> factory = null)
        {
            _toType = toType;
            _scope = scope;
            _factory = factory;
        }

        public string Description => $"Transient: {_toType.Name}";

        public ILifetimeManager BeginScope(IContainer scope)
        {
            return new TransientLifetimeManager(_toType, scope, _factory);
        }

        public object Resolve(Stack<Type> stack)
        {
            return _factory?.Invoke(_scope) ?? _scope.CreateInstance(_toType, stack);
        }

        public Task DisposeAsync(IContainer scope, CancellationToken cancelToken)
        {
            // Nothing to do dispose
            return Task.CompletedTask;
        }

        public bool RequiresDispose(IContainer scope)
        {
            return false;
        }
    }
}
