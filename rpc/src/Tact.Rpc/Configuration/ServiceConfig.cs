using Tact.ComponentModel.DataAnnotations;

namespace Tact.Rpc.Configuration
{
    public class ServiceConfig
    {
        [IsValidationEnabled]
        public bool IsEnabled => Mode == ServiceMode.Local;

        public ServiceMode Mode { get; set; }
    }
}
