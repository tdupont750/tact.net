using Demo.Rpc.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using Tact.Practices;
using Tact.Practices.LifetimeManagers;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Configuration;

namespace Tact.Rpc.Practices
{
    public class RpcServiceImplementationAttribute : RegisterSingletonAttribute, IRegisterConditionAttribute
    {
        private readonly string _serviceName;

        public RpcServiceImplementationAttribute(Type fromType)
            : base(fromType)
        {
            _serviceName = fromType.Name.GetRpcName();
        }

        public bool ShouldRegister(IContainer container, Type toType)
        {
            var configuration = container.Resolve<IConfiguration>();
            var config = configuration.CreateAndValidate<ServiceConfig>(Constants.ServiceConfigPathPrefix + "." + _serviceName, Constants.ServiceConfigDefaults);
            return config.IsEnabled;
        }
    }
}
