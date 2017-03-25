using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace Tact.AspNetCore.Demo.Service
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