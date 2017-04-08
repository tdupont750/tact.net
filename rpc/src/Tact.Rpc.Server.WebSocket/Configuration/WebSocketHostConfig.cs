using Demo.Rpc.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tact.ComponentModel.DataAnnotations;
using Tact.Configuration.Attributes;

namespace Tact.Rpc.Configuration
{
    [RegisterConfiguration(Constants.HostsConfigPathPrefix + ".WebSocket", Constants.HostsConfigDefaults)]
    public class WebSocketHostConfig
    {
        [IsValidationEnabled]
        public bool IsEnabled => Urls != null && Urls.Count > 0;

        [Required, MinLength(1)]
        public List<string> Urls { get; set; }
    }
}
