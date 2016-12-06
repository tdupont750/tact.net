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
                .Single(m => m.Name == "CreateLazy" && m.IsGenericMethod);
        }
        
        public bool TryResolve(
            IContainer container, 
            Type type, 
            Stack<Type> stack,
            bool canThrow,
            out object result)
        {
            if (!type.FullName.StartsWith(LazyPrefix))
            {
                result = null;
                return false;
            }

            var innerType = type.GenericTypeArguments[0];
            var method = CreateLazyMethodInfo.MakeGenericMethod(innerType);
            result = method.Invoke(this, new object[] { container });
            return true;
        }

        // ReSharper disable once UnusedMember.Local
        private Lazy<T> CreateLazy<T>(IContainer container)
        {
            var type = typeof(T);
            return new Lazy<T>(() => (T)container.Resolve(type));
        }
    }
}