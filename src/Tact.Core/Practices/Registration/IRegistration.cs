using System;
using System.Collections.Generic;

namespace Tact.Practices.Registration
{
    public interface IRegistration
    {
        string Description { get; }

        IRegistration Clone(IContainer scope);

        object Resolve(Stack<Type> stack);

        void Dispose(IContainer scope);
    }
}
