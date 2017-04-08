using System;
using System.Threading.Tasks;
using Tact.Practices;

namespace Tact.Rpc.Services
{
    public interface IRpcHandler
    {
        string Name { get; }

        bool CanHandle(string service, string method, out Type argType);

        Task<object> HandleAsync(IResolver resolver, string method, object model);
    }
}
