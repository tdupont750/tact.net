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

        public static T Resolve<T>(this IResolver resolver, string key)
        {
            var type = typeof(T);
            return (T) resolver.Resolve(type, key);
        }

        public static IEnumerable<T> ResolveAll<T>(this IResolver resolver)
        {
            var type = typeof(T);
            return resolver.ResolveAll(type).Cast<T>();
        }
    }
}
