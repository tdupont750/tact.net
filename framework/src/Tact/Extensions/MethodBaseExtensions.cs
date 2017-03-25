using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Tact
{
    public static class MethodBaseExtensions
    {
        public static IReadOnlyList<Type> GetParameterTypes(this MethodBase method)
        {
            return TypeExtensions.ParameterMap.GetOrAdd(method, c => c.GetParameters().Select(p => p.ParameterType).ToArray());
        }
    }
}