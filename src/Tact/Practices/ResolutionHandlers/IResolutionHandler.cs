using System;
using System.Collections.Generic;

namespace Tact.Practices.ResolutionHandlers
{
    public interface IResolutionHandler
    {
        bool CanResolve(IContainer container, Stack<Type> stack, Type type, string key);

        bool TryResolve(out object result, IContainer container, Stack<Type> stack, Type type, string key, bool canThrow);
    }
}