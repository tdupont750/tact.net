using System;
using Tact.Practices;
using Tact.Practices.Registration.Implementation;

namespace Tact
{
    public static class ContainerExtensions
    {
        #region Register PerResolve

        public static void RegisterPerResolve<T>(this IContainer container)
            where T : class
        {
            var type = typeof(T);
            container.RegisterPerResolve(type);
        }

        public static void RegisterPerResolve(this IContainer container, Type type)
        {
            container.RegisterPerResolve(type, type);
        }

        public static void RegisterPerResolve<T>(this IContainer container, Func<IResolver, T> factory)
            where T : class
        {
            var type = typeof(T);
            container.RegisterPerResolve(type, factory);
        }

        public static void RegisterPerResolve(this IContainer container, Type type, Func<IResolver, object> factory)
        {
            var registration = new PerResolveRegistration(type, container, factory);
            container.Register(type, registration);
        }

        public static void RegisterPerResolve<T>(this IContainer container, string key)
            where T : class
        {
            var type = typeof(T);
            container.RegisterPerResolve(type, key);
        }

        public static void RegisterPerResolve(this IContainer container, Type type, string key)
        {
            container.RegisterPerResolve(type, type, key);
        }

        public static void RegisterPerResolve<T>(this IContainer container, string key, Func<IResolver, T> factory)
            where T : class
        {
            var type = typeof(T);
            container.RegisterPerResolve(type, key, factory);
        }

        public static void RegisterPerResolve(this IContainer container, Type type, string key, Func<IResolver, object> factory)
        {
            var registration = new PerResolveRegistration(type, container, factory);
            container.Register(type, key, registration);
        }

        public static void RegisterPerResolve<TFrom, TTo>(this IContainer container)
            where TTo : class, TFrom
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            container.RegisterPerResolve(fromType, toType);
        }

        public static void RegisterPerResolve(this IContainer container, Type fromType, Type toType)
        {
            toType.EnsureSingleCostructor();
            var registration = new PerResolveRegistration(toType, container);
            container.Register(fromType, registration);
        }

        public static void RegisterPerResolve<TFrom, TTo>(this IContainer container, string key)
            where TTo : class, TFrom
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            container.RegisterPerResolve(fromType, toType, key);
        }

        public static void RegisterPerResolve(this IContainer container, Type fromType, Type toType, string key)
        {
            toType.EnsureSingleCostructor();
            var registration = new PerResolveRegistration(toType, container);
            container.Register(fromType, key, registration);
        }

        #endregion

        #region Register PerScope

        public static void RegisterPerScope<T>(this IContainer container)
            where T : class
        {
            var type = typeof(T);
            container.RegisterPerScope(type);
        }

        public static void RegisterPerScope(this IContainer container, Type type)
        {
            container.RegisterPerScope(type, type);
        }

        public static void RegisterPerScope<T>(this IContainer container, Func<IResolver, T> factory)
            where T : class
        {
            var type = typeof(T);
            container.RegisterPerScope(type, factory);
        }

        public static void RegisterPerScope(this IContainer container, Type type, Func<IResolver, object> factory)
        {
            var registration = new PerScopeRegistration(type, container, factory);
            container.Register(type, registration);
        }

        public static void RegisterPerScope<T>(this IContainer container, string key)
            where T : class
        {
            var type = typeof(T);
            container.RegisterPerScope(type, key);
        }

        public static void RegisterPerScope(this IContainer container, Type type, string key)
        {
            container.RegisterPerScope(type, type, key);
        }

        public static void RegisterPerScope<T>(this IContainer container, string key, Func<IResolver, T> factory)
            where T : class
        {
            var type = typeof(T);
            container.RegisterPerScope(type, key, factory);
        }

        public static void RegisterPerScope(this IContainer container, Type type, string key, Func<IResolver, object> factory)
        {
            var registration = new PerScopeRegistration(type, container, factory);
            container.Register(type, key, registration);
        }

        public static void RegisterPerScope<TFrom, TTo>(this IContainer container)
            where TTo : class, TFrom
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            container.RegisterPerScope(fromType, toType);
        }

        public static void RegisterPerScope(this IContainer container, Type fromType, Type toType)
        {
            toType.EnsureSingleCostructor();
            var registration = new PerScopeRegistration(toType, container);
            container.Register(fromType, registration);
        }

        public static void RegisterPerScope<TFrom, TTo>(this IContainer container, string key)
            where TTo : class, TFrom
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            container.RegisterPerScope(fromType, toType, key);
        }

        public static void RegisterPerScope(this IContainer container, Type fromType, Type toType, string key)
        {
            toType.EnsureSingleCostructor();
            var registration = new PerScopeRegistration(toType, container);
            container.Register(fromType, key, registration);
        }

        #endregion

        #region Register Singleton

        public static void RegisterSingleton<T>(this IContainer container, T value)
        {
            var type = typeof(T);
            container.RegisterSingleton(type, value);
        }

        public static void RegisterSingleton(this IContainer container, Type type, object value)
        {
            var registration = new InstanceRegistration(value, container);
            container.Register(type, registration);
        }

        public static void RegisterSingleton<T>(this IContainer container, string key, T value)
        {
            var type = typeof(T);
            container.RegisterSingleton(type, key, value);
        }

        public static void RegisterSingleton(this IContainer container, Type type, string key, object value)
        {
            var registration = new InstanceRegistration(value, container);
            container.Register(type, key, registration);
        }

        public static void RegisterSingleton<T>(this IContainer container)
            where T : class
        {
            var type = typeof(T);
            container.RegisterSingleton(type);
        }

        public static void RegisterSingleton(this IContainer container, Type type)
        {
            container.RegisterSingleton(type, type);
        }

        public static void RegisterSingleton<T>(this IContainer container, Func<IResolver, T> factory)
            where T : class
        {
            var type = typeof(T);
            container.RegisterSingleton(type, factory);
        }

        public static void RegisterSingleton(this IContainer container, Type type, Func<IResolver, object> factory)
        {
            var registration = new SingletonRegistration(type, container, factory);
            container.Register(type, registration);
        }

        public static void RegisterSingleton<T>(this IContainer container, string key)
            where T : class
        {
            var type = typeof(T);
            container.RegisterSingleton(type, key);
        }

        public static void RegisterSingleton(this IContainer container, Type type, string key)
        {
            container.RegisterSingleton(type, type, key);
        }

        public static void RegisterSingleton<T>(this IContainer container, string key, Func<IResolver, T> factory)
            where T : class
        {
            var type = typeof(T);
            container.RegisterSingleton(type, key, factory);
        }

        public static void RegisterSingleton(this IContainer container, Type type, string key, Func<IResolver, object> factory)
        {
            var registration = new SingletonRegistration(type, container, factory);
            container.Register(type, key, registration);
        }

        public static void RegisterSingleton<TFrom, TTo>(this IContainer container)
            where TTo : class, TFrom
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            container.RegisterSingleton(fromType, toType);
        }

        public static void RegisterSingleton(this IContainer container, Type fromType, Type toType)
        {
            toType.EnsureSingleCostructor();
            var registration = new SingletonRegistration(toType, container);
            container.Register(fromType, registration);
        }

        public static void RegisterSingleton<TFrom, TTo>(this IContainer container, string key)
            where TTo : class, TFrom
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            container.RegisterSingleton(fromType, toType, key);
        }

        public static void RegisterSingleton(this IContainer container, Type fromType, Type toType, string key)
        {
            toType.EnsureSingleCostructor();
            var registration = new SingletonRegistration(toType, container);
            container.Register(fromType, key, registration);
        }

        #endregion

        #region Register Transient

        public static void RegisterTransient<T>(this IContainer container)
            where T : class
        {
            var type = typeof(T);
            container.RegisterTransient(type);
        }

        public static void RegisterTransient(this IContainer container, Type type)
        {
            container.RegisterTransient(type, type);
        }

        public static void RegisterTransient<T>(this IContainer container, Func<IResolver, T> factory)
            where T : class
        {
            var type = typeof(T);
            container.RegisterTransient(type, factory);
        }

        public static void RegisterTransient(this IContainer container, Type type, Func<IResolver, object> factory)
        {
            var registration = new TransientRegistration(type, container, factory);
            container.Register(type, registration);
        }

        public static void RegisterTransient<T>(this IContainer container, string key)
            where T : class
        {
            var type = typeof(T);
            container.RegisterTransient(type, key);
        }

        public static void RegisterTransient(this IContainer container, Type type, string key)
        {
            container.RegisterTransient(type, type, key);
        }

        public static void RegisterTransient<T>(this IContainer container, string key, Func<IResolver, T> factory)
            where T : class
        {
            var type = typeof(T);
            container.RegisterTransient(type, key, factory);
        }

        public static void RegisterTransient(this IContainer container, Type type, string key, Func<IResolver, object> factory)
        {
            var registration = new TransientRegistration(type, container, factory);
            container.Register(type, key, registration);
        }

        public static void RegisterTransient<TFrom, TTo>(this IContainer container)
            where TTo : class, TFrom
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            container.RegisterTransient(fromType, toType);
        }

        public static void RegisterTransient(this IContainer container, Type fromType, Type toType)
        {
            toType.EnsureSingleCostructor();
            var registration = new TransientRegistration(toType, container);
            container.Register(fromType, registration);
        }

        public static void RegisterTransient<TFrom, TTo>(this IContainer container, string key)
            where TTo : class, TFrom
        {
            var fromType = typeof(TFrom);
            var toType = typeof(TTo);
            container.RegisterTransient(fromType, toType, key);
        }

        public static void RegisterTransient(this IContainer container, Type fromType, Type toType, string key)
        {
            toType.EnsureSingleCostructor();
            var registration = new TransientRegistration(toType, container);
            container.Register(fromType, key, registration);
        }

        #endregion
    }
}