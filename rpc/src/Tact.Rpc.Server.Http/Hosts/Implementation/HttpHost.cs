using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tact.Practices;
using Tact.Practices.LifetimeManagers;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Configuration;

namespace Tact.Rpc.Hosts.Implementation
{
    [RegisterCondition, RegisterSingleton(typeof(IHost), nameof(HttpHost))]
    public class HttpHost : IHost, IDisposable
    {
        private readonly IWebHost _webHost;

        public HttpHost(HttpServerConfig config, IResolver resolver, IReadOnlyList<IApplicationPart> applicationParts)
        {
            _webHost = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(config.HostUrl)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(resolver);

                    var mvc = services.AddMvc();
                    
                    var assemblies = applicationParts
                        .Select(a => a.Assembly)
                        .Distinct()
                        .ToArray();

                    foreach (var assembly in assemblies)
                        mvc.AddApplicationPart(assembly);
                })
                .Configure(app =>
                {
                    app.UseMvc();
                })
                .Build();
        }

        public void Dispose()
        {
            _webHost.Dispose();
        }

        public Task InitializeAsync(CancellationToken cancelToken)
        {
            _webHost.Start();
            return Task.CompletedTask;
        }

        public class RegisterConditionAttribute : Attribute, IRegisterConditionAttribute
        {
            public bool ShouldRegister(IContainer container, Type toType)
            {
                return container.TryResolve(out HttpServerConfig config) && config.IsEnabled;
            }
        }
    }
}
