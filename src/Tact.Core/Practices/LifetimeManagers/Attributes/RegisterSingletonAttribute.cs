using System;

namespace Tact.Practices.LifetimeManagers.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterSingletonAttribute : Attribute, IRegisterAttribute
    {
        private readonly Type _fromType;
        private readonly string _key;

        public RegisterSingletonAttribute(Type fromType, string key = null)
        {
            _fromType = fromType;
            _key = key;
        }
        
        public void Register(IContainer container, Type toType)
        {
            container.RegisterSingleton(_fromType, toType, _key);
        }
    }
}