using System.Reflection;

namespace Tact.ComponentModel
{
    public interface IModelValidationAttribute
    {
        string ErrorMessage { get; }

        bool IsValid(PropertyInfo propertyInfo, object source);
    }
}