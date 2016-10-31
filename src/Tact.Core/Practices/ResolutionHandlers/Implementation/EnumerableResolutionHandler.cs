using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tact.Practices.ResolutionHandlers.Implementation
{
    public class EnumerableResolutionHandler : IResolutionHandler
    {
        private static readonly string EnumerablePrefix;
        private static readonly MethodInfo CreateEnumerableMethodInfo;

        static EnumerableResolutionHandler()
        {
            EnumerablePrefix = typeof(IEnumerable<>).FullName;

            CreateEnumerableMethodInfo = typeof(EnumerableResolutionHandler)
                .GetTypeInfo()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(m => m.Name == "CreateEnumerable" && m.IsGenericMethod);
        }

        public bool TryGetService(
            IContainer container, 
            Type type, 
            Stack<Type> stack, 
            out object result)
        {
            if (!type.FullName.StartsWith(EnumerablePrefix))
            {
                result = null;
                return false;
            }

            var innerType = type.GenericTypeArguments[0];
            var method = CreateEnumerableMethodInfo.MakeGenericMethod(innerType);
            result = method.Invoke(this, new object[] { container, stack });
            return true;
        }

        // ReSharper disable once UnusedMember.Local
        private IEnumerable<T> CreateEnumerable<T>(
            IContainer container, 
            Stack<Type> stack)
        {
            var type = typeof(T);
            return container.ResolveAll(type, stack).Cast<T>();
        }
    }
}