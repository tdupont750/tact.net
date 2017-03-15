using System.Diagnostics;
using System.Threading;
using Tact.Diagnostics.Implementation;
using Tact.Practices.Implementation;

namespace Tact.Tests.Console
{
    public static class PerformanceTests
    {
        public static void Container()
        {
            Thread.Sleep(2000);
            System.Console.WriteLine("Start");

            using (var container = new TactContainer(new EmptyLog()))
            {
                container.RegisterPerScope<IOne, One>();
                container.RegisterPerScope<ITwo, Two>();

                container.RegisterSingleton<IThree, Three>();
                container.RegisterSingleton<IFour, Four>();
                container.RegisterSingleton<IFive, Five>();
                container.RegisterSingleton<ISix, Six>();
                container.RegisterSingleton<ISeven, Seven>();
                container.RegisterSingleton<IEigth, Eight>();
                container.RegisterSingleton<INine, Nine>();
                container.RegisterSingleton<ITen, Ten>();

                var sw1 = Stopwatch.StartNew();
                using (var scope = container.BeginScope())
                {
                    scope.Resolve<IOne>();
                    scope.Resolve<ITen>();
                }
                sw1.Stop();

                var sw2 = Stopwatch.StartNew();
                for (var i = 0; i < 1000000; i++)
                    using (var scope = container.BeginScope())
                    {
                        scope.Resolve<IOne>();
                        scope.Resolve<ITen>();
                    }
                sw2.Stop();

                System.Console.WriteLine(sw1.ElapsedMilliseconds.ToString());
                System.Console.WriteLine(sw2.ElapsedMilliseconds.ToString());
            }

            System.Console.WriteLine("Stop");
            Thread.Sleep(2000);
        }

        private interface IOne { }
        private class One : IOne { }

        private interface ITwo { }
        private class Two : ITwo { }

        private interface IThree { }
        private class Three : IThree { }

        private interface IFour { }
        private class Four : IFour { }

        private interface IFive { }
        private class Five : IFive { }

        private interface ISix { }
        private class Six : ISix { }

        private interface ISeven { }
        private class Seven : ISeven { }

        private interface IEigth { }
        private class Eight : IEigth { }

        private interface INine { }
        private class Nine : INine { }

        private interface ITen { }
        private class Ten : ITen { }
    }
}
