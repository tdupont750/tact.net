using System;
using System.Threading.Tasks;
using Tact.Rpc.Models;

namespace Tact.Rpc.Hosts
{
    public interface IWebSocketEndpoint
    {
        bool CanHandle(RemoteCallInfo callInfo, out Type modelType);

        Task<object> HandleAsync(RemoteCallInfo callInfo, object model);
    }
}
