using System;
using Microsoft.Extensions.Configuration;
using Tact.Practices;

namespace Tact.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class RegisterConfigurationAttribute : Attribute, IRegisterConfigurationAttribute
    {
        private readonly string[] _configPaths;
        
        public RegisterConfigurationAttribute(params string[] configPaths)
        {
            _configPaths = configPaths;
        }

        public void Register(IContainer container, IConfiguration configuration, Type type)
        {
            var configPaths = _configPaths.Length == 0
                ? new[] { type.Name }
                : _configPaths;
            
            var instance = configuration.CreateAndValidate(type, configPaths);
            container.RegisterInstance(type, instance);
        }
    }
}
