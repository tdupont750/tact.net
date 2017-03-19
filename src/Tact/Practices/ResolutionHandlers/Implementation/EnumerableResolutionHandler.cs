using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tact.Practices.ResolutionHandlers.Implementation
{
    public class EnumerableResolutionHandler : IResolutionHandler
    {
        // ReSharper disable InconsistentNaming
        private static readonly string IEnumerablePrefix;
        private static readonly string ICollectionPrefix;
        private static readonly string IListPrefix;
        private static readonly string ListPrefix;
        
        // ReSharper restore InconsistentNaming
        private static readonly MethodInfo CreateEnumerableMethodInfo;

        static EnumerableResolutionHandler()
        {
            IEnumerablePrefix = typeof(IEnumerable<>).FullName;
            ICollectionPrefix = typeof(ICollection<>).FullName;
            IListPrefix = typeof(IList<>).FullName;
            ListPrefix = typeof(List<>).FullName;

            CreateEnumerableMethodInfo = typeof(EnumerableResolutionHandler)
                .GetTypeInfo()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .Single(m => m.Name == nameof(CreateEnumerable) && m.IsGenericMethod);
        }

        private readonly bool _resolveEnumerable;
        private readonly bool _resolveCollection;
        private readonly bool _resolveList;

        public EnumerableResolutionHandler(bool resolveEnumerable, bool resolveCollection, bool resolveList)
        {
            _resolveEnumerable = resolveEnumerable;
            _resolveCollection = resolveCollection;
            _resolveList = resolveList;
        }

        public bool TryResolve(
            out object result,
            IContainer container,
            Stack<Type> stack,
            Type type,
            string key,
            bool canThrow)
        {
            if ((_resolveEnumerable && type.FullName.StartsWith(IEnumerablePrefix))
                || (_resolveCollection && type.FullName.StartsWith(ICollectionPrefix))
                || (_resolveList && type.FullName.StartsWith(IListPrefix))
                || (_resolveList && type.FullName.StartsWith(ListPrefix)))
            {
                var innerType = type.GenericTypeArguments[0];
                var method = CreateEnumerableMethodInfo.MakeGenericMethod(innerType);
                result = method.Invoke(this, new object[] {container, stack});
                return true;
            }

            result = null;
            return false;
        }
        
        // ReSharper disable once UnusedMember.Local
        private IEnumerable<T> CreateEnumerable<T>(
            IContainer container, 
            Stack<Type> stack)
        {
            var type = typeof(T);
            return container.ResolveAll(stack, type).Cast<T>().ToList();
        }
    }
}