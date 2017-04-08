using Demo.Rpc.Models;
using Demo.Rpc.Services;
using System.Threading.Tasks;
using Tact.Rpc;
using Tact.Rpc.Clients;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Clients.Implementation
{
    [RpcClientImplementation(typeof(IMathService))]
    public class MathServiceClient : IMathService
    {
        private const string ServiceName = "MathService";

        private readonly IRpcClient _rpcClient;
        
        public MathServiceClient(IRpcClientManager clientManager) =>
            _rpcClient = clientManager.GetRpcClient(ServiceName);

        public Task<SumResponse> SumAsync(SumRequest model) =>
            _rpcClient.SendAsync<SumRequest, SumResponse>(
                ServiceName,
                "Sum",
                model);
    }
}