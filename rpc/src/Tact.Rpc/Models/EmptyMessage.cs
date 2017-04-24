using System.Threading.Tasks;
using Tact.Rpc.Practices;

namespace Tact.Rpc.Models
{
    [RpcModel]
    public sealed class EmptyMessage
    {
        public static readonly EmptyMessage Instance = new EmptyMessage();

        public static readonly Task<EmptyMessage> Task = System.Threading.Tasks.Task.FromResult(Instance);

        public static implicit operator Task<EmptyMessage>(EmptyMessage model) => Task;
    }
}
