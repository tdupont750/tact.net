using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tact.Practices.LifetimeManagers.Implementation
{
    public class TransientLifetimeManager : ILifetimeManager
    {
        private readonly Type _toType;
        private readonly Func<IResolver, object> _factory;

        public TransientLifetimeManager(Type toType, Func<IResolver, object> factory = null)
        {
            _toType = toType;
            _factory = factory;
        }

        public string Description => string.Concat("Transient: ", _toType.Name);

        public bool IsScoped => false;

        public bool IsDisposable => false;

        public ILifetimeManager BeginScope(IContainer scope)
        {
            throw new NotImplementedException();
        }

        public object Resolve(IContainer scope, Stack<Type> stack)
        {
            return _factory?.Invoke(scope) ?? scope.CreateInstance(_toType, stack);
        }

        public Task DisposeAsync(IContainer scope, CancellationToken cancelToken)
        {
            throw new NotImplementedException();
        }

        public bool RequiresDispose(IContainer scope)
        {
            throw new NotImplementedException();
        }
    }
}
