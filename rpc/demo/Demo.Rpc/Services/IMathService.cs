using Demo.Rpc.Models;
using System.Threading.Tasks;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Services
{
    [RpcService]
    public interface IMathService
    {
        Task<SumResponse> SumAsync(SumRequest sumRequest);
    }
}
