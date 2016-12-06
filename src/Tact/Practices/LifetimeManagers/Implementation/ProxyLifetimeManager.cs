using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tact.Practices.LifetimeManagers.Implementation
{
    public class ProxyLifetimeManager : ILifetimeManager
    {
        private readonly Type _toType;
        private readonly string _toKey;
        private readonly IContainer _scope;
        private readonly bool _hasKey;

        public string Description => _hasKey
            ? $"Proxy: {_toType.Name} - Proxy Key: {_toKey}"
            : $"Proxy: {_toType.Name}";

        public ProxyLifetimeManager(Type toType, string toKey, IContainer scope)
        {
            _toType = toType;
            _toKey = toKey;
            _scope = scope;
            _hasKey = !string.IsNullOrWhiteSpace(_toKey);
        }

        public ILifetimeManager BeginScope(IContainer scope)
        {
            return new ProxyLifetimeManager(_toType, _toKey, scope);
        }

        public object Resolve(Stack<Type> stack)
        {
            return _hasKey 
                ? _scope.Resolve(_toType, _toKey, stack) 
                : _scope.Resolve(_toType, stack);
        }

        public Task DisposeAsync(IContainer scope, CancellationToken cancelToken)
        {
            // Nothing to do dispose
            return Task.CompletedTask;
        }
    }
}