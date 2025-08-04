namespace Pi.StructureNotes.Domain.Models;

public class FailedAccountInfo
{
    public FailedAccountInfo(AccountInfo accountInfo, string error)
    {
        AccountId = accountInfo.AccountId;
        AccountNo = accountInfo.AccountNo;
        CustCode = accountInfo.CustCode;
        Error = error;
    }

    public string AccountId { get; init; }
    public string AccountNo { get; init; }
    public string CustCode { get; init; }
    public string Error { get; init; }
}
