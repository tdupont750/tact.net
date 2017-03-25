using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Tact.Diagnostics.Implementation;
using Tact.Practices;
using Tact.Practices.Implementation;
using Tact.Tests.Console.Services;
using Xunit;

namespace Tact.Tests.Console
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
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("AppSettings.json");
            var config = configBuilder.Build();

            var logger = new InMemoryLog();
            var container = new TactContainer(logger);

            var assembly = Assembly.GetEntryAssembly();

            container.ConfigureByAttribute(config, assembly);
            container.RegisterByAttribute(assembly);
            container.InitializeByAttribute(assembly);
            
            Assert.Equal(11, logger.LogLines.Count);

            return container;
        }
    }
}
