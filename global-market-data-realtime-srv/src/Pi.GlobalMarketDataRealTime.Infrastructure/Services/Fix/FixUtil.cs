using QuickFix.Fields;
using QuickFix.FIX44;

namespace Pi.GlobalMarketDataRealTime.Infrastructure.Services.Fix;

public static class FixUtil
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public static bool IsFixAvailable { get; set; }

    public static MarketDataRequest CreateMarketDataRequest(
        string reqId,
        List<(string symbol, string securityID, string securityIDSource)> symbols
    )
    {
        var request = new MarketDataRequest();

        // Set the Market Data Request ID (unique ID)
        request.Set(new MDReqID(reqId));

        // Set subscription request type (1 for Snapshot + Updates)
        request.Set(new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES));

        // Set market depth (1 for top of book)
        request.Set(new MarketDepth(1));

        // Set MD Update Type (0 for Full refresh) - Only use valid values: 0 or 1
        request.Set(new MDUpdateType(MDUpdateType.FULL_REFRESH)); // Using 0 for FULL_REFRESH

        // Set MD Entry Types
        var group = new MarketDataRequest.NoMDEntryTypesGroup();

        // Add valid entry types
        var entryTypes = new[]
        {
            MDEntryType.BID, // '0'
            MDEntryType.OFFER, // '1'
            MDEntryType.TRADE, // '2'
            MDEntryType.OPENING_PRICE, // '4'
            MDEntryType.CLOSING_PRICE, // '5'
            MDEntryType.TRADE_VOLUME // 'B'
        };

        foreach (var entryType in entryTypes)
        {
            group.Set(new MDEntryType(entryType));
            request.AddGroup(group);
        }

        // Set symbols and their security identifiers
        foreach (var (symbol, securityId, securityIdSource) in symbols)
        {
            var relatedSymGroup = new MarketDataRequest.NoRelatedSymGroup();
            relatedSymGroup.Set(new Symbol(symbol));
            relatedSymGroup.Set(new SecurityID(securityId));
            relatedSymGroup.Set(new SecurityIDSource(securityIdSource));
            request.AddGroup(relatedSymGroup);
        }

        // Set UseExchangeTimestamp
        request.SetField(new UseExchangeTimestamp(UseExchangeTimestamp.VelexaTimestamp));

        return request;
    }

    public static SecurityListRequest CreateSecurityListRequest(
        string securityReqId,
        int securityListRequestType,
        string? symbol = null,
        string? cfiCode = null
    )
    {
        var request = new SecurityListRequest();
        request.SetField(new SecurityReqID(securityReqId));
        request.SetField(new SecurityListRequestType(securityListRequestType));

        if (!string.IsNullOrEmpty(symbol)) request.SetField(new Symbol(symbol));

        if (!string.IsNullOrEmpty(cfiCode)) request.SetField(new CFICode(cfiCode));

        return request;
    }

    public static TestRequest FixTestRequest()
    {
        var request = new TestRequest();
        var reqId = Guid.NewGuid().ToString();
        request.SetField(new TestReqID(reqId));
        return request;
    }
}

public class UseExchangeTimestamp : CharField
{
    public const int ExchangeTag = 20066;
    public const char ExchangeTimestamp = 'Y';
    public const char VelexaTimestamp = 'N';

    public UseExchangeTimestamp()
        : base(ExchangeTag)
    {
    }

    public UseExchangeTimestamp(char val)
        : base(ExchangeTag, val)
    {
    }
}