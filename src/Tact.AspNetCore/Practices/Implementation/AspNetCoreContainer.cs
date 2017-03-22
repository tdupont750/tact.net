using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Tact.Diagnostics;
using Tact.Practices.Base;
using Tact.Practices.ResolutionHandlers;

namespace Tact.Practices.Implementation
{
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

        IServiceProvider IServiceScope.ServiceProvider => this;

        public override IContainer BeginScope()
        {
            return CreateScope();
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            TryResolve(out object service, serviceType);
            return service;
        }

        object ISupportRequiredService.GetRequiredService(Type serviceType)
        {
            return Resolve(serviceType);
        }

        IServiceScope IServiceScopeFactory.CreateScope()
        {
            return CreateScope();
        }

        public void RegisterCollection(IServiceCollection services)
        {
            this.RegisterProxy<IServiceProvider>(r => (IServiceProvider)r);
            this.RegisterProxy<ISupportRequiredService>(r => (ISupportRequiredService)r);
            this.RegisterProxy<IServiceScopeFactory>(r => (IServiceScopeFactory)r);
            this.RegisterProxy<IServiceScope>(r => (IServiceScope)r);

            foreach (var service in services)
            {
                switch (service.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        if (service.ImplementationInstance != null)
                            this.RegisterInstance(service.ServiceType, service.ImplementationInstance);
                        else if (service.ImplementationFactory != null)
                            this.RegisterSingleton(service.ServiceType, factory: r => service.ImplementationFactory(this));
                        else
                            this.RegisterSingleton(service.ServiceType, service.ImplementationType);
                        break;

                    case ServiceLifetime.Scoped:
                        if (service.ImplementationFactory != null)
                            this.RegisterPerScope(service.ServiceType, factory: r => service.ImplementationFactory(this));
                        else
                            this.RegisterPerScope(service.ServiceType, service.ImplementationType);
                        break;

                    case ServiceLifetime.Transient:
                        if (service.ImplementationFactory != null)
                            this.RegisterTransient(service.ServiceType, factory: r => service.ImplementationFactory(this));
                        else
                            this.RegisterTransient(service.ServiceType, service.ImplementationType);
                        break;
                }
            }
        }

        private AspNetCoreContainer CreateScope()
        {
            return new AspNetCoreContainer(Log, ResolutionHandlers, MaxDisposeParallelization, this);
        }
    }
}
