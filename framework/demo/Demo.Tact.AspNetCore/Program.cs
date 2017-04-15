using System.Linq;
using Microsoft.AspNetCore.Hosting;

namespace Demo.Tact.AspNetCore
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var portString = args.FirstOrDefault() ?? "8080";
            var portInt = int.Parse(portString);

            using (var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://*:{portInt}")
                .UseStartup<Startup>()
                .Build())
            {
                host.Run();
            }
        }
    }
}