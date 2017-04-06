using System;
using Tact.Practices;
using Tact.Practices.LifetimeManagers;

namespace Tact.Rpc.Practices
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class RpcServiceAttribute : Attribute, IRegisterAttribute
    {
        public void Register(IContainer container, Type toType)
        {
            var serviceType = new RpcServiceInfo(toType);
            container.RegisterInstance(serviceType, serviceType.Name);
        }
    }
}
