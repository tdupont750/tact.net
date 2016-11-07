using Tact.Core.Tests.ComponentModel.DataAnnotations;
using Tact.Core.Tests.Extensions;
using Tact.Core.Tests.Practices;

namespace Tact.Tests.Console
{
    public class Program
    {
        public static void Main(string[] args)
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

            System.Console.WriteLine("...Complete");
        }
    }
}
