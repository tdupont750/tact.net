using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tact.Reflection;

namespace Tact
{
    public static class TypeExtensions
    {
        internal static readonly ConcurrentDictionary<MethodBase, IReadOnlyList<Type>> ParameterMap =
            new ConcurrentDictionary<MethodBase, IReadOnlyList<Type>>();
        
        public static EfficientInvoker GetMethodInvoker(this Type type, string methodName)
        {
            return EfficientInvoker.ForMethod(type, methodName);
        }

        public static EfficientInvoker GetPropertyInvoker(this Type type, string propertyName)
        {
            return EfficientInvoker.ForProperty(type, propertyName);
        }
    }
}