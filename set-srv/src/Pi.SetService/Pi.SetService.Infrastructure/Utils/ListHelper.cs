namespace Pi.SetService.Infrastructure.Utils;

public static class ListHelper
{
    public static List<List<T>> SplitList<T>(T[] list, int chunkSize)
    {
        return list
            .Select((item, index) => new { item, index })
            .GroupBy(x => x.index / chunkSize)
            .Select(g => g.Select(x => x.item).ToList())
            .ToList();
    }
}