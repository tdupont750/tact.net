using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tact.Practices.ResolutionHandlers.Implementation
{
    public class GenericResolutionHandler : IResolutionHandler
    {
        public bool CanResolve(IContainer container, Stack<Type> stack, Type type, string key)
        {
            return TryResolve(
                out object result,
                container,
                stack,
                type,
                key,
                false,
                true);
        }

        public bool TryResolve(
            out object result,
            IContainer container,
            Stack<Type> stack,
            Type type,
            string key,
            bool canThrow)
        {
            return TryResolve(
                out result,
                container,
                stack,
                type,
                key,
                canThrow,
                false);
        }

        public bool TryResolve(
            out object result,
            IContainer container,
            Stack<Type> stack,
            Type type,
            string key,
            bool canThrow,
            bool returnNull)
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

            if (returnNull)
            {
                result = null;
                return container.CanResolveGenericType(stack, genericType, argTypes, key);
            }

            return container.TryResolveGenericType(out result, stack, genericType, argTypes, key);
        }
    }
}
