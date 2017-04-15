using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Tact.Tests.NLog
{
    public class ExceptionExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public ExceptionExtensionsTests(ITestOutputHelper output) => _output = output;
        
        [Fact]
        public void Throw()
        {
            try
            {
                ThrowIt().Wait();
            }
            catch (Exception ex)
            {
                AssertOnException(ex);
            }

            async Task ThrowIt()
            {
                await Task.CompletedTask;
                throw new InvalidProgramException(nameof(ThrowIt));
            }
        }

        [Fact]
        public void WhenAllThrow()
        {
            try
            {
                Task.WhenAll(ThrowIt()).Wait();
            }
            catch (Exception ex)
            {
                AssertOnException(ex);
            }

            async Task ThrowIt()
            {
                await Task.CompletedTask;
                throw new InvalidProgramException(nameof(ThrowIt));
            }
        }

        [Fact]
        public void WaitAllThrow()
        {
            try
            {
                Task.WaitAll(ThrowIt());
            }
            catch (Exception ex)
            {
                AssertOnException(ex);
            }

            async Task ThrowIt()
            {
                await Task.CompletedTask;
                throw new InvalidProgramException(nameof(ThrowIt));
            }
        }

        [Fact]
        public void GenericThrow()
        {
            try
            {
                var x = ThrowIt().Result;
            }
            catch (Exception ex)
            {
                AssertOnException(ex);
            }

            async Task<int> ThrowIt()
            {
                await Task.CompletedTask;
                throw new InvalidProgramException(nameof(ThrowIt));
            }
        }

        [Fact]
        public void GenericWhenAllThrow()
        {
            try
            {
                Task.WhenAll(ThrowIt()).Wait();
            }
            catch (Exception ex)
            {
                AssertOnException(ex);
            }

            async Task<int> ThrowIt()
            {
                await Task.CompletedTask;
                throw new InvalidProgramException(nameof(ThrowIt));
            }
        }

        [Fact]
        public void GenericWaitAllThrow()
        {
            try
            {
                Task.WaitAll(ThrowIt());
            }
            catch (Exception ex)
            {
                AssertOnException(ex);
            }

            async Task<int> ThrowIt()
            {
                await Task.CompletedTask;
                throw new InvalidProgramException(nameof(ThrowIt));
            }
        }

        [Fact]
        public async Task NestedThrow()
        {
            try
            {
                await ThrowIt1();
            }
            catch (Exception ex)
            {
                AssertOnException(ex, 3);
            }

            async Task<int> ThrowIt1()
            {
                await Task.CompletedTask;
                return await ThrowIt2().ConfigureAwait(false);
            }

            Task<int> ThrowIt2()
            {
                return Task.FromResult(ThrowIt3().Result);
            }

            async Task<int> ThrowIt3()
            {
                await Task.CompletedTask;
                throw new InvalidProgramException(nameof(ThrowIt3));
            }
        }

        private void AssertOnException(Exception ex, int expected = 1)
        {
            var cleanStack = ex.GetCleanStackTrace();

            _output.WriteLine(ex.StackTrace);
            _output.WriteLine("---");
            _output.WriteLine(cleanStack);

            var split = cleanStack.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            Assert.Equal(expected, split.Length);
        }
    }
}
