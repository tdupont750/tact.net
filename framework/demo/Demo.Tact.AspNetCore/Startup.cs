using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using Tact.Diagnostics.Implementation;
using Tact.Practices.Implementation;

namespace Tact.AspNetCore.Demo.Service
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var log = new EmptyLog();
            Container = new AspNetCoreContainer(log);
        }

        public IConfigurationRoot Configuration { get; }

        private AspNetCoreContainer Container { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var assemblies = new[]
            {
                Assembly.Load(new AssemblyName("Tact.AspNetCore.Demo"))
            };

            Container.RegisterByAttribute(assemblies);
            Container.RegisterCollection(services);

            return Container;

            // services.AddSingleton<IHelloService, HelloService>();
            // return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvc();

            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(() => { Container?.Dispose(); });
        }
    }
}