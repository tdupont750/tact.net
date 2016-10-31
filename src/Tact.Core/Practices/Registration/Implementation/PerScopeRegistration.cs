using System;

namespace Tact.Practices.Registration.Implementation
{
    public class PerScopeRegistration : SingletonRegistration
    {
        private readonly Type _toType;
        private readonly Func<IResolver, object> _factory;

        public PerScopeRegistration(Type toType, IContainer scope, Func<IResolver, object> factory = null)
            : base(toType, scope)
        {
            _toType = toType;
            _factory = factory;
        }

        public string Description => $"PerScope: {_toType.Name}";

        public override IRegistration Clone(IContainer scope)
        {
            return new PerScopeRegistration(_toType, scope, _factory);
        }
    }
}
