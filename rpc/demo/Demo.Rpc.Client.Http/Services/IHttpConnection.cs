using System.Threading.Tasks;

namespace Demo.Rpc.Services
{
    public interface IHttpConnection
    {
        Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, string service, string method);
    }
}
