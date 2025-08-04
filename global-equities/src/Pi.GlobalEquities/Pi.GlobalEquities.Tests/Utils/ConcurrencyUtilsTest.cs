using Pi.GlobalEquities.Utils;
using Xunit;

namespace Pi.GlobalEquities.Tests.Utils;

public class ConcurrencyUtilsTest
{
    public class RunAsConcurrentAsyncTest
    {
        [Fact]
        public async Task RunAsConcurrentAsync_ShouldProcessAllItems()
        {
            var items = new List<int> { 1, 2, 3, 4, 5 };
            var processedItems = new List<int>();
            Func<int, Task> func = async (item) =>
            {
                processedItems.Add(item);
            };
            int concurrentThreads = 2;
            var ct = CancellationToken.None;

            await ConcurrencyUtils.RunAsConcurrentAsync(items, func, concurrentThreads, ct);

            Assert.Equal(items.Count, processedItems.Count);
            Assert.All(items, item => Assert.Contains(item, processedItems));
        }
    }
}
