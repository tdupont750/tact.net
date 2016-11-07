using System;

namespace Tact.Practices.LifetimeManagers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterSingletonAttribute : Attribute, IRegisterAttribute
    {
        private readonly Type _fromType;
        private readonly string _key;

        public RegisterSingletonAttribute(Type fromType, string key)
            : this(fromType)
        {
            _key = key;
        }

        public RegisterSingletonAttribute(Type fromType)
        {
            _fromType = fromType;
        }

        public void Register(IContainer container, Type toType)
        {
            if (string.IsNullOrWhiteSpace(_key))
                container.RegisterSingleton(_fromType, toType);
            else
                container.RegisterSingleton(_fromType, toType, _key);
        }
    }
}