using System;

namespace Tact.Practices.LifetimeManagers
{
    public interface IRegisterConditionAttribute
    {
        bool ShouldRegister(IContainer container, Type toType);
    }
}