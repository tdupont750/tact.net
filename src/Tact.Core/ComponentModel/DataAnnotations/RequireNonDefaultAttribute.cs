using System;
using System.Reflection;

namespace Tact.ComponentModel.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequireNonDefaultAttribute : Attribute, IModelValidationAttribute
    {
        private static readonly Type StringType = typeof(string);

        public string ErrorMessage { get; } = "Non Default Value Required";

        public bool IsValid(PropertyInfo propertyInfo, object source)
        {
            var value = propertyInfo.GetValue(source);

            if (propertyInfo.PropertyType == StringType)
            {
                var s = (string)value;
                return !string.IsNullOrWhiteSpace(s);
            }

            var propertyTypeInfo = propertyInfo.PropertyType.GetTypeInfo();
            var defaultValue = propertyTypeInfo.IsValueType
                ? Activator.CreateInstance(propertyInfo.PropertyType)
                : null;

            return !Equals(value, defaultValue);
        }
    }
}