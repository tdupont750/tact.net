using System;

namespace Tact.Practices.LifetimeManagers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterPerResolveAttribute : Attribute, IRegisterAttribute
    {
        private readonly Type _fromType;
        private readonly string _key;

        public RegisterPerResolveAttribute(Type fromType, string key)
            : this(fromType)
        {
            _key = key;
        }

        public RegisterPerResolveAttribute(Type fromType)
        {
            _fromType = fromType;
        }

        public void Register(IContainer container, Type toType)
        {
            if (string.IsNullOrWhiteSpace(_key))
                container.RegisterPerResolve(_fromType, toType);
            else
                container.RegisterPerResolve(_fromType, toType, _key);
        }
    }
}