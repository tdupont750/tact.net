using System;

namespace Tact.Practices.LifetimeManagers.Implementation
{
    public class PerScopeLifetimeManager : SingletonLifetimeManager
    {
        public PerScopeLifetimeManager(Type toType, IContainer scope, Func<IResolver, object> factory = null)
            : base(toType, scope, factory)
        {
        }

        public override string Description => string.Concat("PerScope: ", ToType.Name);

        public override bool IsScoped => true;

        public override ILifetimeManager CloneWithGenericArguments(Type[] genericArguments)
        {
            var newToType = ToType.GetGenericTypeDefinition().MakeGenericType(genericArguments);
            return new PerScopeLifetimeManager(newToType, Scope, Factory);
        }

        public override ILifetimeManager BeginScope(IContainer scope)
        {
            return new PerScopeLifetimeManager(ToType, scope, Factory);
        }
    }
}
