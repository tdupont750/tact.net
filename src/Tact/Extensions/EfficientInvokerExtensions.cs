using System;
using Tact.Reflection;

namespace Tact
{
    public static class EfficientInvokerExtensions
    {
        public static EfficientInvoker GetMethodInvoker(this Type type, string methodName)
        {
            return EfficientInvoker.GetForMethod(type, methodName);
        }

        public static EfficientInvoker GetPropertyInvoker(this Type type, string propertyName)
        {
            return EfficientInvoker.GetForProperty(type, propertyName);
        }

        public static EfficientInvoker GetInvoker(this Delegate del)
        {
            return EfficientInvoker.GetForDelegate(del);
        }
    }
}