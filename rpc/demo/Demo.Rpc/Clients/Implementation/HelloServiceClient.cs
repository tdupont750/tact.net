using Demo.Rpc.Services;
using Demo.Rpc.Models;
using System.Threading.Tasks;
using Tact.Rpc;
using Tact.Rpc.Clients;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Clients.Implementation
{
    // TODO Auto generate this
    [RpcClientImplementation(typeof(IHelloService))]
    public class HelloServiceClient : IHelloService
    {
        private static readonly string ServiceName = nameof(IHelloService).GetRpcName();

        private readonly IRpcClient _rpcClient;

        public HelloServiceClient(IRpcClientManager clientManager) => 
            _rpcClient = clientManager.GetRpcClient(ServiceName);

        public Task<HelloResponse> SayHelloAsync(HelloRequest model) =>
            _rpcClient.SendAsync<HelloRequest, HelloResponse>(
                ServiceName,
                nameof(SayHelloAsync).GetRpcName(),
                model);
    }
}
