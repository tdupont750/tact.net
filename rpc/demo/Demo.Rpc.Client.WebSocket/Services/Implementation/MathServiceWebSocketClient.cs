using System.Threading.Tasks;
using Demo.Rpc.Models;
using Tact.Practices;
using Tact.Rpc.Services.Base;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Services.Implementation
{
    [RegisterWebSocketClientCondition(ServiceName), RegisterSingleton(typeof(IMathService))]
    public class MathServiceWebSocketClient : WebSocketClientBase, IMathService
    {
        private const string ServiceName = "MathService";

        public MathServiceWebSocketClient(IResolver resolver)
            : base(resolver, ServiceName)
        {
        }

        public Task<SumResponse> SumAsync(SumRequest sumRequest)
        {
            return SendAsync<SumRequest, SumResponse>(sumRequest, "Sum");
        }
    }
}
