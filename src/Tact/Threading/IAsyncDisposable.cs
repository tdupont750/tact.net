using System;
using System.Threading;
using System.Threading.Tasks;

namespace Tact.Threading
{
    public interface IAsyncDisposable : IDisposable
    {
        Task DisposeAsync(CancellationToken cancelToken = default(CancellationToken));
    }
}