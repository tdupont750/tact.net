using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tact.ComponentModel.DataAnnotations;

namespace Tact.ComponentModel
{
    public static class ModelValidation
    {
        private static readonly Type BoolType = typeof(bool);

        private static readonly ConcurrentDictionary<Type, PropertyInfo> IsEnabledPropertyMap =
            new ConcurrentDictionary<Type, PropertyInfo>();

        private static readonly ConcurrentDictionary<Type, Tuple<PropertyInfo, IModelValidationAttribute[]>[]> ValidationPropertyMap =
            new ConcurrentDictionary<Type, Tuple<PropertyInfo, IModelValidationAttribute[]>[]>();

        private const BindingFlags PropertyFlags =
            BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance;

        public static void Validate(object value)
        {
            var type = value.GetType();

            var isEnabled = IsEnabled(value, type);
            if (!isEnabled)
                return;

            var properties = ValidationPropertyMap.GetOrAdd(type, t => t
                .GetTypeInfo()
                .GetProperties(PropertyFlags)
                .Select(p =>
                {
                    var attributes = p
                        .GetCustomAttributes()
                        .OfType<IModelValidationAttribute>()
                        .ToArray();

                    return Tuple.Create(p, attributes);
                })
                .ToArray());

            var modelErrors = new Dictionary<string, IReadOnlyCollection<string>>();
            foreach (var property in properties)
            {
                var propertyErrors = new List<string>();
                foreach (var attribute in property.Item2)
                {
                    if (attribute.IsValid(property.Item1, value))
                        continue;
                    
                    propertyErrors.Add(attribute.ErrorMessage);
                }

                if (propertyErrors.Count == 0)
                    continue;
                
                modelErrors[property.Item1.Name] = propertyErrors;
            }

            if (modelErrors.Count == 0)
                return;

            throw new ModelValidationException(type.Name, modelErrors);
        }

        private static bool IsEnabled(object value, Type type)
        {
            var isEnabledProperty = IsEnabledPropertyMap.GetOrAdd(type, t => t
                .GetTypeInfo()
                .GetProperties(PropertyFlags)
                .SingleOrDefault(p =>
                {
                    var attribute = p.GetCustomAttribute<IsEnabledAttribute>();
                    if (attribute == null)
                        return false;

                    if (p.PropertyType != BoolType)
                        throw new InvalidOperationException("IsEnabledAttribute can only apply to a Boolean");

                    return true;
                }));

            if (isEnabledProperty == null)
                return true;

            return (bool)isEnabledProperty.GetValue(value);
        }
    }
}