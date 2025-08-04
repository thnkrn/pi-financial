namespace Pi.GlobalEquities.Utils;

public class ConcurrencyUtils
{
    public static async Task RunAsConcurrentAsync<T>(IEnumerable<T> batchInputs, Func<T, Task> func,
        int concurrentThreads, CancellationToken ct)
    {
        var tasks = new List<Task>();
        using var semaLock = new SemaphoreSlim(concurrentThreads, concurrentThreads);
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
}
