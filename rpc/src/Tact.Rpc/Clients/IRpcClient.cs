using System.Threading;
using System.Threading.Tasks;

namespace Tact.Rpc.Clients
{
    public interface IRpcClient
    {
        Task<TResponse> SendAsync<TRequest, TResponse>(string serviceName, string methodName, TRequest model, CancellationToken cancelToken = default(CancellationToken));
    }
}
