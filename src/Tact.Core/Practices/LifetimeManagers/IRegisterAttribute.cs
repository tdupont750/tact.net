using System;

namespace Tact.Practices.LifetimeManagers
{
    public interface IRegisterAttribute
    {
        void Reigster(IContainer container, Type toType);
    }
}