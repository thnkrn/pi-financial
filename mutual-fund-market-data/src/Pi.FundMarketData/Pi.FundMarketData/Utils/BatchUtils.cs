namespace Pi.FundMarketData.Utils;

public class BatchUtils
{
    public static async Task RunAsConcurrentBatchAsync<T>(IEnumerable<T> batchInputs, Func<T, Task> func, int concurrentSize, CancellationToken ct)
    {
        var tasks = new List<Task>();
        using var semaLock = new SemaphoreSlim(concurrentSize);
        foreach (var item in batchInputs)
        {
            await semaLock.WaitAsync(ct);

            var task = Task.Run(async () =>
            {
                try
                {
                    await func(item);
                }
                finally
                {
                    semaLock.Release();
                }
            }, ct);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    public static IEnumerable<IEnumerable<T>> GroupToBatches<T>(IEnumerable<T> items, int batchSize)
    {
        return items
            .Select((item, index) => new { Index = index, Value = item })
            .GroupBy(x => x.Index / batchSize)
            .Select(grp => grp.Select(x => x.Value));
    }
}
