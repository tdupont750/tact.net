using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tact.Practices.LifetimeManagers.Implementation
{
    public class ProxyLifetimeManager : ILifetimeManager
    {
        private readonly Func<IResolver, object> _factory;
        private readonly Type _toType;
        private readonly string _toKey;
        private readonly bool _hasKey;

        public ProxyLifetimeManager(Func<IResolver, object> factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public ProxyLifetimeManager(Type toType, string toKey = null)
        {
            _toType = toType;
            _toKey = toKey;
            _hasKey = !string.IsNullOrWhiteSpace(_toKey);
        }

        public string Description => _factory != null
            ? "Proxy: (Factory)"
            : _hasKey
                ? string.Concat("Proxy: ", _toType.Name, " - Proxy Key: ", _toKey)
                : string.Concat("Proxy: ", _toType.Name);

        public bool IsScoped => false;

        public bool IsDisposable => false;

        public ILifetimeManager CloneWithGenericArguments(Type[] genericArguments)
        {
            if (_factory != null)
                return this;

            var newToType = _toType.GetGenericTypeDefinition().MakeGenericType(genericArguments);
            return new ProxyLifetimeManager(newToType, _toKey);
        }

        public ILifetimeManager BeginScope(IContainer scope)
        {
            throw new NotImplementedException();
        }

        public object Resolve(IContainer scope, Stack<Type> stack)
        {
            return _factory != null
                ? _factory(scope)
                : scope.Resolve(stack, _toType, _toKey);
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