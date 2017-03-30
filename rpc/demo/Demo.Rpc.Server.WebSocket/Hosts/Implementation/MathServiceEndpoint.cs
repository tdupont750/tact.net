using Demo.Rpc.Models;
using Demo.Rpc.Services;
using System;
using System.Threading.Tasks;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Hosts;
using Tact.Rpc.Models;

namespace Demo.Rpc.Hosts.Implementation
{
    [RegisterSingleton(typeof(IWebSocketEndpoint), ServiceName)]
    public class MathServiceEndpoint : IWebSocketEndpoint
    {
        private const string ServiceName = "MathService";

        private readonly IMathService _mathService;

        public MathServiceEndpoint(IMathService mathService)
        {
            _mathService = mathService;
        }

        public bool CanHandle(RemoteCallInfo callInfo, out Type modelType)
        {
            if (!ServiceName.Equals(callInfo.Service, StringComparison.OrdinalIgnoreCase))
            {
                modelType = null;
                return false;
            }

            switch (callInfo.Method.ToLowerInvariant())
            {
                case "sum":
                    modelType = typeof(SumRequest);
                    return true;

                default:
                    modelType = null;
                    return false;
            }
        }

        public async Task<object> HandleAsync(RemoteCallInfo callInfo, object model)
        {
            switch (callInfo.Method.ToLowerInvariant())
            {
                case "sum":
                    return await _mathService
                        .SumAsync((SumRequest)model)
                        .ConfigureAwait(false);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
