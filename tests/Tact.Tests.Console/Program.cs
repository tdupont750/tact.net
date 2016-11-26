using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Tact.Diagnostics.Implementation;
using Tact.Practices;
using Tact.Practices.Implementation;
using Tact.Tests.ComponentModel.DataAnnotations;
using Tact.Tests.Console.Services;
using Tact.Tests.Extensions;
using Tact.Tests.Practices;
using Tact.Tests.Reflection;
using Tact.Tests.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Tact.Tests.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Thread.Sleep(1000);
            
            Demo();
            RunTests();

            Thread.Sleep(1000);
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
            var container = new Container(logger);

            var assembly = Assembly.GetEntryAssembly();

            container.ConfigureByAttribute(config, assembly);
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

            new ProxyLifetimeManagerTests().PerScopeProxy();
            new ProxyLifetimeManagerTests().SingletonProxy();
            new ProxyLifetimeManagerTests().KeyProxy();

            new TaskExtensionTests().IgnoreCancellation().Wait();
            new TaskExtensionTests().IgnoreCancellationWithToken().Wait();
            new TaskExtensionTests().IgnoreCancellationWithInvalidToken().Wait();
            new TaskExtensionTests().IgnoreCancellationWithException().Wait();
            new TaskExtensionTests().GenericIgnoreCancellation().Wait();
            new TaskExtensionTests().GenericIgnoreCancellationWithToken().Wait();
            new TaskExtensionTests().GenericIgnoreCancellationWithInvalidToken().Wait();
            new TaskExtensionTests().GenericIgnoreCancellationWithException().Wait();
            new TaskExtensionTests().GetResult();

            new TypeExtensionTests().DefaultConstructor();
            new TypeExtensionTests().OneConstructor();
            new TypeExtensionTests().TwoConstructors();

            new EnumerableExtensionTests().WhenAll().Wait();
            new EnumerableExtensionTests().BailAfterFirstException().Wait();

            new CollectionExtensionTests().OrderedResults().Wait();

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

            new DisposableTests().AsyncDisposableTest().Wait();
            new DisposableTests().DisposableTest().Wait();
            new DisposableTests().NonDisposableTest().Wait();

            new UsingTests().UsingTest().Wait();
            new UsingTests().UsingThrows().Wait();

            new SemaphoreSlimExtensionTests().UseAsync().Wait();
            new ReaderWriterLockSlimExtensionTests().Use();

            var testOutputHelper1 = new TestOutputHelper();
            new EfficientInvokerTests(testOutputHelper1).DelegateComparison();

            var testOutputHelper2 = new TestOutputHelper();
            new EfficientInvokerTests(testOutputHelper2).MethodComparison();

            var testOutputHelper3 = new TestOutputHelper();
            new EfficientInvokerTests(testOutputHelper3).PropertyComparison();

            var testOutputHelper4 = new TestOutputHelper();
            new EfficientInvokerTests(testOutputHelper4).InvokeAsync().Wait();

            System.Console.WriteLine("...Complete");
        }

        public class TestOutputHelper : ITestOutputHelper
        {
            public ConcurrentQueue<string> Lines { get; } = new ConcurrentQueue<string>();

            public void WriteLine(string message)
            {
                Lines.Enqueue(message);
            }

            public void WriteLine(string format, params object[] args)
            {
                var message = string.Format(format, args);
                Lines.Enqueue(message);
            }
        }
    }
}
