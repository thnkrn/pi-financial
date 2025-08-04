using Microsoft.EntityFrameworkCore;
using Pi.Common.Database;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;

namespace Pi.WalletService.Infrastructure.Services;

public class BankInfoService : IBankInfoService
{
    private readonly CommonDbContext _commonDbContext;

    public BankInfoService(CommonDbContext commonDbContext)
    {
        _commonDbContext = commonDbContext;
    }

    public async Task<BankInfo?> GetByBankCode(string code)
    {
        var bankInfo = await _commonDbContext.BankInfos.Where(b => b.Code == code).AsNoTracking().SingleOrDefaultAsync();

        return bankInfo != null
            ? new BankInfo(bankInfo.Name, bankInfo.ShortName, bankInfo.Code, bankInfo.IconUrl)
            : null;
    }
}