using Demo.Rpc.Models;
using System.Threading.Tasks;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Services
{
    [RpcServiceDefinition]
    public interface IHelloService
    {
        Task<HelloResponseCollection> SayHelloAsync(HelloRequest helloRequest);
    }
}
