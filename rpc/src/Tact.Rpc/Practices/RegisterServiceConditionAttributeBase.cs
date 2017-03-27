using Demo.Rpc.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using Tact.Practices;
using Tact.Practices.LifetimeManagers;
using Tact.Rpc.Configuration;

namespace Tact.Rpc.Practices
{
    public class RegisterServiceConditionAttribute : Attribute, IRegisterConditionAttribute
    {
        private readonly string _serviceName;

        public RegisterServiceConditionAttribute(string serviceName)
        {
            _serviceName = serviceName;
        }

        public bool ShouldRegister(IContainer container, Type toType)
        {
            var configuration = container.Resolve<IConfiguration>();
            var config = configuration.CreateAndValidate<ServiceConfig>(Constants.ConfigServicesPathPrefix + _serviceName, Constants.DefaultServicesConfigPath);
            return config.IsEnabled;
        }
    }
}
