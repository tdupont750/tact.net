using System;
using System.Collections.Generic;

namespace Tact.Practices.Registration.Implementation
{
    public class TransientRegistration : IRegistration
    {
        private readonly Type _toType;
        private readonly IContainer _scope;
        private readonly Func<IResolver, object> _factory;

        public TransientRegistration(Type toType, IContainer scope, Func<IResolver, object> factory = null)
        {
            _toType = toType;
            _scope = scope;
            _factory = factory;
        }

        public string Description => $"Transient: {_toType.Name}";

        public IRegistration Clone(IContainer scope)
        {
            return new TransientRegistration(_toType, scope, _factory);
        }

        public object Resolve(Stack<Type> stack)
        {
            return _factory?.Invoke(_scope) ?? _scope.CreateInstance(_toType, stack);
        }

        public void Dispose(IContainer scope)
        {
            // Nothing to do dispose
        }
    }
}
