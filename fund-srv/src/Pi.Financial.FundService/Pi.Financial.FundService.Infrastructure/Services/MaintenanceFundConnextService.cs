using Pi.Common.Features;
using Pi.Financial.Client.FundConnext.Model;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Domain.AccountOpening.Events.Models;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using FundOrder = Pi.Financial.FundService.Application.Models.FundOrder;

namespace Pi.Financial.FundService.Infrastructure.Services;

public class MaintenanceFundConnextService(IFeatureService featureService, FundConnextService fundConnextService) : IFundConnextService
{
    public async Task CreateIndividualCustomerV5(CustomerInfo fundCustomerInfo, Crs? crs, string? ndidRequestId,
        CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        await fundConnextService.CreateIndividualCustomerV5(fundCustomerInfo, crs, ndidRequestId, cancellationToken);
    }

    public async Task CreateIndividualCustomerV6(CustomerInfo fundCustomerInfo, Crs? crs, string? ndidRequestId,
        string identityVerificationDateTime, string dopaVerificationDateTime,
        CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        await fundConnextService.CreateIndividualCustomerV6(fundCustomerInfo, crs, ndidRequestId,
            identityVerificationDateTime, dopaVerificationDateTime, cancellationToken);
    }

    public async Task CreateIndividualAccount(CustomerInfo customerInfo, CustomerAccount customerAccount,
        bool isSendSubscriptionBankAccount = false)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        await fundConnextService.CreateIndividualAccount(customerInfo, customerAccount, isSendSubscriptionBankAccount);
    }

    public async Task UploadIndividualCustomerDocuments(string ticketId, IdentificationCardType idCardType, string idCardNumber,
        Document document)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        await fundConnextService.UploadIndividualCustomerDocuments(ticketId, idCardType, idCardNumber, document);
    }

    public async Task UploadIndividualAccountDocuments(string ticketId, IdentificationCardType idCardType, string idCardNumber,
        string accountId, Document document)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        await fundConnextService.UploadIndividualAccountDocuments(ticketId, idCardType, idCardNumber, accountId, document);
    }

    public async Task UpdateIndivialAccountAsync(UpdateCustomerAccount updateCustomerAccount,
        CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        await fundConnextService.UpdateIndivialAccountAsync(updateCustomerAccount, cancellationToken);
    }

    public async Task UpdateJuristicAccountAsync(UpdateCustomerJuristicAccount updateCustomerJuristicAccount,
        CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        await fundConnextService.UpdateJuristicAccountAsync(updateCustomerJuristicAccount, cancellationToken);
    }

    public async Task<IndividualInvestorV5Response?> GetCustomerProfileAndAccount(string cardId, string? passportCountry = null,
        CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceRead))
        {
            return null;
        }

        return await fundConnextService.GetCustomerProfileAndAccount(cardId, passportCountry, cancellationToken);
    }

    public async Task<JuristicInvestorV5Response?> GetJuristicCustomerProfileAndAccount(string juristicNumber, CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceRead))
        {
            return null;
        }

        return await fundConnextService.GetJuristicCustomerProfileAndAccount(juristicNumber, cancellationToken);
    }

    public async Task<List<FundAssetResponse>> GetAccountBalanceAsync(string accountNo, CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceRead))
        {
            return [];
        }

        return await fundConnextService.GetAccountBalanceAsync(accountNo, cancellationToken);
    }

    public async Task<CustomerAccountDetail?> GetCustomerAccountByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceRead))
        {
            return null;
        }

        return await fundConnextService.GetCustomerAccountByAccountNoAsync(accountNo, cancellationToken);
    }

    public async Task<List<FundOrder>> GetAccountFundOrdersAsync(string accountNo, DateOnly effectiveDateFrom, DateOnly effectiveDateTo,
        CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceRead))
        {
            return [];
        }

        return await fundConnextService.GetAccountFundOrdersAsync(accountNo, effectiveDateFrom, effectiveDateTo, cancellationToken);
    }

    public async Task<List<FundOrder>> GetFundOrdersAsync(DateOnly effectiveDate, FundOrderStatus? status, CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceRead))
        {
            return [];
        }

        return await fundConnextService.GetFundOrdersAsync(effectiveDate, status, cancellationToken);
    }

    public async Task<List<FundOrder>> GetFundOrdersByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceRead))
        {
            return [];
        }

        return await fundConnextService.GetFundOrdersByOrderNoAsync(orderNo, cancellationToken);
    }

    public async Task<List<Client.FundConnext.Model.FundOrder>> GetRawFundOrdersAsync(DateOnly effectiveDate, CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceRead))
        {
            return [];
        }

        return await fundConnextService.GetRawFundOrdersAsync(effectiveDate, cancellationToken);
    }

    public async Task<CreateSubscriptionResponse> CreateSubscriptionAsync(CreateSubscriptionRequest subscriptionRequest,
        CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        return await fundConnextService.CreateSubscriptionAsync(subscriptionRequest, cancellationToken);
    }

    public async Task<CreateSwitchResponse> CreateSwitchAsync(CreateSwitchRequest switchRequest, CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        return await fundConnextService.CreateSwitchAsync(switchRequest, cancellationToken);
    }

    public async Task<CreateRedemptionResponse> CreateRedemptionAsync(CreateRedemptionRequest redemptionRequest, CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        return await fundConnextService.CreateRedemptionAsync(redemptionRequest, cancellationToken);
    }

    public async Task<CancelFundOrderResponse> CancelRedemptionOrderAsync(CancelOrderRequest cancelRequest, CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        return await fundConnextService.CancelRedemptionOrderAsync(cancelRequest, cancellationToken);
    }

    public async Task<CancelFundOrderResponse> CancelSwitchingOrderAsync(CancelOrderRequest cancelRequest, CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        return await fundConnextService.CancelSwitchingOrderAsync(cancelRequest, cancellationToken);
    }

    public async Task<CancelFundOrderResponse> CancelSubscriptionOrderAsync(CancelOrderRequest cancelRequest, CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        return await fundConnextService.CancelSubscriptionOrderAsync(cancelRequest, cancellationToken);
    }

    public async Task UpdateIndividualCustomerV5(CustomerInfo fundCustomerInfo, Crs? crs, string? ndidRequestId,
        CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        await fundConnextService.UpdateIndividualCustomerV5(fundCustomerInfo, crs, ndidRequestId, cancellationToken);
    }

    public async Task UpdateIndividualCustomerV6(CustomerInfo fundCustomerInfo, Crs? crs, string? ndidRequestId,
        string identityVerificationDateTime, string dopaVerificationDateTime,
        CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        await fundConnextService.UpdateIndividualCustomerV6(fundCustomerInfo, crs, ndidRequestId, identityVerificationDateTime, dopaVerificationDateTime, cancellationToken);
    }

    public async Task DebugUpdateIndividualCustomerV5(CustomerAccountCreateRequestV5 customerAccountCreateRequestV5,
        CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceAction))
        {
            throw new FundOrderException(FundOrderErrorCode.FOE218);
        }

        await fundConnextService.DebugUpdateIndividualCustomerV5(customerAccountCreateRequestV5, cancellationToken);
    }

    public async Task<CustomerAccount?> GetCustomerAccountAsync(string cardId, string? passportCountry = null,
        CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceRead))
        {
            return null;
        }

        return await fundConnextService.GetCustomerAccountAsync(cardId, passportCountry, cancellationToken);
    }

    public async Task<bool> CheckAccountExist(string accountNo, CancellationToken cancellationToken = default)
    {
        if (featureService.IsOn(Features.FundConnextMaintenanceRead))
        {
            return false;
        }

        return await fundConnextService.CheckAccountExist(accountNo, cancellationToken: cancellationToken);
    }
}
