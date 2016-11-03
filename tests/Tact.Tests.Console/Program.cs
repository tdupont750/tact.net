using System.Threading;
using System.Threading.Tasks;
using Tact.Core.Tests.Extensions;
using Tact.Core.Tests.Practices;

namespace Tact.Tests.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Console.WriteLine("Start...");

            new PerResolveRegistrationTests().RegisterPerResolve();
            
            new PerScopeRegistrationTests().RegisterPerScope();
            
            new ResolutionHandlerTests().ClassRequired();
            new ResolutionHandlerTests().ConstructorRequired();
            new ResolutionHandlerTests().DoNotThrowOnFail();
            new ResolutionHandlerTests().EnumerableResolve();
            new ResolutionHandlerTests().FuncResolve();
            new ResolutionHandlerTests().LazyResolve();
            new ResolutionHandlerTests().PreventRecursion();
            new ResolutionHandlerTests().ThrowOnFail();
            
            new SingletonRegistrationTests().RegisterSingleton();
            new SingletonRegistrationTests().RegisterSingletonInstance();
            new TransientRegistrationTests().RegisterTransient();

            new TaskExtensionTests().IgnoreCancellation().Wait();
            new TaskExtensionTests().IgnoreCancellationWithToken().Wait();
            new TaskExtensionTests().IgnoreCancellationWithInvalidToken().Wait();
            new TaskExtensionTests().IgnoreCancellationWithException().Wait();
            new TaskExtensionTests().GenericIgnoreCancellation().Wait();
            new TaskExtensionTests().GenericIgnoreCancellationWithToken().Wait();
            new TaskExtensionTests().GenericIgnoreCancellationWithInvalidToken().Wait();
            new TaskExtensionTests().GenericIgnoreCancellationWithException().Wait();

            System.Console.WriteLine("...Complete");
        }
    }
}
