using System;
using System.ComponentModel.DataAnnotations;
using Tact.ComponentModel.DataAnnotations;
using Tact.Rpc.Configuration;

namespace Demo.Rpc.Configuration
{
    public class HttpClientConfig
    {
        [IsValidationEnabled]
        public bool IsEnabled => Mode == ServiceMode.Remote && "HTTP".Equals(Protocol, StringComparison.OrdinalIgnoreCase);

        public ServiceMode Mode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Protocol { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Serializer { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Url { get; set; }
    }
}
