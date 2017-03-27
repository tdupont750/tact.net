using Demo.Rpc.Configuration;
using System.Collections.Generic;
using Tact.ComponentModel.DataAnnotations;
using Tact.Configuration.Attributes;

namespace Tact.Rpc.Configuration
{
    [RegisterConfiguration(Constants.ConfigHostsPathPrefix + "HTTP", Constants.DefaultHostsConfigPath)]
    public class HttpHostConfig
    {
        [IsValidationEnabled]
        public bool IsEnabled => Urls != null && Urls.Count > 0;
        
        public List<string> Urls { get; set; }
    }
}
