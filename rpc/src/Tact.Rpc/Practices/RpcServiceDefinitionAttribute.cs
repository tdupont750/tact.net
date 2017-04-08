using System;
using Tact.Practices;
using Tact.Practices.LifetimeManagers;
using Tact.Rpc.Services;
using Tact.Rpc.Services.Implementation;

namespace Tact.Rpc.Practices
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class RpcServiceDefinitionAttribute : Attribute, IRegisterAttribute
    {
        public void Register(IContainer container, Type toType)
        {
            var service = new RpcHandler(toType);
            container.RegisterInstance<IRpcHandler>(service, service.Name);
        }
    }
}
