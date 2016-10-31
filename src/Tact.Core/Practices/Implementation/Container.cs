using System.Collections.Generic;
using Tact.Diagnostics;
using Tact.Practices.Base;
using Tact.Practices.ResolutionHandlers;
using Tact.Practices.ResolutionHandlers.Implementation;

namespace Tact.Practices.Implementation
{
    public sealed class Container : ContainerBase
    {
        public Container(
            ILog log,
            bool throwOnFailedResolve = true,
            bool resolveUnregistered = true,
            bool resolveLazy = true,
            bool resolveEnumerable = true,
            bool resolveFunc = true)
            : base(log)
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

        public Container(ILog log, IList<IResolutionHandler> resolutionHandlers)
            : base(log)
        {
            ResolutionHandlers = resolutionHandlers;
        }

        protected override IList<IResolutionHandler> ResolutionHandlers { get; }

        protected override ContainerBase CreateScope()
        {
            return new Container(Log, ResolutionHandlers);
        }
    }
}