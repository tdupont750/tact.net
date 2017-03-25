using Demo.Rpc.Models;
using System.Threading.Tasks;

namespace Demo.Rpc.Services
{
    public interface IHelloService
    {
        Task<HelloResponse> SayHelloAsync(HelloRequest helloRequest);
    }
}
