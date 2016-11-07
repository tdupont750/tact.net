using System;
using Tact.Practices;

namespace Tact.Configuration
{
    public interface IRegisterConfigurationAttribute
    {
        void Register(IContainer container, IConfigurationFactory configurationFactory, Type type);
    }
}