using System;
using System.Collections.Generic;

namespace Tact.Practices.LifetimeManagers.Implementation
{
    public class PerResolveLifetimeManager : ILifetimeManager
    {
        private readonly Type _toType;
        private readonly IContainer _scope;
        private readonly Func<IResolver, object> _factory;
        private readonly List<Tuple<WeakReference<Stack<Type>>, WeakReference<object>>> _cache;

        public PerResolveLifetimeManager(Type toType, IContainer scope, Func<IResolver, object> factory = null)
        {
            _toType = toType;
            _scope = scope;
            _factory = factory;
            _cache = new List<Tuple<WeakReference<Stack<Type>>, WeakReference<object>>>();
        }

        public string Description => $"PerResolve: {_toType.Name}";

        public ILifetimeManager Clone(IContainer scope)
        {
            return new PerResolveLifetimeManager(_toType, scope, _factory);
        }

        public object Resolve(Stack<Type> stack)
        {
            lock (_cache)
            {
                for (var i = 0; i < _cache.Count; i++)
                {
                    Stack<Type> existingStack;
                    object existingObject;

                    var tuple = _cache[i];
                    if (tuple.Item1.TryGetTarget(out existingStack) && tuple.Item2.TryGetTarget(out existingObject))
                    {
                        if (ReferenceEquals(stack, existingStack))
                            return existingObject;
                    }
                    else
                    {
                        _cache.RemoveAt(i);
                        i--;
                    }
                }
            }

            var result = _factory?.Invoke(_scope) ?? _scope.CreateInstance(_toType, stack);

            lock (_cache)
            {
                var weakStack = new WeakReference<Stack<Type>>(stack);
                var weakObject = new WeakReference<object>(result);
                var tuple = Tuple.Create(weakStack, weakObject);
                _cache.Add(tuple);
            }

            return result;
        }

        public void Dispose(IContainer scope)
        {
            // Nothing to do dispose
        }
    }
}