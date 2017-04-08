using Demo.Rpc.Configuration;
using System;
using Tact.Practices;
using Tact.Practices.LifetimeManagers;
using Tact.Rpc.Clients;

namespace Tact.Rpc.Practices
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RpcClientImplementationAttribute : Attribute, IRegisterAttribute, IRegisterConditionAttribute
    {
        private readonly Type _fromType;

        public RpcClientImplementationAttribute(Type fromType) =>
            _fromType = fromType;

        public void Register(IContainer container, Type toType) => 
            container.RegisterSingleton(_fromType, toType);

        public bool ShouldRegister(IContainer container, Type toType)
        {
            var serviceName = _fromType.Name.GetRpcName();
            if (container.TryResolve(serviceName, out IRpcClientConfig config))
                return config.IsEnabled;

            if (container.TryResolve(Constants.DefaultsConfigKey, out config))
                return config.IsEnabled;

            return false;
        }
    }
}
