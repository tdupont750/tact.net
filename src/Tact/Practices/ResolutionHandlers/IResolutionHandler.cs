using System;
using System.Collections.Generic;

namespace Tact.Practices.ResolutionHandlers
{
    public interface IResolutionHandler
    {
        bool TryResolve(IContainer container, Type type, string key, Stack<Type> stack, bool canThrow, out object result);
    }
}