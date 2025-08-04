namespace Pi.GlobalEquities.Errors;

public static class AccountErrors
{
    public static readonly Error InsufficientBalance = new Error(
        ErrorCode.GEOE101, "Insufficient account balance");

    public static readonly Error NotExist = new Error(
        ErrorCode.GEOE103, "Account doesn't exist");

    public static readonly Error NotAllowToBuy = new Error(
        ErrorCode.GEOE106, "This account is not allowed to place or modify buy orders");

    public static readonly Error NotAllowToSell = new Error(
            ErrorCode.GEOE106, "This account is not allowed to place or modify sell orders");
}
