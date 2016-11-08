using System.Collections.Generic;
using System.Reflection;
using Tact.Core.Tests.ComponentModel.DataAnnotations;
using Tact.Core.Tests.Extensions;
using Tact.Core.Tests.Practices;
using Tact.Diagnostics.Implementation;
using Tact.Practices;
using Tact.Practices.Implementation;
using Tact.Tests.Console.Configuration;
using Tact.Tests.Console.Services;
using Tact.Tests.Console.Services.Implementation;
using Xunit;

namespace Tact.Tests.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Demo();
            RunTests();
        }

        private static void Demo()
        {
            var expectedThings = new List<int> {1, 2};

            IDemoService demoService;
            using (var resolver = CreateResolver())
            {
                Assert.Equal("Hello world!", DemoService.StaticString);
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
            var logger = new InMemoryLog();
            var container = new Container(logger);

            var assembly = Assembly.GetEntryAssembly();
            var configFactory = new ConfigurationFactory();

            container.ConfigureByAttribute(configFactory, assembly);
            container.RegisterByAttribute(assembly);
            container.InitializeByAttribute(assembly);
            
            Assert.Equal(11, logger.Logs.Count);

            return container;
        }

        private static void RunTests()
        {
            System.Console.WriteLine("Start...");

            new PerResolveLifetimeManagerTests().RegisterPerResolve();

            new PerScopeLifetimeManagerTests().RegisterPerScope();

            new ResolutionHandlerTests().ClassRequired();
            new ResolutionHandlerTests().ConstructorRequired();
            new ResolutionHandlerTests().DoNotThrowOnFail();
            new ResolutionHandlerTests().EnumerableResolve();
            new ResolutionHandlerTests().FuncResolve();
            new ResolutionHandlerTests().LazyResolve();
            new ResolutionHandlerTests().PreventRecursion();
            new ResolutionHandlerTests().ThrowOnFail();

            new SingletonLifetimeManagerTests().RegisterSingleton();
            new SingletonLifetimeManagerTests().RegisterSingletonInstance();
            new TransientLifetimeManagerTests().RegisterTransient();

            new TaskExtensionTests().IgnoreCancellation().Wait();
            new TaskExtensionTests().IgnoreCancellationWithToken().Wait();
            new TaskExtensionTests().IgnoreCancellationWithInvalidToken().Wait();
            new TaskExtensionTests().IgnoreCancellationWithException().Wait();
            new TaskExtensionTests().GenericIgnoreCancellation().Wait();
            new TaskExtensionTests().GenericIgnoreCancellationWithToken().Wait();
            new TaskExtensionTests().GenericIgnoreCancellationWithInvalidToken().Wait();
            new TaskExtensionTests().GenericIgnoreCancellationWithException().Wait();

            new RequireNonDefaultTests().AllErrors();
            new RequireNonDefaultTests().NoErrors();
            new RequireNonDefaultTests().Strings();
            new RequireNonDefaultTests().NoAttributes();

            new IsEnabledAttributeTests().ValidIsEnabled();
            new IsEnabledAttributeTests().InvalidIsEnabled();
            new IsEnabledAttributeTests().ValidNotEnabled();
            new IsEnabledAttributeTests().InvalidNotEnabled();
            new IsEnabledAttributeTests().InvalidIsEnabledModelTest();

            new RegisterByAttributesTests().RegisterByAttribute();

            new RegisterConditionTests().ShouldRegisterFalse();
            new RegisterConditionTests().ShouldRegisterTrue();

            System.Console.WriteLine("...Complete");
        }
    }
}
