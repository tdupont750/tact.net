using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using O = System.Object;

namespace Tact.Reflection
{
    public sealed class EfficientInvoker
    {
        private const string TooManyArgsMessage = "Invokes for more than 10 args are not yet implemented";
        
        private static readonly Type VoidType = typeof(void);

        private static readonly ConcurrentDictionary<Type, EfficientInvoker> TypeToWrapperMap
            = new ConcurrentDictionary<Type, EfficientInvoker>();

        private static readonly ConcurrentDictionary<MethodKey, EfficientInvoker> MethodToWrapperMap
            = new ConcurrentDictionary<MethodKey, EfficientInvoker>(MethodKeyComparer.Instance);

        private readonly Func<object, object[], object> _func;

        private EfficientInvoker(Func<object, object[], object> func)
        {
            _func = func;
        }

        public static EfficientInvoker GetForDelegate(Delegate del)
        {
            var type = del.GetType();
            return TypeToWrapperMap.GetOrAdd(type, t =>
            {
                var method = del.GetMethodInfo();
                var info = CreateMethodInfo(type, method, true);
                var wrapper = CreateWrapper(info);
                return new EfficientInvoker(wrapper);
            });
        }

        public static EfficientInvoker GetForMethod(Type type, string methodName)
        {
            var key = new MethodKey(type, methodName);
            return MethodToWrapperMap.GetOrAdd(key, k =>
            {
                var method = type.GetTypeInfo().GetMethod(methodName);
                var info = CreateMethodInfo(type, method, false);
                var wrapper = CreateWrapper(info);
                return new EfficientInvoker(wrapper);
            });
        }

        public static EfficientInvoker GetForProperty(Type type, string propertyName)
        {
            var key = new MethodKey(type, propertyName);
            return MethodToWrapperMap.GetOrAdd(key, k =>
            {
                var info = CreatePropertyInfo(type, propertyName);
                var wrapper = CreateWrapper(info);
                return new EfficientInvoker(wrapper);
            });
        }

        public object Invoke(object target, params object[] args)
        {
            return _func(target, args);
        }

        public async Task<object> InvokeAsync(object target, params object[] args)
        {
            var result = _func(target, args);
            var task = result as Task;
            if (task == null) return result;

            await task.ConfigureAwait(false);
            return task.GetResult();
        }

        private static WrapperInfo CreateMethodInfo(Type type, MethodInfo method, bool isDelegate)
        {
            var parameters = method.GetParameters();
            if (parameters.Length > 10) throw new NotImplementedException(TooManyArgsMessage);

            var castExps = new Expression[parameters.Length];
            var parameterExps = new ParameterExpression[parameters.Length + 1];
            parameterExps[0] = Expression.Parameter(typeof(object), "target");

            for (var i = 0; i < parameters.Length; i++)
            {
                parameterExps[i + 1] = Expression.Parameter(typeof(object), "arg" + i);
                castExps[i] = Expression.Convert(parameterExps[i + 1], parameters[i].ParameterType);
            }

            var targetCastExp = Expression.Convert(parameterExps[0], type);
            var callExp = isDelegate
                ? (Expression)Expression.Invoke(targetCastExp, castExps)
                : Expression.Call(targetCastExp, method, castExps);

            var hasReturnValue = method.ReturnType != VoidType;
            var callCastExp = hasReturnValue
                ? Expression.Convert(callExp, typeof(object))
                : callExp;

            var lambdaExp = Expression.Lambda(callCastExp, parameterExps);
            var lambda = lambdaExp.Compile();
            return new WrapperInfo(lambda, parameters.Length, hasReturnValue);
        }

        private static WrapperInfo CreatePropertyInfo(Type type, string propertyName)
        {
            var property = type.GetRuntimeProperty(propertyName);
            var argExp = Expression.Parameter(typeof(object), "target");
            var castArgExp = Expression.Convert(argExp, type);
            var propExp = Expression.Property(castArgExp, property);
            var castPropExp = Expression.Convert(propExp, typeof(object));
            var lambdaExp = Expression.Lambda(castPropExp, argExp);
            var lambda = lambdaExp.Compile();
            return new WrapperInfo(lambda, 0, true);
        }

        private static Func<object, object[], object> CreateWrapper(WrapperInfo info)
        {
            if (info.HasReturnValue)
            {
                switch (info.ArgumentCount)
                {
                    case 0: return (target, args) => ((Func<O, O>)info.Wrapper)(target);
                    case 1: return (target, args) => ((Func<O, O, O>)info.Wrapper)(target, args[0]);
                    case 2: return (target, args) => ((Func<O, O, O, O>)info.Wrapper)(target, args[0], args[1]);
                    case 3: return (target, args) => ((Func<O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2]);
                    case 4: return (target, args) => ((Func<O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3]);
                    case 5: return (target, args) => ((Func<O, O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3], args[4]);
                    case 6: return (target, args) => ((Func<O, O, O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3], args[4], args[5]);
                    case 7: return (target, args) => ((Func<O, O, O, O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3], args[4], args[5], args[6]);
                    case 8: return (target, args) => ((Func<O, O, O, O, O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]);
                    case 9: return (target, args) => ((Func<O, O, O, O, O, O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]);
                    case 10: return (target, args) => ((Func<O, O, O, O, O, O, O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]);
                    default: throw new NotImplementedException(TooManyArgsMessage);
                }
            }

            switch (info.ArgumentCount)
            {
                case 0: return (target, args) => { ((Action<O>)info.Wrapper)(target); return null; };
                case 1: return (target, args) => { ((Action<O, O>)info.Wrapper)(target, args[0]); return null; };
                case 2: return (target, args) => { ((Action<O, O, O>)info.Wrapper)(target, args[0], args[1]); return null; };
                case 3: return (target, args) => { ((Action<O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2]); return null; };
                case 4: return (target, args) => { ((Action<O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3]); return null; };
                case 5: return (target, args) => { ((Action<O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3], args[4]); return null; };
                case 6: return (target, args) => { ((Action<O, O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3], args[4], args[5]); return null; };
                case 7: return (target, args) => { ((Action<O, O, O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3], args[4], args[5], args[6]); return null; };
                case 8: return (target, args) => { ((Action<O, O, O, O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]); return null; };
                case 9: return (target, args) => { ((Action<O, O, O, O, O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]); return null; };
                case 10: return (target, args) => { ((Action<O, O, O, O, O, O, O, O, O, O, O>)info.Wrapper)(target, args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9]); return null; };
                default: throw new NotImplementedException(TooManyArgsMessage);
            }
        }

        private class MethodKeyComparer : IEqualityComparer<MethodKey>
        {
            private static readonly StringComparer Comparer = StringComparer.Ordinal;

            public static readonly MethodKeyComparer Instance = new MethodKeyComparer();

            public bool Equals(MethodKey x, MethodKey y)
            {
                return x.Type == y.Type
                       && Comparer.Equals(x.MethodName, y.MethodName);
            }

            public int GetHashCode(MethodKey key)
            {
                var typeCode = key.Type.GetHashCode();
                var methodCode = key.MethodName.GetHashCode();
                return CombineHashCodes(typeCode, methodCode);
            }

            // From System.Web.Util.HashCodeCombiner
            private static int CombineHashCodes(int h1, int h2)
            {
                return ((h1 << 5) + h1) ^ h2;
            }
        }

        private struct MethodKey
        {
            public MethodKey(Type type, string methodName)
            {
                Type = type;
                MethodName = methodName;
            }

            public readonly Type Type;
            public readonly string MethodName;
        }

        private struct WrapperInfo
        {
            public WrapperInfo(object wrapper, int argumentCount, bool hasReturnValue)
            {
                Wrapper = wrapper;
                ArgumentCount = argumentCount;
                HasReturnValue = hasReturnValue;
            }

            public readonly object Wrapper;
            public readonly int ArgumentCount;
            public readonly bool HasReturnValue;
        }
    }
}