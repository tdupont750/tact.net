using Tact.ComponentModel.DataAnnotations;
using Tact.Configuration.Attributes;

namespace Tact.Tests.Console.Configuration
{
    [RegisterConfiguration]
    public class DemoConfig
    {
        [IsEnabled]
        public bool IsEnable { get; set; }

        [RequireNonDefault]
        public string SomeString { get; set; }

        [RequireNonDefault]
        public int Thing1 { get; set; }

        [RequireNonDefault]
        public bool? Thing2 { get; set; }

        [RequireNonDefault]
        public bool? Thing3 { get; set; }
    }
}