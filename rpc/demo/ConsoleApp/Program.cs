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
            // Determine what mode the application is running in.
            if (!Enum.TryParse(args.FirstOrDefault() ?? "Both", out ClientMode clientMode))
            {
                var modes = string.Join(", ", Enum.GetNames(typeof(ClientMode)));
                Console.WriteLine($"First argument must specify a mode: {modes}");
                return -1;
            }

            // Step 1 - Create a logger.
            var log = new InMemoryLog();

            // Step 2 - Create a container.
            using (var container = new TactContainer(log))
            {
                // Step 3 - Read and aggregate configuration files.
                var config = container.BuildConfiguration(cb => cb
                    .AddJsonFile("AppSettings.json")
                    .AddJsonFile($"AppSettings.{clientMode}.json"));
                
                // Step 4 - Load assemblies from the configuration.
                var assemblies = config.LoadAssembliesFromConfig();
                
                // Step 5 - Create and validate configuration objects.
                container.ConfigureByAttribute(config, assemblies);

                // Step 6 - Register services by reflection using attributes.
                container.RegisterByAttribute(assemblies);

                // Step 7 - Initialize / start services in the container.
                container.InitializeByAttribute(assemblies);
                
                // Wait for the user to press enter to exit the application.
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