using System;
using Microsoft.Extensions.Configuration;
using Tact.Practices;

namespace Tact.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterConfigurationAttribute : Attribute, IRegisterConfigurationAttribute
    {
        public void Register(IContainer container, IConfiguration configuration, Type type)
        {
            var instance = configuration.CreateAndValidate(type);
            container.RegisterInstance(type, instance);
        }
    }
}
