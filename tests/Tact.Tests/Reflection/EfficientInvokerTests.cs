using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Tact.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace Tact.Tests.Reflection
{
    public class EfficientInvokerTests
    {
        public const int MinPerformanceIncrease = 5;
        public const int Iterations = 1000000;

        private static readonly TestClass Obj = new TestClass();
        private static readonly object[] Args = { 1, true };

        private readonly ITestOutputHelper _output;

        public EfficientInvokerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void DelegateComparison()
        {
            var count = 0;
            Func<int, int, bool> func = (a, b) =>
            {
                Interlocked.Increment(ref count);
                return a == b;
            };

            var ticks0 = DelegateDynamicInvoke(func);
            Assert.Equal(Iterations, count);

            var ticks1 = DelegateEfficientInvoke(func);
            Assert.Equal(Iterations * 2, count);

            Assert.True(ticks1 < ticks0 * MinPerformanceIncrease);
        }
        
        private long DelegateDynamicInvoke(Delegate d)
        {
            var sw0 = Stopwatch.StartNew();
            d.DynamicInvoke(1, 1);
            sw0.Stop();

            _output.WriteLine(sw0.ElapsedTicks.ToString());

            var sw1 = Stopwatch.StartNew();
            for (var i = 1; i < Iterations; i++)
            {
                d.DynamicInvoke(i, i);
            }
            sw1.Stop();

            _output.WriteLine(sw1.ElapsedTicks.ToString());
            return sw1.ElapsedTicks;
        }

        [Fact]
        public long DelegateEfficientInvoke(Delegate d)
        {
            var sw0 = Stopwatch.StartNew();
            var x = d.GetEfficientInvoker();
            x.Invoke(d, 1, 1);
            sw0.Stop();

            _output.WriteLine(sw0.ElapsedTicks.ToString());

            var sw1 = Stopwatch.StartNew();
            for (var i = 1; i < Iterations; i++)
            {
                x.Invoke(d, i, i);
            }
            sw1.Stop();

            _output.WriteLine(sw1.ElapsedTicks.ToString());
            return sw1.ElapsedTicks;
        }

        [Fact]
        public void MethodComparison()
        {
            var ticks0 = MethodInfoInvoke();
            Assert.Equal(Iterations, Obj.Count);

            var ticks1 = CachedMethodInfoInvoke();
            Assert.Equal(Iterations * 2, Obj.Count);

            var ticks2 = MethodEfficientInvoker();
            Assert.Equal(Iterations * 3, Obj.Count);

            Assert.True(ticks1 < ticks0);
            Assert.True(ticks2 < ticks1 * MinPerformanceIncrease);
        }

        [Fact]
        public long MethodInfoInvoke()
        {
            var sw0 = Stopwatch.StartNew();
            Obj.GetType().GetTypeInfo().GetMethod("TestMethod").Invoke(Obj, Args);
            sw0.Stop();

            _output.WriteLine(sw0.ElapsedMilliseconds.ToString());

            var sw1 = Stopwatch.StartNew();
            for (var i = 1; i < Iterations; i++)
            {
                Obj.GetType().GetTypeInfo().GetMethod("TestMethod").Invoke(Obj, Args);
            }
            sw1.Stop();

            _output.WriteLine(sw1.ElapsedTicks.ToString());
            return sw1.ElapsedTicks;
        }

        [Fact]
        public long CachedMethodInfoInvoke()
        {
            var map = new ConcurrentDictionary<Type, MethodInfo>();

            var sw0 = Stopwatch.StartNew();
            map.GetOrAdd(Obj.GetType(), type => type.GetTypeInfo().GetMethod("TestMethod")).Invoke(Obj, Args);
            sw0.Stop();

            _output.WriteLine(sw0.ElapsedMilliseconds.ToString());

            var sw1 = Stopwatch.StartNew();
            for (var i = 1; i < Iterations; i++)
            {
                map.GetOrAdd(Obj.GetType(), type => type.GetTypeInfo().GetMethod("TestMethod")).Invoke(Obj, Args);
            }
            sw1.Stop();

            _output.WriteLine(sw1.ElapsedTicks.ToString());
            return sw1.ElapsedTicks;
        }
        
        [Fact]
        public long MethodEfficientInvoker()
        {
            var sw0 = Stopwatch.StartNew();
            var x = Obj.GetType().GetEfficientInvoker("TestMethod");
            x.Invoke(Obj, Args);
            sw0.Stop();

            _output.WriteLine(sw0.ElapsedMilliseconds.ToString());

            var sw1 = Stopwatch.StartNew();
            for (var i = 1; i < Iterations; i++)
            {
                x.Invoke(Obj, Args);
            }
            sw1.Stop();

            _output.WriteLine(sw1.ElapsedTicks.ToString());
            return sw1.ElapsedTicks;
        }

        private class TestClass
        {
            public int Count;

            // ReSharper disable once UnusedMember.Local
            public int TestMethod(int i, bool b)
            {
                Interlocked.Increment(ref Count);
                return i + (b ? 1 : 2);
            }
        }
    }
}
