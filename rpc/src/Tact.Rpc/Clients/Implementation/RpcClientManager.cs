using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Practices;
using Tact.Rpc.Configuration;
using Demo.Rpc.Configuration;
using System;
using Tact.Diagnostics;
using Tact.Rpc.Serialization;

namespace Tact.Rpc.Clients.Implementation
{
    [RegisterSingleton(typeof(IRpcClientManager))]
    public class RpcClientManager : IRpcClientManager
    {
        private readonly IResolver _resolver;
        private readonly ILog _log;

        public RpcClientManager(IResolver resolver, ILog log)
        {
            _resolver = resolver;
            _log = log;
        }

        public IRpcClient GetRpcClient(string service)
        {
            if (!_resolver.TryResolve(service, out IRpcClientConfig config))
                if (!_resolver.TryResolve(Constants.DefaultsConfigKey, out config))
                    throw new ArgumentException("Service Configuration Not Found", nameof(service));

            var serializer = _resolver.Resolve<ISerializer>(config.Serializer);
            var factory = _resolver.Resolve<IRpcClientFactory>(config.Protocol);
            return factory.GetRpcClient(serializer, _log, config);
        }
    }
}
