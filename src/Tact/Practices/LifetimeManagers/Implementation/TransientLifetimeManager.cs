using System;
using System.Collections.Generic;
using System.Reflection;
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

        public ILifetimeManager CloneWithGenericArguments(Type[] genericArguments)
        {
            var newToType = _toType.GetGenericTypeDefinition().MakeGenericType(genericArguments);
            return new TransientLifetimeManager(newToType, _factory);
        }

        public ILifetimeManager BeginScope(IContainer scope)
        {
            throw new NotImplementedException();
        }

        public object Resolve(IContainer scope, Stack<Type> stack)
        {
            return _factory?.Invoke(scope) ?? scope.CreateInstance(stack, _toType);
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
