using System.Threading;
using System.Threading.Tasks;

namespace Tact.Rpc.Hosts
{
    public interface IHostManager
    {
        Task InitializeAsync(CancellationToken cancelToken = default(CancellationToken));
    }
}
