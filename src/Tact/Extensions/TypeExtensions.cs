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
        private const string ClassRequired = "TTo must be a class";

        private const string ConstructorRequired = "There must be a single public constructor defined";

        private static readonly ConcurrentDictionary<Type, Tuple<Result, ConstructorInfo>> ConstructorMap =
            new ConcurrentDictionary<Type, Tuple<Result, ConstructorInfo>>();

        private static readonly ConcurrentDictionary<MethodBase, IReadOnlyList<Type>> ParameterMap =
            new ConcurrentDictionary<MethodBase, IReadOnlyList<Type>>();

        public static bool HasSingleCostructor(this Type type)
        {
            if (type == null)
                return false;

            var result = ConstructorMap.GetOrAdd(type, GetConstructorInfo);
            return result.Item1 == Result.Valid;
        }

        public static ConstructorInfo EnsureSingleCostructor(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var result = ConstructorMap.GetOrAdd(type, GetConstructorInfo);

            switch (result.Item1)
            {
                case Result.Valid:
                    return result.Item2;

                case Result.ClassRequired:
                    throw new InvalidOperationException(ClassRequired);

                case Result.ConstructorRequired:
                    throw new InvalidOperationException(ConstructorRequired);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static IReadOnlyList<Type> GetParameterTypes(this MethodBase method)
        {
            return ParameterMap.GetOrAdd(method, c => c.GetParameters().Select(p => p.ParameterType).ToArray());
        }

        public static object EfficientInvoke(this ConstructorInfo constructor, params object[] args)
        {
            return EfficientInvoker.ForConstructor(constructor)(args);
        }

        public static EfficientInvoker GetMethodInvoker(this Type type, string methodName)
        {
            return EfficientInvoker.ForMethod(type, methodName);
        }

        public static EfficientInvoker GetPropertyInvoker(this Type type, string propertyName)
        {
            return EfficientInvoker.ForProperty(type, propertyName);
        }

        private static Tuple<Result, ConstructorInfo> GetConstructorInfo(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsClass)
                return Tuple.Create(Result.ClassRequired, (ConstructorInfo)null);

            var constuctors = typeInfo.GetConstructors();
            if (constuctors.Length != 1)
                return Tuple.Create(Result.ConstructorRequired, (ConstructorInfo)null);

            return Tuple.Create(Result.Valid, constuctors[0]);
        }

        private enum Result
        {
            Valid,
            ClassRequired,
            ConstructorRequired
        }
    }
}