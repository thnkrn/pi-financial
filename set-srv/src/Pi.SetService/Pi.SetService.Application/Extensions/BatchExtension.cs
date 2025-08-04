using System.Runtime.CompilerServices;

namespace Pi.SetService.Application.Extensions;

public static class BatchExtension
{
    public static async IAsyncEnumerable<List<T>> ProcessBatch<T>(this IAsyncEnumerable<T> source, int size,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var batch = new List<T>();
        await foreach (var item in source.WithCancellation(ct))
        {
            if (ct.IsCancellationRequested)
                yield break;

            batch.Add(item);
            if (batch.Count < size) continue;

            yield return batch;
            batch = [];
        }

        if (batch.Count > 0)
        {
            yield return batch;
        }
    }
}
