using System.Reflection;

namespace Tact.Rpc.Hosts
{
    public interface IApplicationPart
    {
        Assembly Assembly { get; }
    }
}
