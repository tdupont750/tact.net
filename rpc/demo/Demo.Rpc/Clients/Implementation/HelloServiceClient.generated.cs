using Demo.Rpc.Models;
using Demo.Rpc.Services;
using System.Threading.Tasks;
using Tact.Rpc;
using Tact.Rpc.Clients;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Clients.Implementation
{
    [RpcClientImplementation(typeof(IHelloService))]
    public class HelloServiceClient : IHelloService
    {
        private const string ServiceName = "HelloService";

        private readonly IRpcClient _rpcClient;
        
        public HelloServiceClient(IRpcClientManager clientManager) =>
            _rpcClient = clientManager.GetRpcClient(ServiceName);

        public Task<HelloResponse> SayHelloAsync(HelloRequest model) =>
            _rpcClient.SendAsync<HelloRequest, HelloResponse>(
                ServiceName,
                "SayHello",
                model);
    }
}