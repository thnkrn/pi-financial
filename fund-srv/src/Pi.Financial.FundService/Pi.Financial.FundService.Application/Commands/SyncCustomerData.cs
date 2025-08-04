using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.Common.Features;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Bank;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Services.CustomerService;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Services.OnboardService;
using Pi.Financial.FundService.Application.Services.UserService;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.CustomerDataAggregate;
using Pi.Financial.FundService.Domain.Events;

namespace Pi.Financial.FundService.Application.Commands;

public record SyncCustomerData(string CustomerCode, Guid CorrelationId, string? BankAccountNo = null) : CorrelatedBy<Guid>;

public class SyncCustomerDataConsumer : IConsumer<SyncCustomerData>
{
    private readonly ILogger<SyncCustomerDataConsumer> _logger;
    private readonly IFundConnextService _fundConnextService;
    private readonly ICustomerService _customerService;
    private readonly IOnboardService _onboardService;
    private readonly ICustomerDataSyncHistoryRepository _customerDataSyncHistoryRepository;
    private readonly IUserService _userService;
    private readonly IFeatureService _featureService;

    public SyncCustomerDataConsumer(
        ILogger<SyncCustomerDataConsumer> logger,
        IFundConnextService fundConnextService,
        ICustomerService customerService,
        IOnboardService onboardService,
        ICustomerDataSyncHistoryRepository customerDataSyncHistoryRepository,
        IUserService userServiceV2,
        IFeatureService featureService
        )
    {
        _logger = logger;
        _fundConnextService = fundConnextService;
        _customerService = customerService;
        _onboardService = onboardService;
        _customerDataSyncHistoryRepository = customerDataSyncHistoryRepository;
        _userService = userServiceV2;
        _featureService = featureService;
    }

    public async Task Consume(ConsumeContext<SyncCustomerData> context)
    {
        _logger.LogInformation("Updating fund account {@UpdateFundAccount}", context.Message);
        CustomerDataSyncHistory history = await GetHistory(context);

        if (history.AccountUpdateSuccess == true) // TODO: change to history.IsAllSuccess when profile360 is ready
        {
            _logger.LogInformation("Customer data already synced successfully for correlation id: {CorrelationId}", context.Message.CorrelationId);
            return;
        }

        try
        {
            string accountId = $"{context.Message.CustomerCode}-M";
            var customerAccountDetail = await _fundConnextService.GetCustomerAccountByAccountNoAsync(accountId, context.CancellationToken) ?? throw new InvalidDataException($"Customer account not found for account id: {accountId}");
            switch (customerAccountDetail.InvestorType)
            {
                case InvestorType.INDIVIDUAL:
                    await SyncIndividualAccount(context, history);
                    break;
                case InvestorType.JURISTIC:
                    await SyncJuristicAccount(context, history, customerAccountDetail.JuristicNumber ?? string.Empty);
                    break;
                case InvestorType.JOINT:
                case InvestorType.BYWHOM:
                case InvestorType.FORWHOM:
                    _logger.LogInformation("Not support sync customer for {InvestorType} investor type", customerAccountDetail.InvestorType);
                    break;
            }
            await context.Publish(new CustomerDataSynced(context.Message.CustomerCode, context.Message.CorrelationId));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to sync customer data for customer code: {CustomerCode}", context.Message.CustomerCode);
            history.MarkAllAsFailed(e.Message);
        }
        finally
        {
            _customerDataSyncHistoryRepository.Update(history);
            await _customerDataSyncHistoryRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
        }
    }

    private async Task SyncIndividualAccount(ConsumeContext<SyncCustomerData> context, CustomerDataSyncHistory history)
    {
        var customerInfo = await _customerService.GetCustomerInfoForSyncFundCustomerAccount(context.Message.CustomerCode);
        var fundConnextCustomerProfile = await _fundConnextService.GetCustomerProfileAndAccount(customerInfo.CardNumber, customerInfo.PassportCountry?.ToDescriptionString(), context.CancellationToken);
        if (fundConnextCustomerProfile == null)
        {
            history.MarkAllAsFailed($"Not found fund connext customer profile with customer code: {context.Message.CustomerCode}");
            await _customerDataSyncHistoryRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
            return;
        }

        var fundTradingAccount = await _onboardService.GetMutualFundTradingAccountByCustCodeAsync(context.Message.CustomerCode, context.CancellationToken);
        if (fundTradingAccount == null)
        {
            history.MarkAllAsFailed($"Not found fund trading account with customer code: {context.Message.CustomerCode}");
            await _customerDataSyncHistoryRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
            return;
        }

        AtsBankAccounts atsBankAccounts = await GetAtsBankAccounts(context.Message.CustomerCode, false,
            context.Message.BankAccountNo, context.CancellationToken);

        var investmentObjectiveKeys = customerInfo.InvestmentObjective.Select(x => x.Key.ToString()).ToList();
        var updateCustomerAccount = new UpdateCustomerAccount(
            customerInfo.IdentificationCardType.ToDescriptionString(),
            customerInfo.PassportCountry?.ToString() ?? string.Empty,
            customerInfo.CardNumber,
            $"{context.Message.CustomerCode}-M",
            fundTradingAccount.SaleLicense,
            fundConnextCustomerProfile.ApplicationDate,
            customerInfo.MailingAddressOption.ToDescriptionString(),
            customerInfo.MailingMethod.ToDescriptionString(),
            string.Join(", ", investmentObjectiveKeys!),
            InvestmentObjectiveOther: investmentObjectiveKeys.FirstOrDefault(x => x == InvestmentObjective.PleaseSpecify.ToString()) is null ? string.Empty : "-",
            atsBankAccounts.RedemptionBankAccounts,
            atsBankAccounts.SubscriptionBankAccounts.Select(x =>
            {
                string? finnetCustomerNo = x.BankCode switch
                {
                    "002" => customerInfo.CardNumber, // BBL
                    "004" => "ADKSPYR", // KBANK
                    _ => x.FinnetCustomerNo
                };
                return x with { FinnetCustomerNo = finnetCustomerNo };
            }).ToList(),
            customerInfo.Approved);

        try
        {
            await _fundConnextService.UpdateIndivialAccountAsync(updateCustomerAccount, context.CancellationToken);
            history.MarkAccountUpdateAsSuccess();

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update fund account for customer code: {CustomerCode}", context.Message.CustomerCode);
            history.MarkAccountUpdateAsFailed(e.Message);
        }

        // TODO: comment out this block as we can't update customer info in fund connext now, wait for profile360
        // try
        // {
        //     var crs = GetCrs(fundConnextCustomerProfile);
        //     await _fundConnextService.UpdateIndividualCustomerV5(customerInfo, crs, fundConnextCustomerProfile.NdidRequestId, context.CancellationToken);
        //     history.MarkProfileUpdateAsSuccess();
        // }
        // catch (Exception e)
        // {
        //     _logger.LogError(e, "Failed to update fund profile for customer code: {CustomerCode}", context.Message.CustomerCode);
        //     history.MarkProfileUpdateAsFailed(e.Message);
        // }
    }

    private async Task SyncJuristicAccount(ConsumeContext<SyncCustomerData> context, CustomerDataSyncHistory history, string juristicNumber)
    {
        var fundConnextJuristicCustomerProfile = await _fundConnextService.GetJuristicCustomerProfileAndAccount(juristicNumber, context.CancellationToken);
        if (fundConnextJuristicCustomerProfile == null)
        {
            history.MarkAllAsFailed($"Not found fund connext juristic customer profile with customer code: {context.Message.CustomerCode}");
            await _customerDataSyncHistoryRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
            return;
        }

        string accountId = $"{context.Message.CustomerCode}-M";
        var fundTradingAccount =
            fundConnextJuristicCustomerProfile.Accounts.FirstOrDefault(x => x.AccountId == accountId);
        if (fundTradingAccount == null)
        {
            history.MarkAllAsFailed($"Not found fund trading account with customer code: {context.Message.CustomerCode}");
            await _customerDataSyncHistoryRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
            return;
        }

        AtsBankAccounts atsBankAccounts = await GetAtsBankAccounts(context.Message.CustomerCode, true,
            context.Message.BankAccountNo, context.CancellationToken);

        var updateCustomerJuristicAccount = new UpdateCustomerJuristicAccount(
            juristicNumber,
            accountId,
            fundTradingAccount.IcLicense,
            fundTradingAccount.AccountOpenDate,
            fundTradingAccount.MailingAddressSameAsFlag,
            fundTradingAccount.MailingMethod,
            fundTradingAccount.InvestmentObjective,
            fundTradingAccount.InvestmentObjectiveOther,
            atsBankAccounts.RedemptionBankAccounts,
            atsBankAccounts.SubscriptionBankAccounts.Select(x =>
            {
                string? finnetCustomerNo = x.BankCode switch
                {
                    "002" => fundConnextJuristicCustomerProfile.TaxNumber, // BBL
                    "004" => "ADKSPYR", // KBANK
                    _ => x.FinnetCustomerNo
                };
                return x with { FinnetCustomerNo = finnetCustomerNo };
            }).ToList(),
            true);

        try
        {
            await _fundConnextService.UpdateJuristicAccountAsync(updateCustomerJuristicAccount, context.CancellationToken);
            history.MarkAccountUpdateAsSuccess();

        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update fund account for customer code: {CustomerCode}", context.Message.CustomerCode);
            history.MarkAccountUpdateAsFailed(e.Message);
        }
    }

    private async Task<CustomerDataSyncHistory> GetHistory(ConsumeContext<SyncCustomerData> context)
    {
        var history = await _customerDataSyncHistoryRepository.GetByCorrelationIdAsync(context.Message.CorrelationId, context.CancellationToken);
        if (history == null)
        {
            history = new CustomerDataSyncHistory(context.Message.CorrelationId, context.Message.CustomerCode);
            await _customerDataSyncHistoryRepository.AddAsync(history, context.CancellationToken);
            await _customerDataSyncHistoryRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
        }

        return history;
    }

    private async Task<AtsBankAccounts> GetAtsBankAccounts(string customerCode, bool isJuristicCustomer, string? bankAccountNo, CancellationToken cancellationToken)
    {
        AtsBankAccounts atsBankAccounts;
        if (_featureService.IsOn(Features.SsoPhase3) || isJuristicCustomer)
        {
            atsBankAccounts = await _userService.GetAtsBankAccountsByCustomerCodeAsync(customerCode, cancellationToken);
        }
        else
        {
            atsBankAccounts = await _onboardService.GetATSBankAccountsByCustomerCodeAsync(customerCode, cancellationToken);
        }

        if (!string.IsNullOrEmpty(bankAccountNo))
        {
            atsBankAccounts = new AtsBankAccounts(
                atsBankAccounts.RedemptionBankAccounts.Where(x => x.BankAccountNo == bankAccountNo).Select(x => x with { IsDefault = true }).ToList(),
                atsBankAccounts.SubscriptionBankAccounts.Where(x => x.BankAccountNo == bankAccountNo).Select(x => x with { IsDefault = true }).ToList());
        }

        return atsBankAccounts;
    }

    // private static Crs GetCrs(IndividualInvestorV5Response fundConnextCustomerProfile)
    // {
    //     var crsDetails = fundConnextCustomerProfile.CrsDetails.Select(x => new CrsDetail(x.CountryOfTaxResidence, x.Tin, ConvertCrsReason(x.Reason), x.ReasonDesc)).ToList();
    //     var crs = new Crs(fundConnextCustomerProfile.CrsPlaceOfBirthCountry, fundConnextCustomerProfile.CrsPlaceOfBirthCity, fundConnextCustomerProfile.CrsTaxResidenceInCountriesOtherThanTheUS == true, crsDetails, fundConnextCustomerProfile.CrsDeclarationDate);
    //     return crs;
    // }

    // private static CrsReason? ConvertCrsReason(Client.FundConnext.Model.IndividualInvestorV5ResponseCrsDetailsInner.ReasonEnum? crsReason)
    // {
    //     return crsReason switch
    //     {
    //         IndividualInvestorV5ResponseCrsDetailsInner.ReasonEnum.A => CrsReason.A,
    //         IndividualInvestorV5ResponseCrsDetailsInner.ReasonEnum.B => CrsReason.B,
    //         IndividualInvestorV5ResponseCrsDetailsInner.ReasonEnum.C => CrsReason.C,
    //         _ => null,
    //     };
    // }
}

