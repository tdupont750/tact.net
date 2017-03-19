using System;
using System.Collections.Generic;
using System.Linq;
using Tact.Practices;

namespace Tact
{
    public static class ResolverExtensions
    {
        public static T Resolve<T>(this IResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            var type = typeof(T);
            return (T) resolver.Resolve(type);
        }

        public static bool TryResolve<T>(this IResolver resolver, out T result)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            var type = typeof(T);
            object objResult;
            if (resolver.TryResolve(out objResult, type))
            {
                result = (T) objResult;
                return true;
            }

            result = default(T);
            return false;
        }

        public static T Resolve<T>(this IResolver resolver, string key)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            var type = typeof(T);
            return (T) resolver.Resolve(type, key);
        }

        public static bool TryResolve<T>(this IResolver resolver, string key, out T result)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            var type = typeof(T);
            object objResult;
            if (resolver.TryResolve(out objResult, type, key))
            {
                result = (T)objResult;
                return true;
            }

            result = default(T);
            return false;
        }

        public static IEnumerable<T> ResolveAll<T>(this IResolver resolver)
        {
            if (resolver == null)
                throw new ArgumentNullException(nameof(resolver));

            var type = typeof(T);
            return resolver.ResolveAll(type).Cast<T>();
        }
    }
}
