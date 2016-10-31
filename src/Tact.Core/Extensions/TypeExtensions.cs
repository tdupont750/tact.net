using System;
using System.Reflection;

namespace Tact
{
    public static class TypeExtensions
    {
        private const string ClassRequired = "TTo must be a class";
        private const string ConstructorRequired = "There must be a single public constructor defined";

        public static ConstructorInfo ValidateTypeForInstanceCreation(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsClass)
                throw new ArgumentException(ClassRequired);

            var constuctors = typeInfo.GetConstructors();
            if (constuctors.Length != 1)
                throw new ArgumentException(ConstructorRequired);

            return constuctors[0];
        }
    }
}