using Pi.Financial.Client.FundConnext.Model;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Domain.AccountOpening.Events.Models;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using FundOrder = Pi.Financial.FundService.Application.Models.FundOrder;
using RawFundOrder = Pi.Financial.Client.FundConnext.Model.FundOrder;

namespace Pi.Financial.FundService.Application.Services.FundConnextService
{
    public interface IFundConnextService
    {
        Task CreateIndividualCustomerV5(CustomerInfo fundCustomerInfo, Crs? crs, string? ndidRequestId, CancellationToken cancellationToken = default);
        Task CreateIndividualCustomerV6(CustomerInfo fundCustomerInfo, Crs? crs, string? ndidRequestId, string identityVerificationDateTime, string dopaVerificationDateTime, CancellationToken cancellationToken = default);

        Task CreateIndividualAccount(CustomerInfo customerInfo, CustomerAccount customerAccount, bool isSendSubscriptionBankAccount = false);

        Task UploadIndividualCustomerDocuments(
            string ticketId,
            IdentificationCardType idCardType,
            string idCardNumber,
            Document document);

        Task UploadIndividualAccountDocuments(
            string ticketId,
            IdentificationCardType idCardType,
            string idCardNumber,
            string accountId,
            Document document);

        Task UpdateIndivialAccountAsync(UpdateCustomerAccount updateCustomerAccount,
            CancellationToken cancellationToken = default);
        Task UpdateJuristicAccountAsync(UpdateCustomerJuristicAccount updateCustomerJuristicAccount,
            CancellationToken cancellationToken = default);

        Task<IndividualInvestorV5Response?> GetCustomerProfileAndAccount(string cardId, string? passportCountry = null, CancellationToken cancellationToken = default);
        Task<JuristicInvestorV5Response?> GetJuristicCustomerProfileAndAccount(string juristicNumber, CancellationToken cancellationToken = default);
        Task<List<FundAssetResponse>> GetAccountBalanceAsync(string accountNo, CancellationToken cancellationToken = default);
        Task<CustomerAccountDetail?> GetCustomerAccountByAccountNoAsync(
            string accountNo,
            CancellationToken cancellationToken = default);
        Task<List<FundOrder>> GetAccountFundOrdersAsync(string accountNo, DateOnly effectiveDateFrom, DateOnly effectiveDateTo, CancellationToken cancellationToken = default);
        Task<List<FundOrder>> GetFundOrdersAsync(DateOnly effectiveDate, FundOrderStatus? status, CancellationToken cancellationToken = default);
        Task<List<FundOrder>> GetFundOrdersByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default);
        Task<List<RawFundOrder>> GetRawFundOrdersAsync(DateOnly effectiveDate, CancellationToken cancellationToken = default);
        Task<CreateSubscriptionResponse> CreateSubscriptionAsync(CreateSubscriptionRequest subscriptionRequest, CancellationToken cancellationToken = default);
        Task<CreateSwitchResponse> CreateSwitchAsync(CreateSwitchRequest switchRequest, CancellationToken cancellationToken = default);
        Task<CreateRedemptionResponse> CreateRedemptionAsync(CreateRedemptionRequest redemptionRequest, CancellationToken cancellationToken = default);
        Task<CancelFundOrderResponse> CancelRedemptionOrderAsync(CancelOrderRequest cancelRequest, CancellationToken cancellationToken = default);
        Task<CancelFundOrderResponse> CancelSwitchingOrderAsync(CancelOrderRequest cancelRequest, CancellationToken cancellationToken = default);
        Task<CancelFundOrderResponse> CancelSubscriptionOrderAsync(CancelOrderRequest cancelRequest, CancellationToken cancellationToken = default);
        Task UpdateIndividualCustomerV5(CustomerInfo fundCustomerInfo, Crs? crs, string? ndidRequestId, CancellationToken cancellationToken = default);
        Task UpdateIndividualCustomerV6(CustomerInfo fundCustomerInfo, Crs? crs, string? ndidRequestId, string identityVerificationDateTime, string dopaVerificationDateTime, CancellationToken cancellationToken = default);
        Task DebugUpdateIndividualCustomerV5(CustomerAccountCreateRequestV5 customerAccountCreateRequestV5,
            CancellationToken cancellationToken = default);
        Task<CustomerAccount?> GetCustomerAccountAsync(string cardId, string? passportCountry = null, CancellationToken cancellationToken = default);
        Task<bool> CheckAccountExist(string accountNo, CancellationToken cancellationToken = default);
    }

    public class FundCustomerDuplicatedException : Exception
    {
    }

    public class RequiredFieldMissing : Exception
    {
        public string Msg => base.Message;

        public RequiredFieldMissing(string? msg) : base(msg)
        {
        }
    }

    public class InvalidFormat : Exception
    {
        public string Field => base.Message;

        public InvalidFormat(string? field) : base(field)
        {
        }
    }

    public class UploadDocumentFailed : Exception
    {
        public string DocumentNames => base.Message;

        public UploadDocumentFailed(string? documentNames) : base(documentNames)
        {
        }
    }

    public class DownloadDocumentFailed : Exception
    {
        public string DocumentNames => base.Message;

        public DownloadDocumentFailed(string? documentNames) : base(documentNames)
        {
        }
    }
}
