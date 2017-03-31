using Demo.Rpc.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using Tact.Practices;
using Tact.Practices.LifetimeManagers;
using Tact.Rpc.Configuration;

namespace Tact.Rpc.Practices
{
    class RegisterWebSocketClientConditionAttribute : Attribute, IRegisterConditionAttribute
    {
        private readonly string _serviceName;

        public RegisterWebSocketClientConditionAttribute(string serviceName)
        {
            _serviceName = serviceName;
        }

        public bool ShouldRegister(IContainer container, Type toType)
        {
            var configuration = container.Resolve<IConfiguration>();
            var config = configuration.CreateAndValidate<WebSocketClientConfig>(Constants.ConfigServicesPathPrefix + _serviceName, Constants.DefaultServicesConfigPath);

            if (!config.IsEnabled)
                return false;

            container.RegisterInstance(config, _serviceName);
            return true;
        }
    }
}
