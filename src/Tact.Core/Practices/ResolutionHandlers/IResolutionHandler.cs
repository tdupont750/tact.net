using System;
using System.Collections.Generic;

namespace Tact.Practices.ResolutionHandlers
{
    public interface IResolutionHandler
    {
        bool TryGetService(IContainer container, Type type, Stack<Type> stack, out object result);
    }
}