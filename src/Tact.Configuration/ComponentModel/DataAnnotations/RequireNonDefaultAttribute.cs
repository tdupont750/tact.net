using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Tact.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequireNonDefaultAttribute : ValidationAttribute
    {
        private static readonly ConcurrentDictionary<string, PropertyInfo> PropertyMap = new ConcurrentDictionary<string, PropertyInfo>();

        private static readonly Type StringType = typeof(string);

        public override string FormatErrorMessage(string name)
        {
            return $"Non Default Value Required for '{name}'";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var key = $"{validationContext.ObjectType.FullName}.{validationContext.MemberName}";
            var property = PropertyMap.GetOrAdd(key, k => validationContext.ObjectType.GetTypeInfo().GetProperty(validationContext.MemberName));

            if (IsValid(value, property.PropertyType))
                return null;

            var message = FormatErrorMessage(validationContext.MemberName);
            return new ValidationResult(message, new[] {validationContext.MemberName});
        }

        private static bool IsValid(object value, Type type)
        {
            if (value == null)
                return false;
            
            if (type == StringType)
            {
                var stringValue = (string) value;
                return !string.IsNullOrWhiteSpace(stringValue);
            }

            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsValueType)
                return true;
            
            var defaultValue = Activator.CreateInstance(type);
            return !Equals(value, defaultValue);
        }
    }
}