using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tact.Practices.ResolutionHandlers.Implementation
{
    public class LazyResolutionHandler : IResolutionHandler
    {
        private static readonly string LazyPrefix;
        private static readonly MethodInfo CreateLazyMethodInfo;

        static LazyResolutionHandler()
        {
            LazyPrefix = typeof(Lazy<>).FullName;

            CreateLazyMethodInfo = typeof(LazyResolutionHandler)
                .GetTypeInfo()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(m => m.Name == nameof(CreateLazy) && m.IsGenericMethod);
        }

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
            if (!type.FullName.StartsWith(LazyPrefix))
            {
                result = null;
                return false;
            }

            if (returnNull)
            {
                result = null;
                return true;
            }

            var innerType = type.GenericTypeArguments[0];
            var method = CreateLazyMethodInfo.MakeGenericMethod(innerType);
            result = method.Invoke(this, new object[] { container, key });
            return true;
        }

        // ReSharper disable once UnusedMember.Local
        private Lazy<T> CreateLazy<T>(IContainer container, string key)
        {
            var type = typeof(T);
            return new Lazy<T>(() => (T)container.Resolve(type, key));
        }
    }
}