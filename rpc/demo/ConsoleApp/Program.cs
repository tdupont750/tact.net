using Demo.Rpc.Models;
using Demo.Rpc.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tact;
using Tact.Diagnostics.Implementation;
using Tact.Practices;
using Tact.Practices.Implementation;

namespace ConsoleApp
{
    public static class Program
    {
        public enum ClientMode
        {
            Both,
            Client,
            Server
        }

        public static int Main(string[] args)
        {
            if (!Enum.TryParse(args.FirstOrDefault() ?? "Both", out ClientMode clientMode))
            {
                var modes = string.Join(", ", Enum.GetNames(typeof(ClientMode)));
                Console.WriteLine($"First argument must specify a mode: {modes}");
                return -1;
            }

            var log = new EmptyLog();
            using (var container = new TactContainer(log))
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("AppSettings.json")
                    .AddJsonFile($"AppSettings.{clientMode}.json")
                    .Build();

                var assemblies = config.GetContainerAssemblies();
                
                container.ConfigureByAttribute(config, assemblies);
                container.RegisterByAttribute(assemblies);
                container.InitializeByAttribute(assemblies);

                if (clientMode == ClientMode.Server)
                    PressEnterTask.Value.Wait();
                else
                    TestApplication(container).Wait();
            }

            return 0;
        }
        
        private static async Task TestApplication(IResolver resolver)
        {
            var client = resolver.Resolve<IHelloService>();

            while(true)
            {
                var delayTask = Task.Delay(1000);
                
                await Task.WhenAny(delayTask, PressEnterTask.Value).ConfigureAwait(false);

                if (PressEnterTask.Value.IsCompleted)
                    return;

                var response = await client
                    .SayHelloAsync(new HelloRequest { Name = "Tom" })
                    .ConfigureAwait(false);

                Console.WriteLine($"{DateTime.Now.Minute}:{DateTime.Now.Second} - {response.Message}");
            }
        }

        private static readonly Lazy<Task> PressEnterTask = new Lazy<Task>(() => Task.Run(() =>
        {
            Console.WriteLine("Press enter to exit.");
            Console.ReadLine();
        }));
    }
}