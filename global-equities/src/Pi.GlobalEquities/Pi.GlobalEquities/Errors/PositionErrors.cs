namespace Pi.GlobalEquities.Errors;

public static class PositionErrors
{
    public static readonly Error InsufficientHoldings = new Error(
        ErrorCode.GEOE102, "Quantity exceeds available holdings");
}
