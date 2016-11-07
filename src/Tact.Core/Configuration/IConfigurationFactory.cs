using System;

namespace Tact.Configuration
{
    public interface IConfigurationFactory
    {
        object CreateObject(Type type);
    }
}
