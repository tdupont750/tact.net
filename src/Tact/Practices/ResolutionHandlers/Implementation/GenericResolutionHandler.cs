using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tact.Practices.ResolutionHandlers.Implementation
{
    public class GenericResolutionHandler : IResolutionHandler
    {
        public bool TryResolve(
            out object result,
            IContainer container,
            Stack<Type> stack,
            Type type,
            string key,
            bool canThrow)
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsGenericType)
            {
                result = null;
                return false;
            }

            var argTypes = typeInfo.GetGenericArguments();
            if (argTypes.Length == 0)
            {
                result = null;
                return false;
            }
            
            var genericType = typeInfo.GetGenericTypeDefinition();
            return container.TryResolveGenericType(out result, stack, genericType, argTypes, key);
        }
    }
}
