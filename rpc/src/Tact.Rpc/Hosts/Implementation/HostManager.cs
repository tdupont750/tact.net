using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tact.Configuration;
using Tact.Practices;
using Tact.Practices.LifetimeManagers.Attributes;

namespace Tact.Rpc.Hosts.Implementation
{
    [Initalize, RegisterSingleton(typeof(IHostManager))]
    public class HostManager : IHostManager
    {
        private readonly IReadOnlyList<IHost> _hosts;

        public HostManager(IReadOnlyList<IHost> hosts)
        {
            _hosts = hosts;
        }

        public async Task InitializeAsync(CancellationToken cancelToken = default(CancellationToken))
        {
            foreach (var host in _hosts)
                await host.InitializeAsync(cancelToken).ConfigureAwait(false);
        }

        public class InitalizeAttribute : Attribute, IInitializeAttribute
        {
            public void Initialize(IContainer container)
            {
                var hostManager = container.Resolve<IHostManager>();
                
                // TODO ADD ASYNC SUPPORT?
                hostManager.InitializeAsync().Wait();
            }
        }
    }
}
