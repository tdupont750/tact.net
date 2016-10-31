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
            out object result)
        {
            throw new InvalidOperationException("No matching registrations found");
        }
    }
}