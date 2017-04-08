using System;
using System.ComponentModel.DataAnnotations;
using Tact.ComponentModel.DataAnnotations;
using Tact.Rpc.Clients;
using Tact.Rpc.Practices;

namespace Tact.Rpc.Configuration
{
    [RpcClientConfiguration(ProtocolName)]
    public class WebSocketClientConfig : IRpcClientConfig
    {
        public const string ProtocolName = "WebSocket";

        [IsValidationEnabled]
        public bool IsEnabled => Mode == ServiceMode.Remote && ProtocolName.Equals(Protocol, StringComparison.OrdinalIgnoreCase);

        public ServiceMode Mode { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Protocol { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Serializer { get; set; }

        [Required(AllowEmptyStrings = false)]
        public string Url { get; set; }
    }
}
