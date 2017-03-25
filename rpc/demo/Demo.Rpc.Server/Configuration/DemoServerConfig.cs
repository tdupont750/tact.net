using Tact.ComponentModel.DataAnnotations;
using Tact.Configuration.Attributes;

namespace Demo.Rpc.Configuration
{
    [RegisterConfiguration]
    public class DemoServerConfig
    {
        [IsValidationEnabled]
        public bool IsEnabled { get; set; }
    }
}
