using System;
using System.Collections.Generic;
using System.Reflection;

namespace Tact.Practices.ResolutionHandlers.Implementation
{
    public class UnregisteredResolutionHandler : IResolutionHandler
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
            if (!typeInfo.IsClass)
            {
                result = null;
                return false;
            }

            if (returnNull)
            {
                result = null;
                return container.CanCreateInstance(stack, type);
            }

            if (canThrow)
            {
                result = container.CreateInstance(stack, type);
                return true;
            }

            return container.TryCreateInstance(out result, stack, type);
        }
    }
}