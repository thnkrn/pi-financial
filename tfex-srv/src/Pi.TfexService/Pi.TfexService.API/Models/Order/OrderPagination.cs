using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Pi.TfexService.Application.Models;

namespace Pi.TfexService.API.Models.Order;

public class OrderPagination
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = int.MaxValue;
    public DateOnly? DateFrom { get; init; }
    public DateOnly? DateTo { get; init; }
    public Side? Side { get; init; }
    public SetTradePaginationOrderBy? OrderBy { get; init; } = SetTradePaginationOrderBy.OrderNo;
    public SetTradePaginationOrderDir? OrderDir { get; init; } = SetTradePaginationOrderDir.Desc;

    public void Validate()
    {
        if (Page < 1)
        {
            throw new ArgumentException($"Invalid Page: {Page}", nameof(Page));
        }

        if (PageSize < 1)
        {
            throw new ArgumentException($"Invalid PageSize: {PageSize}", nameof(PageSize));
        }

        if (Side != null && !Enum.IsDefined(typeof(Side), Side))
        {
            throw new ArgumentException($"Invalid Side: {Side}", nameof(Side));
        }

        if (OrderBy != null && !Enum.IsDefined(typeof(SetTradePaginationOrderBy), OrderBy))
        {
            throw new ArgumentException($"Invalid OrderBy: {OrderBy}", nameof(OrderBy));
        }

        if (OrderDir != null && !Enum.IsDefined(typeof(SetTradePaginationOrderDir), OrderDir))
        {
            throw new ArgumentException($"Invalid OrderDir: {OrderDir}", nameof(OrderDir));
        }
    }

    public string? GetSort()
    {
        if (OrderBy == null || OrderDir == null)
        {
            return null;
        }
        return $"{GetEnumMemberValue(OrderBy)}:{GetEnumMemberValue(OrderDir)}";
    }

    private static string? GetEnumMemberValue<T>(T? value) where T : struct, Enum
    {
        if (value == null) return null;

        var memberName = Enum.GetName(typeof(T), value.Value);
        if (memberName == null) return null;

        var memberInfo = typeof(T).GetMember(memberName).FirstOrDefault();
        if (memberInfo == null) return memberName;

        var attribute = memberInfo.GetCustomAttribute<EnumMemberAttribute>();
        return attribute?.Value ?? memberName;
    }
}

[JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy))]
public enum SetTradePaginationOrderBy
{
    [EnumMember(Value = "orderNo")] OrderNo,
    [EnumMember(Value = "account")] Account,
    [EnumMember(Value = "series")] Series,
    [EnumMember(Value = "time")] Time,
    [EnumMember(Value = "side")] Side,
    [EnumMember(Value = "status")] Status
}

[JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy))]
public enum SetTradePaginationOrderDir
{
    [EnumMember(Value = "asc")] Asc,
    [EnumMember(Value = "desc")] Desc
}