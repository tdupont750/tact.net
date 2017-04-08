using Demo.Rpc.Configuration;
using Microsoft.Extensions.Configuration;
using System;
using Tact.Configuration;
using Tact.Practices;
using Tact.Rpc.Clients;

namespace Tact.Rpc.Practices
{
    public class RpcClientConfigurationAttribute : Attribute, IRegisterConfigurationAttribute
    {
        private readonly string Protocol;

        public RpcClientConfigurationAttribute(string protocol) =>
            Protocol = protocol;

        public void Register(IContainer container, IConfiguration configuration, Type type)
        {
            var serviceSections = configuration.GetSections(Constants.ServiceConfigPathPrefix);
            foreach (var serviceSection in serviceSections)
            {
                var childSections = serviceSection.GetChildren();
                foreach (var section in childSections)
                {
                    string[] configPaths;
                    if (Constants.DefaultsConfigKey.Equals(section.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        configPaths = new[] { Constants.ServiceConfigDefaults };
                    }
                    else
                    {
                        var configPath = Constants.ServiceConfigPathPrefix + "." + section.Key;
                        configPaths = new[] { configPath, Constants.ServiceConfigDefaults };
                    }

                    var config = configuration.CreateAndValidate(type, configPaths) as IRpcClientConfig;

                    if (Protocol.Equals(config.Protocol, StringComparison.OrdinalIgnoreCase))
                        container.RegisterInstance(config, section.Key);
                }
            }
        }
    }
}
