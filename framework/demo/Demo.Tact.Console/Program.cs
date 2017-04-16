using System.Collections.Generic;
using System.Reflection;
using Demo.Tact.Console.Services;
using Microsoft.Extensions.Configuration;
using Tact;
using Tact.Diagnostics.Implementation;
using Tact.Practices;
using Tact.Practices.Implementation;
using Xunit;

namespace Demo.Tact.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // PerformanceTests.WebRequests();
            // PerformanceTests.Container();

            Demo();
        }

        private static void Demo()
        {
            var expectedThings = new List<int> {1, 2};

            IDemoService demoService;
            using (var resolver = CreateResolver())
            {
                Assert.Equal("Hello world!", StaticHelper.StaticString);
                demoService = resolver.Resolve<IDemoService>();

                var things = demoService.DemoAllOfTheThings();
                Assert.Equal(expectedThings, things);

                IDemoService scopeDemoService;
                using (var scope = resolver.BeginScope())
                {
                    scopeDemoService = scope.Resolve<IDemoService>();
                    Assert.NotEqual(demoService.Id, scopeDemoService.Id);

                    var scopeThings = scopeDemoService.DemoAllOfTheThings();
                    Assert.Equal(expectedThings, scopeThings);
                }

                Assert.False(demoService.IsDisposed);
                Assert.True(scopeDemoService.IsDisposed);
            }

            Assert.True(demoService.IsDisposed);
        }

        private static IResolver CreateResolver()
        {
            // Step 1 - Create a logger.
            var log = NLogWrapper.GetLog("Default");

            // Step 2 - Create a container.
            var container = new TactContainer(log);

            // Step 3 - Read and aggregate configuration files.
            var config = container.BuildConfiguration(cb =>
                cb.AddJsonFile("AppSettings.json"));

            // Step 4 - Load assemblies from the configuration.
            var assemblies = config.LoadAssembliesFromConfig();

            // Step 5 - Create and validate configuration objects.
            container.ConfigureByAttribute(config, assemblies);

            // Step 6 - Register services by reflection using attributes.
            container.RegisterByAttribute(assemblies);

            // Step 7 - Initialize / start services in the container.
            container.InitializeByAttribute(assemblies);

            return container;
        }
    }
}
