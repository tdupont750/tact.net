using Demo.Rpc.Services;
using Demo.Rpc.Models;
using System.Threading.Tasks;
using Tact.Rpc;
using Tact.Rpc.Clients;
using Tact.Rpc.Practices;

namespace Demo.Rpc.Clients.Implementation
{
    // TODO Auto generate this
    [RpcClientImplementation(typeof(IMathService))]
    public class MathServiceClient : IMathService
    {
        private static readonly string ServiceName = nameof(IMathService).GetRpcName();

        private readonly IRpcClient _rpcClient;
        
        public MathServiceClient(IRpcClientManager clientManager) =>
            _rpcClient = clientManager.GetRpcClient(ServiceName);

        public Task<SumResponse> SumAsync(SumRequest model) =>
            _rpcClient.SendAsync<SumRequest, SumResponse>(
                ServiceName,
                nameof(SumAsync).GetRpcName(),
                model);
    }
}
