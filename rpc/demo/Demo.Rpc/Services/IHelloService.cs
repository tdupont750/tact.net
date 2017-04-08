using Demo.Rpc.Models;
using System.Threading.Tasks;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Services
{
    [RpcServiceDefinition]
    public interface IHelloService
    {
        Task<HelloResponse> SayHelloAsync(HelloRequest helloRequest);
    }
}
