using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tact.Practices.ResolutionHandlers.Implementation
{
    public class GenericResolutionHandler : IResolutionHandler
    {
        public bool TryResolve(IContainer container, Type type, string key, Stack<Type> stack, bool canThrow, out object result)
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
            return container.TryResolveGenericType(genericType, argTypes, key, stack, out result);
        }
    }
}
