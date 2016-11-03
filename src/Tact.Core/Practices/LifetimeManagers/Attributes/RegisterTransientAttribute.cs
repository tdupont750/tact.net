using System;

namespace Tact.Practices.LifetimeManagers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterTransientAttribute : Attribute, IRegisterAttribute
    {
        private readonly Type _fromType;
        private readonly string _key;

        public RegisterTransientAttribute(Type fromType, string key)
            : this(fromType)
        {
            _key = key;
        }

        public RegisterTransientAttribute(Type fromType)
        {
            _fromType = fromType;
        }

        public void Reigster(IContainer container, Type toType)
        {
            if (string.IsNullOrWhiteSpace(_key))
                container.RegisterTransient(_fromType, toType);
            else
                container.RegisterTransient(_fromType, toType, _key);
        }
    }
}