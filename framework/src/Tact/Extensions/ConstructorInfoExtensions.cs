using System.Reflection;
using Tact.Reflection;

namespace Tact
{
    public static class ConstructorInfoExtensions
    {
        public static object EfficientInvoke(this ConstructorInfo constructor, params object[] args)
        {
            return EfficientInvoker.ForConstructor(constructor)(args);
        }
    }
}