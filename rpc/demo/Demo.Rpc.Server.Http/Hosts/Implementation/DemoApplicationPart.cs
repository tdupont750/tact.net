using System.Reflection;
using Tact.Practices.LifetimeManagers.Attributes;
using Tact.Rpc.Hosts;

namespace Demo.Rpc.Hosts.Implementation
{
    [RegisterSingleton(typeof(IApplicationPart), nameof(DemoApplicationPart))]
    public class DemoApplicationPart : IApplicationPart
    {
        private static readonly Assembly Assembly = typeof(DemoApplicationPart).GetTypeInfo().Assembly;

        Assembly IApplicationPart.Assembly => Assembly;
    }
}
