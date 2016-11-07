using System;

namespace Tact.Practices.LifetimeManagers
{
    public interface IRegisterAttribute
    {
        void Register(IContainer container, Type toType);
    }
}