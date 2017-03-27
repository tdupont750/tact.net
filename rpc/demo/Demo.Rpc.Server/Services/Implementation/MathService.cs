using System.Threading.Tasks;
using Demo.Rpc.Models;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Services.Implementation
{
    [RegisterServiceCondition(nameof(HelloService)), RegisterSingleton(typeof(IMathService))]
    public class MathService : IMathService
    {
        public Task<SumResponse> SumAsync(SumRequest sumRequest)
        {
            return Task.FromResult(new SumResponse
            {
                Sum = sumRequest.X + sumRequest.Y
            });
        }
    }
}
