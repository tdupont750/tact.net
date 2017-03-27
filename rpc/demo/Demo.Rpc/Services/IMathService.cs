using Demo.Rpc.Models;
using System.Threading.Tasks;

namespace Demo.Rpc.Services
{
    public interface IMathService
    {
        Task<SumResponse> SumAsync(SumRequest sumRequest);
    }
}
