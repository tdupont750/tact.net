using System;
using System.Collections.Generic;

namespace Tact.Practices.ResolutionHandlers.Implementation
{
    public class ThrowOnFailResolutionHandler : IResolutionHandler
    {
        public bool TryGetService(
            IContainer container, 
            Type type, 
            Stack<Type> stack,
            bool canThrow,
            out object result)
        {
            if (canThrow)
                throw new InvalidOperationException("No matching registrations found");

            result = null;
            return false;
        }
    }
}