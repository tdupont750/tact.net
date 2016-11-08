using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tact.Configuration;
using Tact.Configuration.Attributes;
using Tact.Diagnostics;
using Tact.Practices;
using Tact.Practices.LifetimeManagers;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Practices.LifetimeManagers.Implementation;

namespace Tact
{
    public static class ContainerExtensions
    {
        #region Initialize By Attribute

        public static void InitializeByAttribute(this IContainer container, params Assembly[] assemblies)
        {
            container.InitializeByAttribute<IInitializeAttribute>(assemblies);
        }

        public static void InitializeByAttribute(this IContainer container, params Type[] types)
        {
            container.InitializeByAttribute<IInitializeAttribute>(types);
        }

        public static void InitializeByAttribute<T>(this IContainer container, params Assembly[] assemblies)
            where T : IInitializeAttribute
        {
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                container.InitializeByAttribute<T>(types);
            }
        }

        public static void InitializeByAttribute<T>(this IContainer container, params Type[] types)
            where T : IInitializeAttribute
        {
            ILog logger;
            container.TryResolve(out logger);

            foreach (var type in types)
            {
                var attribute = type
                    .GetTypeInfo()
                    .GetCustomAttributes()
                    .OfType<T>()
                    .SingleOrDefault();

                if (attribute == null)
                    continue;
                
                logger?.Debug("Type: {0} - Attribute: {1}", type.Name, attribute.GetType().Name);
                attribute.Initialize(container);
            }
        }

        #endregion

        #region Configure By Attribute

        public static void ConfigureByAttribute(this IContainer container, IConfigurationFactory configurationFactory, params Assembly[] assemblies)
        {
            container.ConfigureByAttribute<IRegisterConfigurationAttribute>(configurationFactory, assemblies);
        }

        public static void ConfigureByAttribute(this IContainer container, IConfigurationFactory configurationFactory, params Type[] types)
        {
            container.ConfigureByAttribute<IRegisterConfigurationAttribute>(configurationFactory, types);
        }

        public static void ConfigureByAttribute<T>(this IContainer container, IConfigurationFactory configurationFactory, params Assembly[] assemblies)
            where T : IRegisterConfigurationAttribute
        {
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                container.ConfigureByAttribute<T>(configurationFactory, types);
            }
        }

        public static void ConfigureByAttribute<T>(this IContainer container, IConfigurationFactory configurationFactory, params Type[] types)
            where T : IRegisterConfigurationAttribute
        {
            ILog logger;
            container.TryResolve(out logger);

            foreach (var type in types)
            {
                var attribute = type
                    .GetTypeInfo()
                    .GetCustomAttributes()
                    .OfType<T>()
                    .SingleOrDefault();
                
                if (attribute == null)
                    continue;

                logger?.Debug("Type: {0} - Attribute: {1}", type.Name, attribute.GetType().Name);
                attribute.Register(container, configurationFactory, type);
            }
        }

        #endregion

        #region Register By Attribute

        public static void RegisterByAttribute(this IContainer container, params Assembly[] assemblies)
        {
            container.RegisterByAttribute<IRegisterAttribute, IRegisterConditionAttribute>(assemblies);
        }

        public static void RegisterByAttribute(this IContainer container, params Type[] types)
        {
            container.RegisterByAttribute<IRegisterAttribute, IRegisterConditionAttribute>(types);
        }

        public static void RegisterByAttribute<T>(this IContainer container, params Assembly[] assemblies)
            where T : IRegisterAttribute
        {
            container.RegisterByAttribute<T, IRegisterConditionAttribute>(assemblies);
        }

        public static void RegisterByAttribute<T>(this IContainer container, params Type[] types)
            where T : IRegisterAttribute
        {
            container.RegisterByAttribute<T, IRegisterConditionAttribute>(types);
        }

        public static void RegisterByAttribute<TRegister, TCondition>(this IContainer container, params Assembly[] assemblies)
            where TRegister : IRegisterAttribute
            where TCondition : IRegisterConditionAttribute
        {
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                container.RegisterByAttribute<TRegister, TCondition>(types);
            }
        }

        public static void RegisterByAttribute<TRegister, TCondition>(this IContainer container, params Type[] types)
            where TRegister : IRegisterAttribute
            where TCondition : IRegisterConditionAttribute
        {
            ILog logger;
            container.TryResolve(out logger);

            foreach (var type in types)
            {
                var attribute = type
                    .GetTypeInfo()
                    .GetCustomAttributes()
                    .OfType<TRegister>()
                    .SingleOrDefault();

                if (attribute == null)
                    continue;

                var conditions = type
                    .GetTypeInfo()
                    .GetCustomAttributes()
                    .OfType<TCondition>()
                    .ToArray();

                var shouldRegister = conditions.All(condition => condition.ShouldRegister(container, type));

                logger?.Debug("Type: {0} - Attribute: {1} - Conditions: {2} - ShouldRegister: {3}", type.Name,
                    attribute.GetType().Name, conditions.Length, shouldRegister);

                if (!shouldRegister)
                    continue;

                attribute.Register(container, type);
            }
        }

        #endregion

        #region Register Per Resolve

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
            var lifetimeManager = new PerResolveLifetimeManager(type, container, factory);
            container.Register(type, lifetimeManager);
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
            var lifetimeManager = new PerResolveLifetimeManager(type, container, factory);
            container.Register(type, key, lifetimeManager);
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
            var lifetimeManager = new PerResolveLifetimeManager(toType, container);
            container.Register(fromType, lifetimeManager);
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
            var lifetimeManager = new PerResolveLifetimeManager(toType, container);
            container.Register(fromType, key, lifetimeManager);
        }

        #endregion

        #region Register Per Scope

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
            var lifetimeManager = new PerScopeLifetimeManager(type, container, factory);
            container.Register(type, lifetimeManager);
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
            var lifetimeManager = new PerScopeLifetimeManager(type, container, factory);
            container.Register(type, key, lifetimeManager);
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
            var lifetimeManager = new PerScopeLifetimeManager(toType, container);
            container.Register(fromType, lifetimeManager);
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
            var lifetimeManager = new PerScopeLifetimeManager(toType, container);
            container.Register(fromType, key, lifetimeManager);
        }

        #endregion

        #region Register Instance
        
        public static void RegisterInstance<T>(this IContainer container, T value)
        {
            var type = typeof(T);
            container.RegisterInstance(type, value);
        }

        public static void RegisterInstance(this IContainer container, Type type, object value)
        {
            var lifetimeManager = new InstanceLifetimeManager(value, container);
            container.Register(type, lifetimeManager);
        }

        public static void RegisterInstance<T>(this IContainer container, string key, T value)
        {
            var type = typeof(T);
            container.RegisterInstance(type, key, value);
        }

        public static void RegisterInstance(this IContainer container, Type type, string key, object value)
        {
            var lifetimeManager = new InstanceLifetimeManager(value, container);
            container.Register(type, key, lifetimeManager);
        }

        #endregion

        #region Register Singleton

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
            var lifetimeManager = new SingletonLifetimeManager(type, container, factory);
            container.Register(type, lifetimeManager);
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
            var lifetimeManager = new SingletonLifetimeManager(type, container, factory);
            container.Register(type, key, lifetimeManager);
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
            var lifetimeManager = new SingletonLifetimeManager(toType, container);
            container.Register(fromType, lifetimeManager);
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
            var lifetimeManager = new SingletonLifetimeManager(toType, container);
            container.Register(fromType, key, lifetimeManager);
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
            var lifetimeManager = new TransientLifetimeManager(type, container, factory);
            container.Register(type, lifetimeManager);
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
            var lifetimeManager = new TransientLifetimeManager(type, container, factory);
            container.Register(type, key, lifetimeManager);
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
            var lifetimeManager = new TransientLifetimeManager(toType, container);
            container.Register(fromType, lifetimeManager);
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
            var lifetimeManager = new TransientLifetimeManager(toType, container);
            container.Register(fromType, key, lifetimeManager);
        }

        #endregion
    }
}