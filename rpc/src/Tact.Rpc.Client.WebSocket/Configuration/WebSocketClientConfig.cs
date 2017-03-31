using System;
using System.ComponentModel.DataAnnotations;
using Tact.ComponentModel.DataAnnotations;

namespace Tact.Rpc.Configuration
{
    public class WebSocketClientConfig
    {
        [IsValidationEnabled]
        public bool IsEnabled => Mode == ServiceMode.Remote && "WebSocket".Equals(Protocol, StringComparison.OrdinalIgnoreCase);

        public ServiceMode Mode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Protocol { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Serializer { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Url { get; set; }
    }
}
