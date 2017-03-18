using System;
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
            bool resolveFunc = true,
            bool resolveUnregistered = true,
            bool resolveEnumerable = true,
            bool resolveCollection = true,
            bool resolveList = true,
            bool resolveGenerics = true,
            bool includeUnkeyedInResolveAll = false)
            : base(log, maxDisposeParallelization, includeUnkeyedInResolveAll)
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

            if (resolveGenerics)
                resolutionHandlers.Add(new GenericResolutionHandler());

            ResolutionHandlers = resolutionHandlers;
        }

        public TactContainer(
            ILog log,
            IReadOnlyList<IResolutionHandler> resolutionHandlers,
            int? maxDisposeParallelization = null,
            bool includeUnkeyedInResolveAll = false)
            : base(log, maxDisposeParallelization, includeUnkeyedInResolveAll)
        {
            ResolutionHandlers = resolutionHandlers ?? throw new ArgumentNullException(nameof(resolutionHandlers));
        }

        private TactContainer(
            ILog log,
            IReadOnlyList<IResolutionHandler> resolutionHandlers,
            int? maxDisposeParallelization,
            bool includeUnkeyedInResolveAll,
            TactContainer parentScope)
            : base(log, maxDisposeParallelization, includeUnkeyedInResolveAll, parentScope)
        {
            ResolutionHandlers = resolutionHandlers;
        }

        public override IContainer BeginScope()
        {
            return new TactContainer(Log, ResolutionHandlers, MaxDisposeParallelization, IncludeUnkeyedInResovleAll, this);
        }

        protected override IReadOnlyList<IResolutionHandler> ResolutionHandlers { get; }
    }
}