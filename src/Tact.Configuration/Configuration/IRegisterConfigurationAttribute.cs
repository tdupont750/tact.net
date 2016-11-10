using System;
using Microsoft.Extensions.Configuration;
using Tact.Practices;

namespace Tact.Configuration
{
    public interface IRegisterConfigurationAttribute
    {
        void Register(IContainer container, IConfiguration configuration, Type type);
    }
}