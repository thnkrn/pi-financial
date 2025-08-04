using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Pi.TfexService.Application.Models;

namespace Pi.TfexService.API.Models.Order;

public class OrderTradePagination
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = int.MaxValue;
    public required DateOnly DateFrom { get; init; }
    public required DateOnly DateTo { get; init; }
    public Side? Side { get; init; }
    public Position? Position { get; init; }
    public ItPaginationOrderBy? OrderBy { get; init; } = ItPaginationOrderBy.TradeDateTime;
    public ItPaginationOrderDir? OrderDir { get; init; } = ItPaginationOrderDir.Desc;

    public void Validate()
    {
        if (Page < 1)
        {
            throw new InvalidOperationException("Invalid Page");
        }

        if (PageSize < 1)
        {
            throw new InvalidOperationException("Invalid PageSize");
        }

        if (Side != null && !Enum.IsDefined(typeof(Side), Side))
        {
            throw new InvalidOperationException("Invalid Side");
        }

        if (OrderBy != null && !Enum.IsDefined(typeof(ItPaginationOrderBy), OrderBy))
        {
            throw new InvalidOperationException("Invalid OrderBy");
        }

        if (OrderDir != null && !Enum.IsDefined(typeof(ItPaginationOrderDir), OrderDir))
        {
            throw new InvalidOperationException("Invalid OrderDir");
        }
    }
}

[JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy))]
public enum ItPaginationOrderBy
{
    [EnumMember(Value = "series")] Series,
    [EnumMember(Value = "tradeDateTime")] TradeDateTime,
    [EnumMember(Value = "side")] Side,
    [EnumMember(Value = "position")] Position
}

[JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy))]
public enum ItPaginationOrderDir
{
    [EnumMember(Value = "asc")] Asc,
    [EnumMember(Value = "desc")] Desc
}