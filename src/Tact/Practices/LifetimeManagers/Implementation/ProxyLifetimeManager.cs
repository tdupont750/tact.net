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
        private readonly bool _hasKey;

        public string Description => _hasKey
            ? string.Concat("Proxy: ", _toType.Name, " - Proxy Key: ", _toKey)
            : string.Concat("Proxy: ", _toType.Name);

        public bool IsScoped => false;

        public bool IsDisposable => false;

        public ProxyLifetimeManager(Type toType, string toKey)
        {
            _toType = toType;
            _toKey = toKey;
            _hasKey = !string.IsNullOrWhiteSpace(_toKey);
        }

        public ILifetimeManager BeginScope(IContainer scope)
        {
            throw new NotImplementedException();
        }

        public object Resolve(IContainer scope, Stack<Type> stack)
        {
            return _hasKey 
                ? scope.Resolve(_toType, _toKey, stack) 
                : scope.Resolve(_toType, stack);
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