﻿using System;
using System.Collections.Generic;

namespace Tact.Practices.Registration.Implementation
{
    public class SingletonRegistration : IRegistration
    {
        private readonly Type _toType;
        private readonly object _scope;
        private readonly Lazy<object> _instance;

        public SingletonRegistration(Type toType, IContainer scope, Func<IResolver, object> factory = null)
        {
            _toType = toType;
            _scope = scope;
            _instance = new Lazy<object>(() => factory?.Invoke(scope) ?? scope.CreateInstance(toType, new Stack<Type>()));
        }

        public string Description => $"Singleton: {_toType.Name}";

        public virtual IRegistration Clone(IContainer scope)
        {
            return this;
        }

        public object Resolve(Stack<Type> stack)
        {
            return _instance.Value;
        }

        public void Dispose(IContainer scope)
        {
            if (!_instance.IsValueCreated)
                return;

            if (!ReferenceEquals(_scope, scope))
                return;

            var instance = _instance.Value;
            if (ReferenceEquals(instance, scope))
                return;

            var disposable = _instance.Value as IDisposable;
            disposable?.Dispose();
        }
    }
}
