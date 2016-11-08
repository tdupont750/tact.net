using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tact.Practices.ResolutionHandlers.Implementation
{
    public class FuncResolutionHandler : IResolutionHandler
    {
        private static readonly string FuncPrefix;
        private static readonly MethodInfo CreateFuncMethodInfo;

        static FuncResolutionHandler()
        {
            FuncPrefix = typeof(Func<>).FullName;

            CreateFuncMethodInfo = typeof(FuncResolutionHandler)
                .GetTypeInfo()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(m => m.Name == "CreateFunc" && m.IsGenericMethod);
        }
        
        public bool TryGetService(
            IContainer container, 
            Type type, 
            Stack<Type> stack,
            bool canThrow,
            out object result)
        {
            if (!type.FullName.StartsWith(FuncPrefix))
            {
                result = null;
                return false;
            }

            var innerType = type.GenericTypeArguments[0];
            var method = CreateFuncMethodInfo.MakeGenericMethod(innerType);
            result = method.Invoke(this, new object[] { container });
            return true;
        }

        // ReSharper disable once UnusedMember.Local
        private Func<T> CreateFunc<T>(IContainer container)
        {
            var type = typeof(T);
            return () => (T)container.Resolve(type);
        }
    }
}