using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Tact.ComponentModel.DataAnnotations;

namespace Tact
{
    public static class ValidationContextExtensions
    {
        private static readonly Type BoolType = typeof(bool);

        private static readonly ConcurrentDictionary<Type, PropertyInfo> IsEnabledMap =
            new ConcurrentDictionary<Type, PropertyInfo>();

        public static void ValidateObject(this ValidationContext context, bool validateAllProperties = true)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var instance = context.ObjectInstance;
            var type = instance.GetType();

            var property = IsEnabledMap.GetOrAdd(type, t => t.GetRuntimeProperties().SingleOrDefault(p =>
            {
                if (p.GetCustomAttribute<IsValidationEnabledAttribute>() == null)
                    return false;

                if (p.PropertyType == BoolType)
                    return true;

                throw new InvalidOperationException(
                    $"{nameof(IsValidationEnabledAttribute)} can only be applied to boolean properties");
            }));

            if (property != null)
            {
                var isEnabled = (bool) property.GetValue(instance);
                if (!isEnabled) return;
            }

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(instance, context, results, validateAllProperties);
            if (results.Count == 0)
                return;

            throw new AggregateException(results.Select(r => new ValidationException(r.ErrorMessage)));
        }
    }
}