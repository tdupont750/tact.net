using System.Collections.Generic;
using Tact.Diagnostics;
using Tact.Practices.Base;
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
            bool resolveEnumerable = true,
            bool resolveFunc = true,
            bool resolveUnregistered = true,
            bool throwOnFailedResolve = true)
            : base(log, maxDisposeParallelization)
        {
            ResolutionHandlers = new List<IResolutionHandler>();

            if (resolveLazy)
                ResolutionHandlers.Add(new LazyResolutionHandler());

            if (resolveEnumerable)
                ResolutionHandlers.Add(new EnumerableResolutionHandler());

            if (resolveFunc)
                ResolutionHandlers.Add(new FuncResolutionHandler());

            if (resolveUnregistered)
                ResolutionHandlers.Add(new UnregisteredResolutionHandler());

            if (throwOnFailedResolve)
                ResolutionHandlers.Add(new ThrowOnFailResolutionHandler());
        }

        public TactContainer(ILog log, 
            IList<IResolutionHandler> resolutionHandlers,
            int? maxDisposeParallelization = null)
            : base(log, maxDisposeParallelization)
        {
            ResolutionHandlers = resolutionHandlers ?? new List<IResolutionHandler>();
        }

        protected override IList<IResolutionHandler> ResolutionHandlers { get; }

        protected override ContainerBase CreateScope()
        {
            return new TactContainer(Log, ResolutionHandlers, MaxDisposeParallelization);
        }
    }
}