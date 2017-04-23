using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
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

        public HttpHost(IResolver resolver, HttpHostConfig hostConfig)
        {
            _webHost = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(hostConfig.Urls.ToArray())
                .ConfigureServices(services =>
                {
                    services.AddSingleton(resolver);
                    services.AddMvc();
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
                var config = container.Resolve<HttpHostConfig>();
                return config.IsEnabled;
            }
        }
    }
}
