using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Tact.Diagnostics.Implementation;
using Tact.Practices.Implementation;
using Xunit;
using Xunit.Abstractions;

namespace Tact.Tests.Practices
{
    public class TactContainerTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public TactContainerTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Fact]
        public void PerformanceTest()
        {
            using (var container = new TactContainer(new InMemoryLog()))
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

                var sw = Stopwatch.StartNew();

                for (var i = 0; i < 100000; i++)
                    using (container.BeginScope()) { }

                sw.Stop();

                _outputHelper.WriteLine(sw.ElapsedMilliseconds.ToString());
            }
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
