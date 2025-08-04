using System.Text.Json.Serialization;
namespace Pi.User.Application.Models.BankAccount;

public class BankAccountDto
{
    public BankAccountDto(
        string accountNo,
        string accountName,
        string bankCode,
        string bankBranchCode
    )
    {
        AccountNo = accountNo;
        AccountName = accountName;
        BankCode = bankCode;
        BankBranchCode = bankBranchCode;
    }

    public string AccountNo { get; init; }

    public string AccountName { get; init; }

    public string BankCode { get; init; }

    public string BankBranchCode { get; init; }
}
