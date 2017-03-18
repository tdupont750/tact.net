using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tact.Practices.ResolutionHandlers.Implementation
{
    public class UnregisteredResolutionHandler : IResolutionHandler
    {
        public bool TryResolve(
            IContainer container, 
            Type type, 
            string key,
            Stack<Type> stack,
            bool canThrow,
            out object result)
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsClass)
            {
                result = null;
                return false;
            }

            if (!canThrow && !type.HasSingleCostructor())
            {
                result = null;
                return false;
            }

            result = container.CreateInstance(type, stack);
            return true;
        }
    }
}