using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;
using Tact.Diagnostics;
using Tact.Diagnostics.Implementation;
using Tact.Practices;
using Tact.Practices.Base;
using Tact.Practices.Implementation;
using Tact.Practices.ResolutionHandlers;

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
            Container = new TactContainer(log, includeUnkeyedInResolveAll: true);
        }

        public IConfigurationRoot Configuration { get; }

        public IContainer Container { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var assemblies = new[]
            {
                Assembly.Load(new AssemblyName("Tact.AspNetCore.Demo"))
            };

            Container.RegisterByAttribute(assemblies);

            var serviceProvider = new TactServiceProvider(Container);
            serviceProvider.RegisterCollection(services);
            return serviceProvider;
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvc();

            var applicationLifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopping.Register(() => { Container?.Dispose(); });
        }
        
        public sealed class AspNetCoreContainer : ContainerBase, IServiceProvider, ISupportRequiredService, IServiceScopeFactory, IServiceScope
        {
            private const bool IncludeUnkeyedInResolveAll = true;

            public AspNetCoreContainer(
                ILog log,
                int? maxDisposeParallelization = null,
                bool resolveLazy = true,
                bool resolveFunc = true,
                bool resolveUnregistered = true,
                bool resolveEnumerable = true,
                bool resolveCollection = true,
                bool resolveList = true,
                bool resolveGenerics = true)
                : base(log, maxDisposeParallelization, IncludeUnkeyedInResolveAll)
            {
                ResolutionHandlers = CreateDefaultHandlers(
                    resolveLazy,
                    resolveFunc,
                    resolveUnregistered,
                    resolveEnumerable,
                    resolveCollection,
                    resolveList,
                    resolveGenerics);
            }

            public AspNetCoreContainer(
                ILog log,
                IReadOnlyList<IResolutionHandler> resolutionHandlers,
                int? maxDisposeParallelization = null)
                : base(log, maxDisposeParallelization, IncludeUnkeyedInResolveAll)
            {
                ResolutionHandlers = resolutionHandlers ?? throw new ArgumentNullException(nameof(resolutionHandlers));
            }

            private AspNetCoreContainer(
                ILog log,
                IReadOnlyList<IResolutionHandler> resolutionHandlers,
                int? maxDisposeParallelization,
                AspNetCoreContainer parentScope)
                : base(log, maxDisposeParallelization, IncludeUnkeyedInResolveAll, parentScope)
            {
                ResolutionHandlers = resolutionHandlers;
            }

            protected override IReadOnlyList<IResolutionHandler> ResolutionHandlers { get; }

            public IServiceProvider ServiceProvider => this;

            public override IContainer BeginScope()
            {
                return CreateScope();
            }

            public object GetService(Type serviceType)
            {
                TryResolve(out object service, serviceType);
                return service;
            }

            public object GetRequiredService(Type serviceType)
            {
                return Resolve(serviceType);
            }

            IServiceScope IServiceScopeFactory.CreateScope()
            {
                return CreateScope();
            }
            
            private AspNetCoreContainer CreateScope()
            {
                return new AspNetCoreContainer(Log, ResolutionHandlers, MaxDisposeParallelization, this);
            }
        }

        private class TactServiceProvider : IServiceProvider, ISupportRequiredService, IServiceScopeFactory, IServiceScope
        {
            private readonly IContainer _container;

            private bool _isDisposed;

            public TactServiceProvider(IContainer container)
            {
                _container = container;
            }

            public IServiceProvider ServiceProvider => this;

            public object GetService(Type serviceType)
            {
                _container.TryResolve(out object service, serviceType);
                return service;
            }

            public object GetRequiredService(Type serviceType)
            {
                return _container.Resolve(serviceType);
            }

            public IServiceScope CreateScope()
            {
                var scope = _container.BeginScope();
                return new TactServiceProvider(scope);
            }

            public void RegisterCollection(IServiceCollection services)
            {
                // _container.RegisterSingleton(factory: r => (IServiceProvider)r);
                // _container.RegisterSingleton(factory: r => (ISupportRequiredService)r);
                // _container.RegisterSingleton(factory: r => (IServiceScopeFactory)r);

                _container.RegisterInstance<IServiceProvider>(this);
                _container.RegisterInstance<ISupportRequiredService>(this);
                _container.RegisterInstance<IServiceScopeFactory>(this);
                
                foreach (var service in services)
                {
                    switch (service.Lifetime)
                    {
                        case ServiceLifetime.Singleton:
                            if (service.ImplementationInstance != null)
                                _container.RegisterInstance(service.ServiceType, service.ImplementationInstance);
                            else if (service.ImplementationFactory != null)
                                _container.RegisterSingleton(service.ServiceType, factory: r => service.ImplementationFactory(this));
                            else
                                _container.RegisterSingleton(service.ServiceType, service.ImplementationType);
                            break;

                        case ServiceLifetime.Scoped:
                            if (service.ImplementationFactory != null)
                                _container.RegisterPerScope(service.ServiceType, factory: r => service.ImplementationFactory(this));
                            else
                                _container.RegisterPerScope(service.ServiceType, service.ImplementationType);
                            break;

                        case ServiceLifetime.Transient:
                            if (service.ImplementationFactory != null)
                                _container.RegisterTransient(service.ServiceType, factory: r => service.ImplementationFactory(this));
                            else
                                _container.RegisterTransient(service.ServiceType, service.ImplementationType);
                            break;
                    }
                }
            }

            public void Dispose()
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;
                _container.Dispose();
            }
        }
    }
}