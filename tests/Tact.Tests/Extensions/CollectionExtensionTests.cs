using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Tact.Tests.Extensions
{
    public class CollectionExtensionTests
    {
        [Fact]
        public async Task OrderedResults()
        {
            const int count = 50;

            var results = await Enumerable
                .Range(0, count)
                .ToArray()
                .WhenAll(async v =>
                {
                    await Task.Delay(15 + (v % 15)).ConfigureAwait(false);
                    return -v;
                })
                .ConfigureAwait(false);

            for (var i = 0; i < count; i++)
                Assert.Equal(-i, results[i]);
        }
    }
}