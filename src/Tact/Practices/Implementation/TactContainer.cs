using System;
using System.Collections.Generic;
using Tact.Diagnostics;
using Tact.Practices.Base;
using Tact.Practices.LifetimeManagers;
using Tact.Practices.ResolutionHandlers;
using Tact.Practices.ResolutionHandlers.Implementation;

namespace Tact.Practices.Implementation
{
    public sealed class TactContainer : ContainerBase
    {
        public TactContainer(
            ILog log,
            int? maxDisposeParallelization = null,
            bool resolveLazy = true,
            bool resolveFunc = true,
            bool resolveUnregistered = true,
            bool resolveEnumerable = true,
            bool resolveCollection = true,
            bool resolveList = true)
            : base(log, maxDisposeParallelization)
        {
            var resolutionHandlers = new List<IResolutionHandler>();

            if (resolveLazy)
                resolutionHandlers.Add(new LazyResolutionHandler());

            if (resolveEnumerable || resolveCollection || resolveList)
                resolutionHandlers.Add(new EnumerableResolutionHandler(resolveEnumerable, resolveCollection, resolveList));

            if (resolveFunc)
                resolutionHandlers.Add(new FuncResolutionHandler());

            if (resolveUnregistered)
                resolutionHandlers.Add(new UnregisteredResolutionHandler());

            ResolutionHandlers = resolutionHandlers;
        }

        public TactContainer(
            ILog log, 
            IReadOnlyList<IResolutionHandler> resolutionHandlers,
            int? maxDisposeParallelization = null)
            : base(log, maxDisposeParallelization)
        {
            ResolutionHandlers = resolutionHandlers ?? throw new ArgumentNullException(nameof(resolutionHandlers));
        }

        private TactContainer(
            ILog log,
            IReadOnlyList<IResolutionHandler> resolutionHandlers,
            int? maxDisposeParallelization,
            Dictionary<Type, ILifetimeManager> lifetimeManagerMap,
            Dictionary<Type, Dictionary<string, ILifetimeManager>> multiRegistrationMap,
            List<Type> scopedKeys,
            List<Type> multiScopedKeys)
            : base(
                  log, 
                  maxDisposeParallelization,
                  lifetimeManagerMap,
                  multiRegistrationMap,
                  scopedKeys,
                  multiScopedKeys)
        {
            ResolutionHandlers = resolutionHandlers;
        }

        protected override IReadOnlyList<IResolutionHandler> ResolutionHandlers { get; }

        protected override ContainerBase CreateScope()
        {
            CloneMaps(
                out Dictionary<Type, ILifetimeManager> lifetimeManagerMap,
                out Dictionary<Type, Dictionary<string, ILifetimeManager>> multiRegistrationMap,
                out List<Type> scopedKeys,
                out List<Type> multiScopedKeys);

            return new TactContainer(
                Log, 
                ResolutionHandlers, 
                MaxDisposeParallelization, 
                lifetimeManagerMap, 
                multiRegistrationMap,
                scopedKeys,
                multiScopedKeys);
        }
    }
}