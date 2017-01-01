using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tact.Collections;
using Xunit;
using Xunit.Abstractions;

namespace Tact.Tests.Collections
{
    public class ObjectPoolTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public ObjectPoolTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        [Theory]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(6)]
        [InlineData(8)]
        public async Task Parallelism(int maxParallelism)
        {
            const int count = 100000;

            var idSeed = 0;

            var swA = Stopwatch.StartNew();
            using (var pool = new ObjectPool<TestObject>(100, () => new TestObject(Interlocked.Increment(ref idSeed))))
            {
                var tasks = Enumerable
                .Range(0, maxParallelism)
                .Select(t => Task.Run(() =>
                {
                    for (var i = 0; i < count; i++)
                    {
                        // ReSharper disable AccessToDisposedClosure
                        var value = pool.Aquire();
                        value.WasteTime();
                        pool.Release(value);
                        // ReSharper restore AccessToDisposedClosure
                    }
                }))
                .ToArray();

                await Task.WhenAll(tasks).ConfigureAwait(false);
            }

            swA.Stop();

            _outputHelper.WriteLine($"MaxParallelism: {maxParallelism} - ElapsedMilliseconds: {swA.ElapsedMilliseconds}");
        }
        
        [Fact]
        public void AquireAndRelease()
        {
            TestObject a, b, c, d, e, f;

            var idSeed = 0;

            using (var pool = new ObjectPool<TestObject>(2, () => new TestObject(Interlocked.Increment(ref idSeed))))
            {
                a = pool.Aquire();
                b = pool.Aquire();
                c = pool.Aquire();

                pool.Release(c);
                pool.Release(b);
                pool.Release(a);

                d = pool.Aquire();
                e = pool.Aquire();
                f = pool.Aquire();

                pool.Release(f);
                pool.Release(e);
                pool.Release(d);

                Assert.Equal(1, a.Id);
                Assert.Equal(2, b.Id);
                Assert.Equal(3, c.Id);
                Assert.Same(b, d);
                Assert.Same(c, e);
                Assert.Equal(4, f.Id);
            }

            Assert.False(a.IsDisposed);
            Assert.False(b.IsDisposed);
            Assert.True(c.IsDisposed);
            Assert.False(d.IsDisposed);
            Assert.True(e.IsDisposed);
            Assert.True(f.IsDisposed);
        }

        [Fact]
        public void TryAquire()
        {
            var idSeed = 0;

            using (var pool = new ObjectPool<TestObject>(1, () => new TestObject(Interlocked.Increment(ref idSeed))))
            {
                TestObject value;
                Assert.False(pool.TryAquire(out value));

                value = pool.Aquire();
                Assert.True(pool.Release(value));
                Assert.False(pool.Release(value));

                Assert.True(pool.TryAquire(out value));
            }
        }

        [Fact]
        public void Use()
        {
            ObjectPool<TestObject>.UsableValue a, b, c, d, e, f;

            var idSeed = 0;

            using (var pool = new ObjectPool<TestObject>(2, () => new TestObject(Interlocked.Increment(ref idSeed))))
            {
                using (a = pool.Use())
                using (b = pool.Use())
                using (c = pool.Use())
                {
                }

                using (d = pool.Use())
                using (e = pool.Use())
                using (f = pool.Use())
                {
                    Assert.Equal(1, a.Value.Id);
                    Assert.Equal(2, b.Value.Id);
                    Assert.Equal(3, c.Value.Id);
                    Assert.Same(b.Value, d.Value);
                    Assert.Same(c.Value, e.Value);
                    Assert.Equal(4, f.Value.Id);
                }
            }

            Assert.False(a.Value.IsDisposed);
            Assert.False(b.Value.IsDisposed);
            Assert.True(c.Value.IsDisposed);
            Assert.False(d.Value.IsDisposed);
            Assert.True(e.Value.IsDisposed);
            Assert.True(f.Value.IsDisposed);
        }

        private class TestObject : IDisposable
        {
            public TestObject(int id)
            {
                Id = id;
            }

            public int Id { get; }

            public bool IsDisposed { get; private set; }

            public void WasteTime()
            {
                var random = new Random();
                var i = random.Next(10, 100);
                while (i > 0) i--;
            }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }
    }
}
