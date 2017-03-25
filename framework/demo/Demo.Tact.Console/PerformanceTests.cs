using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Tact.Diagnostics.Implementation;
using Tact.Practices.Implementation;

namespace Tact.Tests.Console
{
    public static class PerformanceTests
    {
        private static int _requestCount;

        public static void WebRequests(string url = null)
        {
            const string defaultUrl = "http://localhost:8080/api/demo/hello/tom";

            WebRequests(025, 4000, url ?? defaultUrl).Wait();
            WebRequests(050, 2000, url ?? defaultUrl).Wait();
            WebRequests(100, 1000, url ?? defaultUrl).Wait();
            WebRequests(200, 0500, url ?? defaultUrl).Wait();
        }

        private static async Task WebRequests(int parallel, int count, string url)
        {
            _requestCount = 0;

            var sw = Stopwatch.StartNew();

            var tasks = new List<Task>();
            for (var i = 0; i < parallel; i++)
                tasks.Add(MakeRequests(count, url));

            await Task.WhenAll(tasks).ConfigureAwait(false);

            sw.Stop();

            System.Console.WriteLine($"{parallel} * {count} = { (int)(((float)_requestCount / sw.ElapsedMilliseconds) * 1000)} rps");
        }

        private static async Task MakeRequests(int count, string url)
        {
            using (var httpClient = new HttpClient())
            {
                for (var i = 0; i < count; i++)
                    using (var x = await httpClient.GetAsync(url).ConfigureAwait(false))
                    {
                        x.EnsureSuccessStatusCode();
                        Interlocked.Increment(ref _requestCount);
                    }
            }
        }

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
