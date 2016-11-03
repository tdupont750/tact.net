using System;
using System.Collections.Generic;

namespace Tact.Practices.LifetimeManagers
{
    public interface ILifetimeManager
    {
        string Description { get; }

        ILifetimeManager Clone(IContainer scope);

        object Resolve(Stack<Type> stack);

        void Dispose(IContainer scope);
    }
}
