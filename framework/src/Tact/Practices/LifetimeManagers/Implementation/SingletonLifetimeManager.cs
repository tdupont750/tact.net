using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Tact.Threading;

namespace Tact.Practices.LifetimeManagers.Implementation
{
    public class SingletonLifetimeManager : ILifetimeManager
    {
        private static readonly TypeInfo DisposableTypeInfo = typeof(IDisposable).GetTypeInfo();
        
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        protected readonly Type ToType;
        protected readonly IContainer Scope;
        protected readonly Func<IResolver, object> Factory;

        private volatile object _instance;

        public SingletonLifetimeManager(Type toType, IContainer scope, Func<IResolver, object> factory = null)
        {
            ToType = toType;
            Scope = scope;
            Factory = factory;
        }

        public virtual string Description => string.Concat("Singleton: ", ToType.Name);

        public virtual bool IsScoped => false;

        public bool IsDisposable => DisposableTypeInfo.IsAssignableFrom(ToType);

        public virtual ILifetimeManager CloneWithGenericArguments(Type[] genericArguments)
        {
            var newToType = ToType.GetGenericTypeDefinition().MakeGenericType(genericArguments);
            return new SingletonLifetimeManager(newToType, Scope, Factory);
        }

        public virtual ILifetimeManager BeginScope(IContainer scope)
        {
            throw new NotImplementedException();
        }

        public object Resolve(IContainer scope, Stack<Type> stack)
        {
            if (_instance == null)
                using (_lock.UseWriteLock())
                    if (_instance == null)
                        _instance = Factory?.Invoke(Scope) ?? Scope.CreateInstance(stack, ToType);

            return _instance;
        }

        public Task DisposeAsync(IContainer scope, CancellationToken cancelToken)
        {
            return RequiresDispose(scope) 
                ? Disposable.Async(_instance, cancelToken)
                : Task.CompletedTask;
        }

        public bool RequiresDispose(IContainer scope)
        {
            if (_instance == null)
                using (_lock.UseReadLock())
                    if (_instance == null)
                        return false;

            if (!ReferenceEquals(Scope, scope))
                return false;

            if (ReferenceEquals(_instance, scope))
                return false;

            return _instance is IDisposable;
        }
    }
}
