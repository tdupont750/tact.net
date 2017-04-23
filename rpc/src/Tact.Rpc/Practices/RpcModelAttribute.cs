using System;
using Tact.Practices;
using Tact.Practices.LifetimeManagers;

namespace Tact.Rpc.Practices
{
    public class RpcModelAttribute : Attribute, IRegisterAttribute
    {
        public Type Type { get; private set; }

        public void Register(IContainer container, Type toType)
        {
            Type = toType;
            container.RegisterInstance(this, toType.Name);
        }
    }
}