using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tact.Practices.ResolutionHandlers.Implementation
{
    public class UnregisteredResolutionHandler : IResolutionHandler
    {
        public bool TryGetService(
            IContainer container, 
            Type type, 
            Stack<Type> stack, 
            out object result)
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsClass)
            {
                result = null;
                return false;
            }

            result = container.CreateInstance(type, stack);
            return true;
        }
    }
}