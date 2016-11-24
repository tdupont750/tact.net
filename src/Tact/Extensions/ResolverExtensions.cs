using System.Collections.Generic;
using System.Linq;
using Tact.Practices;

namespace Tact
{
    public static class ResolverExtensions
    {
        public static T Resolve<T>(this IResolver resolver)
        {
            var type = typeof(T);
            return (T) resolver.Resolve(type);
        }

        public static bool TryResolve<T>(this IResolver resolver, out T result)
        {
            var type = typeof(T);
            object objResult;
            if (resolver.TryResolve(type, out objResult))
            {
                result = (T) objResult;
                return true;
            }

            result = default(T);
            return false;
        }

        public static T Resolve<T>(this IResolver resolver, string key)
        {
            var type = typeof(T);
            return (T) resolver.Resolve(type, key);
        }

        public static bool TryResolve<T>(this IResolver resolver, string key, out T result)
        {
            var type = typeof(T);
            object objResult;
            if (resolver.TryResolve(type, key, out objResult))
            {
                result = (T)objResult;
                return true;
            }

            result = default(T);
            return false;
        }

        public static IEnumerable<T> ResolveAll<T>(this IResolver resolver)
        {
            var type = typeof(T);
            return resolver.ResolveAll(type).Cast<T>();
        }
    }
}
