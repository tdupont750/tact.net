using System.ComponentModel.DataAnnotations;
using Tact.ComponentModel.DataAnnotations;
using Tact.Configuration.Attributes;

namespace Demo.Rpc.Configuration
{
    [RegisterConfiguration]
    public class HttpClientConfig
    {
        [IsValidationEnabled]
        public bool IsEnabled { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string HostUrl { get; set; }
    }
}
