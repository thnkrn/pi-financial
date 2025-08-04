using System.Reflection;

namespace Pi.WalletService.Application.Utilities;

public static class TypeUtils
{
    public static List<T> GetAllPublicConstValues<T>(this Type type)
    {
        return type
            .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(fi => fi.IsLiteral && !fi.IsInitOnly)
            .Select(x => x.GetRawConstantValue())
            .OfType<T>()
            .ToList();
    }
}