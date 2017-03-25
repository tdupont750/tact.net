using System;

namespace Tact.Practices.LifetimeManagers.Attributes
{
    public class RegisterProxyAttribute : Attribute, IRegisterAttribute
    {
        private readonly Type _fromType;
        private readonly string _fromKey;
        private readonly string _toKey;

        public RegisterProxyAttribute(Type fromType, string fromKey = null, string toKey = null)
        {
            _fromType = fromType;
            _fromKey = fromKey;
            _toKey = toKey;
        }

        public void Register(IContainer container, Type toType)
        {
            container.RegisterProxy(_fromType, toType, _fromKey, _toKey);
        }
    }
}