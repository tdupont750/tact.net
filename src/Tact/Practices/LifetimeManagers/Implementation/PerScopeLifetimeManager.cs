using System;

namespace Tact.Practices.LifetimeManagers.Implementation
{
    public class PerScopeLifetimeManager : SingletonLifetimeManager
    {
        private readonly Type _toType;
        private readonly Func<IResolver, object> _factory;

        public PerScopeLifetimeManager(Type toType, IContainer scope, Func<IResolver, object> factory = null)
            : base(toType, scope)
        {
            _toType = toType;
            _factory = factory;
        }

        public override string Description => $"PerScope: {_toType.Name}";

        public override ILifetimeManager BeginScope(IContainer scope)
        {
            return new PerScopeLifetimeManager(_toType, scope, _factory);
        }
    }
}
