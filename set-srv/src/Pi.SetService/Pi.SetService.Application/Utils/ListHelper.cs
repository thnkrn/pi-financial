namespace Pi.SetService.Application.Utils;

public static class ListHelper
{
    public static List<T> ApplyPagination<T>(List<T> result, int offset, int limit)
    {
        if (offset == 0 || limit == 0) return result;

        if (offset - 1 > result.Count) return new List<T>();

        return offset - 1 + limit > result.Count
            ? result.Skip(offset - 1).ToList()
            : result.Skip(offset - 1).Take(limit).ToList();
    }
}