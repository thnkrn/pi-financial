namespace Pi.WalletService.Application.Services.Bank;

public record BankInfo(string Name, string ShortName, string Code, string IconUrl);

public interface IBankInfoService
{
    Task<BankInfo?> GetByBankCode(string code);
}