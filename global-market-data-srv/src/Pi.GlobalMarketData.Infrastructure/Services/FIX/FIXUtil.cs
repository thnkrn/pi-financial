using QuickFix.Fields;
using QuickFix.FIX44;

namespace Pi.GlobalMarketData.Infrastructure.Services.FIX;

public static class FIXUtil
{
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

        // Set MD Update Type (0 for Full refresh)
        request.Set(new MDUpdateType(MDUpdateType.FULL_REFRESH));

        // Set MD Entry Types (0 for Bid, 1 for Offer, 2 for Trade)
        var group = new MarketDataRequest.NoMDEntryTypesGroup();
        group.Set(new MDEntryType(MDEntryType.BID));
        request.AddGroup(group);
        group.Set(new MDEntryType(MDEntryType.OFFER));
        request.AddGroup(group);
        group.Set(new MDEntryType(MDEntryType.TRADE));
        request.AddGroup(group);

        // Set MD Entry Types (4 for Opening price, 5 for Closing price, B = Trade volume)
        group.Set(new MDEntryType(MDEntryType.OPENING_PRICE));
        request.AddGroup(group);
        group.Set(new MDEntryType(MDEntryType.CLOSING_PRICE));
        request.AddGroup(group);
        group.Set(new MDEntryType(MDEntryType.TRADE_VOLUME));
        request.AddGroup(group);

        // Set symbols and their security identifiers
        foreach (var (symbol, securityID, securityIDSource) in symbols)
        {
            var relatedSymGroup = new MarketDataRequest.NoRelatedSymGroup();
            relatedSymGroup.Set(new Symbol(symbol));
            relatedSymGroup.Set(new SecurityID(securityID));
            relatedSymGroup.Set(new SecurityIDSource(securityIDSource));
            request.AddGroup(relatedSymGroup);
        }

        return request;
    }

    public static SecurityListRequest CreateSecurityListRequest(
        string securityReqID,
        int securityListRequestType,
        string? symbol = null,
        string? cfiCode = null
    )
    {
        var request = new SecurityListRequest();
        request.SetField(new SecurityReqID(securityReqID));
        request.SetField(new SecurityListRequestType(securityListRequestType));

        if (!string.IsNullOrEmpty(symbol)) request.SetField(new Symbol(symbol));

        if (!string.IsNullOrEmpty(cfiCode)) request.SetField(new CFICode(cfiCode));

        return request;
    }
}