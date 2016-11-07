using System;

namespace Tact.Configuration
{
    public interface IConfigurationFactory
    {
        T CreateObject<T>();

        object CreateObject(Type type);
    }
}
