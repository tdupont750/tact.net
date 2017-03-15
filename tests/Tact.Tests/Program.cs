using System;
using System.Threading;
using Tact.Tests.Practices;
using Xunit.Abstractions;

namespace Tact.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Thread.Sleep(2000);
            Console.WriteLine("Start");
            new TactContainerTests(new ConsoleOutputHelper()).PerformanceTest();
            Console.WriteLine("Stop");
            Thread.Sleep(2000);
        }

        public class ConsoleOutputHelper : ITestOutputHelper
        {
            public void WriteLine(string message)
            {
                Console.WriteLine(message);
            }

            public void WriteLine(string format, params object[] args)
            {
                Console.WriteLine(format, args);
            }
        }
    }
}
