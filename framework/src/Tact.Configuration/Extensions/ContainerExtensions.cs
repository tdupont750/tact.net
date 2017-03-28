using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Tact.Configuration;
using Tact.Diagnostics;
using Tact.Practices;

namespace Tact
{
    public static class ContainerExtensions
    {
        public static IConfiguration BuildConfiguration(this IContainer container, Func<IConfiguration> func)
        {
            var config = func();
            container.RegisterInstance(config);
            return config;
        }

        public static IConfiguration BuildConfiguration(this IContainer container, Func<ConfigurationBuilder, IConfiguration> func)
        {
            var configBuilder = new ConfigurationBuilder();
            var config = func(configBuilder);
            container.RegisterInstance(config);
            return config;
        }

        public static IConfiguration BuildConfiguration(this IContainer container, Action<ConfigurationBuilder> action)
        {
            var configBuilder = new ConfigurationBuilder();
            action(configBuilder);
            var config = configBuilder.Build();
            container.RegisterInstance<IConfiguration>(config);
            return config;
        }

        public static void ConfigureByAttribute(this IContainer container, IConfiguration configuration, params Assembly[] assemblies)
        {
            container.ConfigureByAttribute<IRegisterConfigurationAttribute>(configuration, assemblies);
        }

        public static void ConfigureByAttribute(this IContainer container, IConfiguration configuration, params Type[] types)
        {
            container.ConfigureByAttribute<IRegisterConfigurationAttribute>(configuration, types);
        }

        public static void ConfigureByAttribute<T>(this IContainer container, IConfiguration configuration, params Assembly[] assemblies)
            where T : IRegisterConfigurationAttribute
        {
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                container.ConfigureByAttribute<T>(configuration, types);
            }
        }

        public static void ConfigureByAttribute<T>(this IContainer container, IConfiguration configuration, params Type[] types)
            where T : IRegisterConfigurationAttribute
        {
            if(container == null)
                throw new ArgumentNullException(nameof(container));

            container.TryResolve(out ILog logger);

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
                attribute.Register(container, configuration, type);
            }
        }
    }
}
