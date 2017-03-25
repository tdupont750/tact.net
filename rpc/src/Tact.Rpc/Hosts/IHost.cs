using System.Threading;
using System.Threading.Tasks;

namespace Tact.Rpc.Hosts
{
    public interface IHost
    {
        Task InitializeAsync(CancellationToken cancelToken = default(CancellationToken));
    }
}
