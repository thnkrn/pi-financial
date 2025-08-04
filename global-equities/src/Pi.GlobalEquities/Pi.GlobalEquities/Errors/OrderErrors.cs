namespace Pi.GlobalEquities.Errors;

public static class OrderErrors
{
    public static readonly Error UserAccessDenied = new Error(
        ErrorCode.GEOE103, "Order does not belong to user");

    public static readonly Error MarketClose = new Error(
        ErrorCode.GEOE104, "The order cannot be edited or canceled when the market is closed.");

    public static readonly Error OrderNotFound = new Error(
        ErrorCode.GEOE105, "The requested orderId (refId) cannot be found, is already canceled, or rejected.");
}
