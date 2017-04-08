using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace Tact
{
    public static class ConfigurationExtensions
    {
        public static Assembly[] LoadAssembliesFromConfig(this IConfiguration config)
        {
            return config.LoadAssembliesFromConfig("Tact.Assemblies");
        }

        public static Assembly[] LoadAssembliesFromConfig(this IConfiguration config, params string[] configPaths)
        {
            var names = new List<string>();

            Bind(config, names, configPaths);

            return names
                .Select(a => Assembly.Load(new AssemblyName(a)))
                .ToArray();
        }

        public static T Create<T>(this IConfiguration config)
            where T : new()
        {
            var type = typeof(T);
            return config.Create<T>(type.Name);
        }

        public static T Create<T>(this IConfiguration config, params string[] configPaths)
            where T : new()
        {
            var value = new T();
            Bind(config, value, configPaths);
            return value;
        }

        public static object Create(this IConfiguration config, Type type)
        {
            return config.Create(type, type.Name);
        }

        public static object Create(this IConfiguration config, Type type, params string[] configPaths)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var value = Activator.CreateInstance(type);
            Bind(config, value, configPaths);
            return value;
        }

        public static T CreateAndValidate<T>(this IConfiguration config)
            where T : new()
        {
            var type = typeof(T);
            return config.CreateAndValidate<T>(type.Name);
        }

        public static T CreateAndValidate<T>(this IConfiguration config, params string[] configPaths)
            where T : new()
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var value = new T();
            BindAndValidate(config, value, configPaths);
            return value;
        }

        public static object CreateAndValidate(this IConfiguration config, Type type)
        {
            return config.CreateAndValidate(type, type.Name);
        }

        public static object CreateAndValidate(this IConfiguration config, Type type, params string[] configPaths)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var value = Activator.CreateInstance(type);
            BindAndValidate(config, value, configPaths);
            return value;
        }

        public static void BindAndValidate<T>(this IConfiguration config, object value)
        {
            var type = typeof(T);
            config.BindAndValidate(value, type.Name);
        }

        public static void BindAndValidate<T>(this IConfiguration config, object value, params string[] configPaths)
        {
            var type = typeof(T);
            config.BindAndValidate(value, configPaths);
        }

        public static void BindAndValidate(this IConfiguration config, object value, Type type)
        {
            config.BindAndValidate(value, type.Name);
        }

        public static void BindAndValidate(this IConfiguration config, object value, params string[] configPaths)
        {
            if (configPaths == null)
                throw new ArgumentNullException(nameof(configPaths));

            Bind(config, value, configPaths);

            var context = new ValidationContext(value);
            context.ValidateObject();
        }

        public static IReadOnlyList<IConfigurationSection> GetSections(this IConfiguration config, params string[] configPaths)
        {
            var sections = new List<IConfigurationSection>();

            foreach (var configPath in configPaths.Reverse())
            {
                var sectionNames = configPath.Split('.');
                var firstSectionName = sectionNames.First();

                var section = config.GetSection(firstSectionName);
                foreach (var sectionName in sectionNames.Skip(1))
                    section = section.GetSection(sectionName);

                sections.Add(section);
            }

            return sections;
        }

        private static void Bind(IConfiguration config, object value, string[] configPaths)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (configPaths.Length == 0 || configPaths.All(p => string.IsNullOrWhiteSpace(p)))
            {
                config.Bind(value);
                return;
            }

            var sections = config.GetSections(configPaths);
            foreach (var section in sections)
                section.Bind(value);
        }
    }
}
