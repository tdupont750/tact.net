using System.ComponentModel.DataAnnotations;
using Tact.ComponentModel.DataAnnotations;
using Tact.Configuration.Attributes;

namespace Tact.Rpc.Configuration
{
    [RegisterConfiguration]
    public class HttpServerConfig
    {
        [IsValidationEnabled]
        public bool IsEnabled { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string HostUrl { get; set; }
    }
}
