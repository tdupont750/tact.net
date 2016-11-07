using System;
using Tact.ComponentModel;
using Tact.Practices;

namespace Tact.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterConfigurationAttribute : Attribute, IRegisterConfigurationAttribute
    {
        public void Register(IContainer container, IConfigurationFactory configurationFactory, Type type)
        {
            var instance = configurationFactory.CreateObject(type);
            ModelValidation.Validate(instance);
            container.RegisterSingleton(type, instance);
        }
    }
}
