using System.ComponentModel;
using System.Reflection;

namespace Pi.BackofficeService.API.Models;

public record NameAliasResponse(string Name, string Alias);

public record NameAliasResponse<TEnum> where TEnum : Enum
{
    public NameAliasResponse(TEnum data)
    {
        Alias = data.ToString();
        Name = data.ToString();

        var field = typeof(TEnum).GetField(data.ToString());
        if (field == null) return;

        var attribute = field.GetCustomAttribute<DescriptionAttribute>();
        Name = attribute?.Description ?? data.ToString();
    }

    public string Name { get; set; }
    public string Alias { get; set; }
};
