using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace Tact
{
    public static class ConfigurationExtensions
    {
        public static T Create<T>(this IConfiguration config)
            where T : new()
        {
            var type = typeof(T);
            var value = new T();
            Bind(config, type, value);
            return value;
        }

        public static object Create(this IConfiguration config, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var value = Activator.CreateInstance(type);
            Bind(config, type, value);
            return value;
        }

        public static T CreateAndValidate<T>(this IConfiguration config)
            where T : new()
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var type = typeof(T);
            var value = new T();
            BindAndValidate(config, type, value);
            return value;
        }

        public static object CreateAndValidate(this IConfiguration config, Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var value = Activator.CreateInstance(type);
            BindAndValidate(config, type, value);
            return value;
        }

        public static void BindAndValidate<T>(IConfiguration config, object value)
        {
            var type = typeof(T);
            BindAndValidate(config, type, value);
        }

        public static void BindAndValidate(IConfiguration config, Type type, object value)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Bind(config, type, value);
            var context = new ValidationContext(value);
            context.ValidateObject();
        }

        private static void Bind(IConfiguration config, Type type, object value)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            config.GetSection(type.Name).Bind(value);
        }
    }
}
