using System;

namespace Tact.Practices.LifetimeManagers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterPerScopeAttribute : Attribute, IRegisterAttribute
    {
        private readonly Type _fromType;
        private readonly string _key;

        public RegisterPerScopeAttribute(Type fromType, string key)
            : this(fromType)
        {
            _key = key;
        }

        public RegisterPerScopeAttribute(Type fromType)
        {
            _fromType = fromType;
        }

        public void Register(IContainer container, Type toType)
        {
            if (string.IsNullOrWhiteSpace(_key))
                container.RegisterPerScope(_fromType, toType);
            else
                container.RegisterPerScope(_fromType, toType, _key);
        }
    }
}