using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Tact.Rpc.Generator
{
    public class GeneratorConfig
    {
        [Required(AllowEmptyStrings = false)]
        public string CurrentDirectory { get; set; } = Directory.GetCurrentDirectory();

        [Required(AllowEmptyStrings = false)]
        public string ServiceDirectory { get; set; } = "Services";

        [Required(AllowEmptyStrings = false)]
        public string ClientDirectory { get; set; } = "Clients\\Implementation";

        [Required(AllowEmptyStrings = false)]
        public string FileExtension { get; set; } = ".generated.cs";

        [Required(AllowEmptyStrings = false)]
        public string ServiceSearchFilter { get; set; } = "*.cs";

        [Required(AllowEmptyStrings = false)]
        public string ServiceNamespaceSuffix { get; set; } = "Services";
        
        [Required(AllowEmptyStrings = false)]
        public string ClientClassSuffix { get; set; } = "Client";

        [Required(AllowEmptyStrings = false)]
        public string IncludeNamespaces { get; set; } = "System.Threading.Tasks|Tact.Rpc|Tact.Rpc.Clients|Tact.Rpc.Practices";

        public bool CreateDirectories { get; set; } = true;

        public string ServicePath => Path.Combine(CurrentDirectory, ServiceDirectory);

        public string ClientPath => Path.Combine(CurrentDirectory, ClientDirectory);
    }
}