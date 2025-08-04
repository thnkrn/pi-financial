using System.Globalization;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Pi.Common.ExtensionMethods;
using Pi.Common.Features;
using Pi.Financial.Client.FundConnext.Api;
using Pi.Financial.Client.FundConnext.Client;
using Pi.Financial.Client.FundConnext.Model;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Application.Models;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Enums;
using Pi.Financial.FundService.Application.Models.Trading;
using Pi.Financial.FundService.Application.Services.FundConnextService;
using Pi.Financial.FundService.Application.Utils;
using Pi.Financial.FundService.Domain.AccountOpening.Events.Models;
using Pi.Financial.FundService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;
using Pi.Financial.FundService.Infrastructure.Factories;
using Pi.Financial.FundService.Infrastructure.Models;
using Serilog;
using FundOrder = Pi.Financial.FundService.Application.Models.FundOrder;
using RawFundOrder = Pi.Financial.Client.FundConnext.Model.FundOrder;

namespace Pi.Financial.FundService.Infrastructure.Services
{
    public class FundConnextService : IFundConnextService
    {
        private readonly IFundConnextApi _fundConnextApi;
        private readonly IAccountOpeningV5Api _accountOpeningV5Api;
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly TimeSpan _cacheTimeSpan;

        public const string DateFormatPattern = "yyyyMMdd";
        public const string DateTimeFormatPattern = "yyyyMMddHHmmss";

        public FundConnextService(
            IFundConnextApi fundConnextApi,
            IAccountOpeningV5Api accountOpeningV5Api,
            IDistributedCache cache,
            IConfiguration configuration,
            ILogger logger)
        {
            _fundConnextApi = fundConnextApi;
            _accountOpeningV5Api = accountOpeningV5Api;
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
            _cacheTimeSpan = TimeSpan.FromMinutes(configuration.GetValue("FundConnext:TokenCacheExpiration", 15));
        }

        public async Task<IndividualInvestorV5Response?> GetCustomerProfileAndAccount(string cardId, string? passportCountry = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await GetAuthenticationToken();
                var resp = await _accountOpeningV5Api.ApiCustomerIndividualInvestorProfileV5GetAsync(cardNumber: cardId, passportCountry, xAuthToken: authToken, cancellationToken);
                return resp;
            }
            catch (ApiException e) when (e.ErrorCode == 422)
            {
                return null;
            }
        }

        public async Task<JuristicInvestorV5Response?> GetJuristicCustomerProfileAndAccount(string juristicNumber,
            CancellationToken cancellationToken = default)
        {
            try
            {
                string authToken = await GetAuthenticationToken();
                var resp = await _fundConnextApi.ApiCustomerJuristicInvestorProfileV5GetAsync(juristicNumber: juristicNumber, xAuthToken: authToken, cancellationToken);
                return resp;
            }
            catch (ApiException e) when (e.ErrorCode == 422)
            {
                return null;
            }
        }

        public async Task<List<FundAssetResponse>> GetAccountBalanceAsync(string accountNo, CancellationToken cancellationToken = default)
        {
            var authToken = await GetAuthenticationToken();
            try
            {
                var accountBalance = await _fundConnextApi.ApiAccountBalancesGetAsync(authToken, accountNo, cancellationToken);

                return accountBalance.Result.Select(q => new FundAssetResponse
                {
                    UnitholderId = q.UnitholderId,
                    FundCode = q.FundCode,
                    Unit = q.Unit,
                    Amount = q.Amount,
                    RemainUnit = q.RemainUnit,
                    RemainAmount = q.RemainAmount,
                    PendingAmount = q.PendingAmount,
                    PendingUnit = q.PendingUnit,
                    AvgCost = q.AvgCost ?? 0,
                    Nav = q.Nav,
                    NavDate = DateOnly.ParseExact(q.NavDate, DateFormatPattern, CultureInfo.InvariantCulture)
                }).ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Request to get account balances failed");
                return new List<FundAssetResponse>();
            }
        }

        public async Task<List<FundOrder>> GetAccountFundOrdersAsync(string accountNo, DateOnly effectiveDateFrom, DateOnly effectiveDateTo,
            CancellationToken cancellationToken = default)
        {
            var authToken = await GetAuthenticationToken();
            try
            {
                var next = true;
                var page = 1;
                var fundOrders = new List<FundOrder>();
                while (next)
                {
                    var response = await _fundConnextApi.ApiAccountFundOrdersV2GetAsync(authToken,
                        accountNo,
                        effectiveDateFrom.ToString(DateFormatPattern, CultureInfo.InvariantCulture),
                        effectiveDateTo.ToString(DateFormatPattern, CultureInfo.InvariantCulture),
                        page: page.ToString(),
                        cancellationToken: cancellationToken);
                    fundOrders.AddRange(response.Result.Select(ApplicationFactory.NewFundOrder));

                    next = response.Next;
                    page += 1;
                }

                return fundOrders.ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Request to get account fund order failed");
                return new List<FundOrder>();
            }
        }

        public async Task<List<FundOrder>> GetFundOrdersAsync(DateOnly effectiveDate, FundOrderStatus? status, CancellationToken cancellationToken = default)
        {
            var authToken = await GetAuthenticationToken();
            var parsed = Enum.TryParse<Client.FundConnext.Model.FundOrder.StatusEnum>(status?.ToString().ToUpper(), out var orderStatus);
            if (status != null && !parsed)
            {
                _logger.Error("Can\'t parse {Status} status into fundconnext status", status);
                return new List<FundOrder>();
            }

            try
            {
                var next = true;
                var page = 1;
                var fundOrders = new List<FundOrder>();
                while (next)
                {
                    var response = await _fundConnextApi.ApiFundOrdersV2GetAsync(authToken,
                        effectiveDate.ToString(DateFormatPattern, CultureInfo.InvariantCulture),
                        status: status != null ? orderStatus.ToString() : null, // Should we map into fundconnext status?
                        page: page.ToString(),
                        cancellationToken: cancellationToken);
                    fundOrders.AddRange(response.Result.Select(ApplicationFactory.NewFundOrder));

                    next = response.Next;
                    page += 1;
                }

                return fundOrders.ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Request to get account fund order failed");
                return new List<FundOrder>();
            }
        }

        public async Task<List<FundOrder>> GetFundOrdersByOrderNoAsync(string orderNo, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await GetAuthenticationToken();
                var response = await _fundConnextApi.ApiFundOrdersSaOrderReferenceNoGetAsync(authToken, orderNo, cancellationToken);

                return response.Result.Select(ApplicationFactory.NewFundOrder).ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Request to get account fund order failed");
                return new List<FundOrder>();
            }
        }

        public async Task<List<RawFundOrder>> GetRawFundOrdersAsync(DateOnly effectiveDate, CancellationToken cancellationToken = default)
        {
            var authToken = await GetAuthenticationToken();

            try
            {
                var next = true;
                var page = 1;
                var fundOrders = new List<RawFundOrder>();
                while (next)
                {
                    var response = await _fundConnextApi.ApiFundOrdersV2GetAsync(authToken,
                        effectiveDate.ToString(DateFormatPattern, CultureInfo.InvariantCulture),
                        page: page.ToString(),
                        cancellationToken: cancellationToken);
                    fundOrders.AddRange(response.Result);

                    next = response.Next;
                    page += 1;
                }

                return fundOrders.ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Request to get raw fund order failed");
                return new List<RawFundOrder>();
            }
        }

        public async Task<CustomerAccountDetail?> GetCustomerAccountByAccountNoAsync(string accountNo, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await GetAuthenticationToken();
                var response = await _fundConnextApi.ApiCustomerAccountGetAsync(authToken, accountNo, cancellationToken);
                var account = response.Accounts.Find(account => account.AccountId == accountNo);

                return account == null ? null : ApplicationFactory.NewCustomerAccount(account, response.InvestorClass, response.InvestorType, response.JuristicNumber);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Failed to get customer account from FundConnext");
                throw;
            }
        }

        public async Task<CreateSubscriptionResponse> CreateSubscriptionAsync(CreateSubscriptionRequest subscriptionRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await GetAuthenticationToken();
                var response = await _fundConnextApi.ApiSubscriptionsV2PostAsync(
                    authToken,
                    new ApiSubscriptionsV2PostRequest(
                        subscriptionRequest.SaOrderReferenceNo,
                        subscriptionRequest.TransactionDateTime.ToString(DateTimeFormatPattern,
                            CultureInfo.InvariantCulture),
                        subscriptionRequest.SaCode,
                        subscriptionRequest.AccountId,
                        subscriptionRequest.UnitholderId,
                        subscriptionRequest.FundCode,
                        subscriptionRequest.Amount,
                        subscriptionRequest.EffectiveDate.ToString(DateFormatPattern, CultureInfo.InvariantCulture),
                        subscriptionRequest.OverrideRiskProfile
                            ? ApiSubscriptionsV2PostRequest.OverrideRiskProfileEnum.Y
                            : ApiSubscriptionsV2PostRequest.OverrideRiskProfileEnum.N,
                        subscriptionRequest.OverrideFxRisk
                            ? ApiSubscriptionsV2PostRequest.OverrideFxRiskEnum.Y
                            : ApiSubscriptionsV2PostRequest.OverrideFxRiskEnum.N,
                        FundconnextFactory.NewSubscriptionPaymentTypeEnum(subscriptionRequest.PaymentType),
                        bankCode: subscriptionRequest.BankCode,
                        bankAccount: subscriptionRequest.BankAccount,
                        channel: FundconnextFactory.NewSubscriptionChannelEnum(subscriptionRequest.Channel),
                        icLicense: subscriptionRequest.SaleLicense,
                        forceEntry: subscriptionRequest.ForceEntry
                            ? ApiSubscriptionsV2PostRequest.ForceEntryEnum.Y
                            : ApiSubscriptionsV2PostRequest.ForceEntryEnum.N
                    )
                    , cancellationToken);

                return new CreateSubscriptionResponse
                {
                    TransactionId = response.TransactionId,
                    SaOrderReferenceNo = response.SaOrderReferenceNo,
                    UnitHolderId = response.UnitholderId
                };
            }
            catch (ApiException e)
            {
                _logger.Error(e, "Request to create subscription fund order failed");
                throw ApiExceptionHandling(e, OrderSide.Buy);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Request to create subscription fund order failed");
                throw;
            }
        }

        public async Task<CreateSwitchResponse> CreateSwitchAsync(CreateSwitchRequest switchRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await GetAuthenticationToken();
                var response = await _fundConnextApi.ApiSwitchingsPostAsync(
                    authToken,
                    new ApiSwitchingsPostRequest(
                        switchRequest.SaOrderReferenceNo,
                        switchRequest.TransactionDateTime.ToString(DateTimeFormatPattern, CultureInfo.InvariantCulture),
                        switchRequest.SaCode,
                        switchRequest.AccountId,
                        switchRequest.UnitholderId,
                        switchRequest.FundCode,
                        switchRequest.CounterFundCode,
                        switchRequest.OverrideRiskProfile ? ApiSwitchingsPostRequest.OverrideRiskProfileEnum.Y : ApiSwitchingsPostRequest.OverrideRiskProfileEnum.N,
                        switchRequest.OverrideFxRisk ? ApiSwitchingsPostRequest.OverrideFxRiskEnum.Y : ApiSwitchingsPostRequest.OverrideFxRiskEnum.N,
                        switchRequest.Amount,
                        switchRequest.Unit,
                        switchRequest.EffectiveDate.ToString(DateFormatPattern, CultureInfo.InvariantCulture),
                        FundconnextFactory.NewSwitchingChannelEnum(switchRequest.Channel),
                        null,
                        switchRequest.IcLicense,
                        switchRequest.RedemptionType.GetEnumDescription(),
                        null,
                        switchRequest.ForceEntry ? ApiSwitchingsPostRequest.ForceEntryEnum.Y : ApiSwitchingsPostRequest.ForceEntryEnum.N,
                        switchRequest.SellAllUnitFlag ? ApiSwitchingsPostRequest.SellAllUnitFlagEnum.Y : ApiSwitchingsPostRequest.SellAllUnitFlagEnum.N
                    )
                    , cancellationToken);

                return new CreateSwitchResponse
                {
                    TransactionId = response.TransactionId,
                    SaOrderReferenceNo = response.SaOrderReferenceNo,
                };
            }
            catch (ApiException e)
            {
                _logger.Error(e, "Request to create switch fund order failed");
                throw ApiExceptionHandling(e, OrderSide.Switch);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Request to create switch fund order failed");
                throw;
            }
        }

        public async Task<CreateRedemptionResponse> CreateRedemptionAsync(CreateRedemptionRequest redemptionRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await GetAuthenticationToken();
                var response = await _fundConnextApi.ApiRedemptionsPostAsync(
                    authToken,
                    new ApiRedemptionsPostRequest(
                        redemptionRequest.SaOrderReferenceNo,
                        redemptionRequest.TransactionDateTime.ToString(DateTimeFormatPattern, CultureInfo.InvariantCulture),
                        redemptionRequest.SaCode,
                        redemptionRequest.AccountId,
                        redemptionRequest.UnitholderId,
                        redemptionRequest.FundCode,
                        redemptionRequest.Amount,
                        redemptionRequest.Unit,
                        redemptionRequest.EffectiveDate.ToString(DateFormatPattern, CultureInfo.InvariantCulture),
                        redemptionRequest.BankCode,
                        redemptionRequest.BankAccount,
                        FundconnextFactory.NewRedemptionChannelEnum(redemptionRequest.Channel),
                        redemptionRequest.IcLicense,
                        redemptionRequest.RedemptionType.GetEnumDescription(),
                        redemptionRequest.ForceEntry ? ApiRedemptionsPostRequest.ForceEntryEnum.Y : ApiRedemptionsPostRequest.ForceEntryEnum.N,
                        redemptionRequest.SellAllUnitFlag ? ApiRedemptionsPostRequest.SellAllUnitFlagEnum.Y : ApiRedemptionsPostRequest.SellAllUnitFlagEnum.N,
                        null,
                        FundconnextFactory.NewRedemptionPaymentTypeEnum(redemptionRequest.PaymentType)

                    )
                    , cancellationToken);

                return new CreateRedemptionResponse
                {
                    TransactionId = response.TransactionId,
                    SaOrderReferenceNo = response.SaOrderReferenceNo,
                    SettlementDate = DateOnly.ParseExact(response.SettlementDate, DateFormatPattern, CultureInfo.InvariantCulture)
                };
            }
            catch (ApiException e)
            {
                _logger.Error(e, "Request to create redemption fund order failed");
                throw ApiExceptionHandling(e, OrderSide.Sell);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Request to create redemption fund order failed");
                throw;
            }
        }

        public async Task<CancelFundOrderResponse> CancelSubscriptionOrderAsync(CancelOrderRequest cancelOrderRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await GetAuthenticationToken();
                var response = await _fundConnextApi.ApiSubscriptionsTransactionIdDeleteAsync(
                    authToken,
                    cancelOrderRequest.BrokerOrderId,
                    new ApiSubscriptionsTransactionIdDeleteRequest(
                        force: cancelOrderRequest.Force ? ApiSubscriptionsTransactionIdDeleteRequest.ForceEnum.Y : ApiSubscriptionsTransactionIdDeleteRequest.ForceEnum.N
                    )
                    , cancellationToken);

                return new CancelFundOrderResponse
                {
                    TransactionId = response.TransactionId
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "Request to cancel subscription fund order failed");
                throw;
            }
        }

        public async Task<CancelFundOrderResponse> CancelRedemptionOrderAsync(CancelOrderRequest cancelOrderRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await GetAuthenticationToken();
                var response = await _fundConnextApi.ApiRedemptionsTransactionIdDeleteAsync(
                    authToken,
                    cancelOrderRequest.BrokerOrderId,
                    new ApiSubscriptionsTransactionIdDeleteRequest(
                        force: cancelOrderRequest.Force ? ApiSubscriptionsTransactionIdDeleteRequest.ForceEnum.Y : ApiSubscriptionsTransactionIdDeleteRequest.ForceEnum.N
                    )
                    , cancellationToken);

                return new CancelFundOrderResponse
                {
                    TransactionId = response.TransactionId
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "Request to cancel redemption fund order failed");
                throw;
            }
        }

        public async Task<CancelFundOrderResponse> CancelSwitchingOrderAsync(CancelOrderRequest cancelOrderRequest, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await GetAuthenticationToken();
                var response = await _fundConnextApi.ApiSwitchingsTransactionIdDeleteAsync(
                    authToken,
                    cancelOrderRequest.BrokerOrderId,
                    new ApiSubscriptionsTransactionIdDeleteRequest(
                        force: cancelOrderRequest.Force ? ApiSubscriptionsTransactionIdDeleteRequest.ForceEnum.Y : ApiSubscriptionsTransactionIdDeleteRequest.ForceEnum.N
                    )
                    , cancellationToken);

                return new CancelFundOrderResponse
                {
                    TransactionId = response.TransactionId
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "Request to cancel switching fund order failed");
                throw;
            }
        }
        public async Task CreateIndividualAccount(CustomerInfo customerInfo, CustomerAccount customerAccount, bool isSendSubscriptionBankAccount = false)
        {
            var authToken = await GetAuthenticationToken();

            var investmentObjectiveKeys = customerInfo.InvestmentObjective.Select(x => x.Key.ToString());
            var createCustomerAccountReq = new CustomerAccountCreateRequest(
                identificationCardType: customerInfo.IdentificationCardType.ToDescriptionString(),
                cardNumber: customerInfo.CardNumber,
                accountId: customerAccount.AccountId,
                icLicense: customerAccount.SaleLicense,
                accountOpenDate: customerAccount.OpenDate.ToString(DateFormatPattern, CultureInfo.InvariantCulture),
                mailingAddressSameAsFlag: customerInfo.CurrentAddressSameAsFlag == CurrentAddressSameAsFlag.True
                    ? "IdDocument"
                    : "Current",
                mailingMethod: "Email",
                investmentObjective: String.Join(", ", investmentObjectiveKeys),
                investmentObjectiveOther: investmentObjectiveKeys.Contains(InvestmentObjective.PleaseSpecify.ToString()) ? "-" : null, // No investment object other from Freewill
                redemptionBankAccounts:
                customerAccount.RedemptionBankAccount.Select(b =>
                    new BankAccount(b.BankCode, b.BankBranchCode, b.BankAccountNo, b.IsDefault, b.FinnetCustomerNo)).ToList(),
                subscriptionBankAccounts:
                isSendSubscriptionBankAccount
                    ? customerAccount.SubscriptionBankAccount.Select(b =>
                        new BankAccount(b.BankCode, b.BankBranchCode, b.BankAccountNo, b.IsDefault, b.FinnetCustomerNo)).ToList()
                    : null,
                approved: true,
                openOmnibusFormFlag: true
            );


            _logger.Information("CreateIndividualAccount request: {Request}", createCustomerAccountReq.ToJson());
            await _fundConnextApi.ApiCustomerIndividualAccountV4PostAsync(
                createCustomerAccountReq,
                authToken);
        }

        public async Task CreateIndividualCustomerV5(CustomerInfo fundCustomerInfo, Crs? crs, string? ndidRequestId, CancellationToken cancellationToken = default)
        {
            var currentProfile = await GetCustomerProfileAndAccount(fundCustomerInfo.CardNumber, cancellationToken: cancellationToken);

            if (currentProfile == null)
            {
                var authToken = await GetAuthenticationToken();

                var req = fundCustomerInfo.ToAccountCreateRequestV5Payload(crs, ndidRequestId);
                _logger.Information("CreateIndividualCustomerV5 request: {Request}", req.ToJson());
                await _fundConnextApi.ApiCustomerIndividualV5PostAsync(
                    customerAccountCreateRequestV5: req,
                    xAuthToken: authToken,
                    cancellationToken: cancellationToken
                );
            }
            else
            {
                _logger.Information("Already have customer profile V5 with card number: {CardNumber}", fundCustomerInfo.CardNumber);
            }
        }

        public async Task UpdateIndividualCustomerV5(CustomerInfo fundCustomerInfo, Crs? crs, string? ndidRequestId, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await GetAuthenticationToken();
                var req = fundCustomerInfo.ToAccountCreateRequestV5Payload(crs, ndidRequestId);
                _logger.Debug("UpdateIndividualCustomerV5 request: {Request}", req.ToJson());
                await _fundConnextApi.ApiCustomerIndividualV5PutAsync(
                    customerAccountCreateRequestV5: req,
                    xAuthToken: authToken,
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error when UpdateIndividualCustomerV5: {Message}", e.Message);
                throw;
            }
        }

        public async Task CreateIndividualCustomerV6(CustomerInfo fundCustomerInfo, Crs? crs, string? ndidRequestId, string identityVerificationDateTime, string dopaVerificationDateTime, CancellationToken cancellationToken = default)
        {
            var currentProfile = await GetCustomerProfileAndAccount(fundCustomerInfo.CardNumber, cancellationToken: cancellationToken);

            if (currentProfile == null)
            {
                var authToken = await GetAuthenticationToken();

                var req = fundCustomerInfo.ToAccountCreateRequestV6Payload(crs, ndidRequestId, identityVerificationDateTime, dopaVerificationDateTime);
                _logger.Information("CreateIndividualCustomerV6 request: {Request}", req.ToJson());
                await _fundConnextApi.ApiCustomerIndividualV6PostAsync(
                    customerAccountCreateRequestV6: req,
                    xAuthToken: authToken,
                    cancellationToken: cancellationToken
                );
            }
            else
            {
                _logger.Information("Already have customer profile V6 with card number: {CardNumber}", fundCustomerInfo.CardNumber);
            }
        }

        public async Task UpdateIndividualCustomerV6(CustomerInfo fundCustomerInfo, Crs? crs, string? ndidRequestId, string identityVerificationDateTime, string dopaVerificationDateTime, CancellationToken cancellationToken = default)
        {
            string authToken = await GetAuthenticationToken();
            var req = fundCustomerInfo.ToAccountCreateRequestV6Payload(crs, ndidRequestId, identityVerificationDateTime, dopaVerificationDateTime);
            _logger.Debug("UpdateIndividualCustomerV6 request: {Request}", req.ToJson());
            await _fundConnextApi.ApiCustomerIndividualV6PutAsync(
                customerAccountCreateRequestV6: req,
                xAuthToken: authToken,
                cancellationToken: cancellationToken
            );
        }

        public async Task DebugUpdateIndividualCustomerV5(CustomerAccountCreateRequestV5 customerAccountCreateRequestV5, CancellationToken cancellationToken = default)
        {
            try
            {
                var authToken = await GetAuthenticationToken();
                _logger.Information("DebugUpdateIndividualCustomerV5 request: {Request}", customerAccountCreateRequestV5.ToJson());
                await _fundConnextApi.ApiCustomerIndividualV5PutAsync(
                    customerAccountCreateRequestV5: customerAccountCreateRequestV5,
                    xAuthToken: authToken,
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error when DebugUpdateIndividualCustomerV5: {Message}", e.Message);
                throw;
            }
        }

        public async Task UploadIndividualCustomerDocuments(
            string ticketId,
            IdentificationCardType idCardType,
            string idCardNumber,
            Document document)
        {
            var authToken = await this.GetAuthenticationToken();

            byte[] file;
            try
            {
                using HttpClient httpClient = new();
                file = await httpClient.GetByteArrayAsync(document.PreSignedUrl);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Upload Individual Customer. Unable To Download File from S3. pre-sign url: {Url}, Exception: {Exception}", document.PreSignedUrl, e.Message);

                throw new DownloadDocumentFailed($"Failed to Download: {document.DocumentType.ToDescriptionString()}");
            }

            using var streamFile = new MemoryStream(file);

            try
            {
                await _fundConnextApi.ApiCustomerIndividualFileTypeUploadPostAsync(
                    fileType: document.DocumentType.ToDescriptionString(),
                    xAuthToken: authToken,
                    file: new FileParameter(document.DocumentType.ToDescriptionString() + ".pdf", streamFile),
                    identificationCardType: idCardType.ToDescriptionString(),
                    passportCountry: null,
                    cardNumber: idCardNumber,
                    approved: true);
            }
            catch (Exception e)
            {
                this._logger.Error(
                    e,
                    "Unable To Upload Customer File: TicketID: {TicketId}, DocType: {DocType}, Exception: {Exception}",
                    ticketId,
                    document.DocumentType.ToDescriptionString(),
                    e.Message);

                throw new UploadDocumentFailed($"Failed to Upload: {document.DocumentType.ToDescriptionString()}");
            }
        }

        public async Task UploadIndividualAccountDocuments(
            string ticketId,
            IdentificationCardType idCardType,
            string idCardNumber,
            string accountId,
            Document document)
        {
            var authToken = await this.GetAuthenticationToken();

            byte[] file;
            try
            {
                using HttpClient httpClient = new();
                file = await httpClient.GetByteArrayAsync(document.PreSignedUrl);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Upload Individual Account. Unable To Download File from S3. pre-sign url: {Url}, Exception: {Exception}", document.PreSignedUrl, e.Message);

                throw new DownloadDocumentFailed($"Failed to Download: {document.OriginalDocumentType?.ToDescriptionString()}");
            }

            using var streamFile = new MemoryStream(file);

            try
            {
                await _fundConnextApi.ApiCustomerIndividualAccountFileTypeUploadPostAsync(
                    fileType: document.DocumentType.ToDescriptionString(),
                    xAuthToken: authToken,
                    file: new FileParameter(document.DocumentType.ToDescriptionString() + ".pdf", streamFile),
                    accountId: accountId,
                    identificationCardType: idCardType.ToDescriptionString(),
                    passportCountry: null,
                    cardNumber: idCardNumber,
                    approved: true);
            }
            catch (Exception e)
            {
                this._logger.Error(
                    e,
                    "Unable To Upload Account File: TicketID: {TicketId}, DocType: {DocType}, Exception: {Exception}",
                    ticketId,
                    document.DocumentType.ToDescriptionString(),
                    e.Message);

                throw new UploadDocumentFailed($"Failed to Upload: {document.OriginalDocumentType?.ToDescriptionString()}");
            }
        }

        public async Task UpdateIndivialAccountAsync(UpdateCustomerAccount updateCustomerAccount, CancellationToken cancellationToken = default)
        {
            var authToken = await this.GetAuthenticationToken();

            await _fundConnextApi.ApiCustomerIndividualAccountV4PutAsync(
                new CustomerAccountUpdateRequest(
                    identificationCardType: updateCustomerAccount.IdentificationCardType,
                    passportCountry: updateCustomerAccount.PassportCountry,
                    cardNumber: updateCustomerAccount.CardNumber,
                    accountId: updateCustomerAccount.AccountId,
                    icLicense: updateCustomerAccount.IcLicense,
                    accountOpenDate: updateCustomerAccount.AccountOpenDate,
                    mailingAddressSameAsFlag: updateCustomerAccount.MailingAddressSameAsFlag,
                    mailing: null,
                    mailingMethod: updateCustomerAccount.MailingMethod,
                    investmentObjective: updateCustomerAccount.InvestmentObjective,
                    investmentObjectiveOther: updateCustomerAccount.InvestmentObjectiveOther,
                    redemptionBankAccounts: updateCustomerAccount.RedemptionBankAccounts.Select(x => new BankAccount(x.BankCode, x.BankBranchCode, x.BankAccountNo, x.IsDefault, x.FinnetCustomerNo)).ToList(),
                    subscriptionBankAccounts: updateCustomerAccount.SubscriptionBankAccounts.Select(x => new BankAccount(x.BankCode, x.BankBranchCode, x.BankAccountNo, x.IsDefault, x.FinnetCustomerNo)).ToList(),
                    approved: updateCustomerAccount.Approved,
                    openOmnibusFormFlag: true
                ),
                authToken,
                cancellationToken
            );
        }

        public async Task UpdateJuristicAccountAsync(UpdateCustomerJuristicAccount updateCustomerJuristicAccount,
            CancellationToken cancellationToken = default)
        {
            string authToken = await GetAuthenticationToken();

            await _fundConnextApi.ApiCustomerJuristicAccountV4PutAsync(
                new CustomerJuristicAccountUpdateRequest(
                    juristicNumber: updateCustomerJuristicAccount.JuristicNumber,
                    accountId: updateCustomerJuristicAccount.AccountId,
                    icLicense: updateCustomerJuristicAccount.IcLicense,
                    accountOpenDate: updateCustomerJuristicAccount.AccountOpenDate,
                    mailingAddressSameAsFlag: updateCustomerJuristicAccount.MailingAddressSameAsFlag,
                    mailing: null,
                    mailingMethod: updateCustomerJuristicAccount.MailingMethod,
                    investmentObjective: updateCustomerJuristicAccount.InvestmentObjective,
                    investmentObjectiveOther: updateCustomerJuristicAccount.InvestmentObjectiveOther,
                    redemptionBankAccounts: updateCustomerJuristicAccount.RedemptionBankAccounts.Select(x => new BankAccount(x.BankCode, x.BankBranchCode, x.BankAccountNo, x.IsDefault, x.FinnetCustomerNo)).ToList(),
                    subscriptionBankAccounts: updateCustomerJuristicAccount.SubscriptionBankAccounts.Select(x => new BankAccount(x.BankCode, x.BankBranchCode, x.BankAccountNo, x.IsDefault, x.FinnetCustomerNo)).ToList(),
                    approved: updateCustomerJuristicAccount.Approved,
                    openOmnibusFormFlag: true
                ),
                authToken,
                cancellationToken
            );
        }

        private async Task<string> GetAuthenticationToken()
        {
            var cachedValue = await _cache.GetAsync(CacheKeys.FundConnextAuthToken);

            if (cachedValue != null)
            {
                return Encoding.UTF8.GetString(cachedValue);
            }

            var authenticationRequest = new AuthenticationRequest
            (
                username: _configuration["FundConnext:Username"],
                password: _configuration["FundConnext:Password"]
            );
            try
            {
                var authResponse = await _fundConnextApi.ApiAuthPostAsync(authenticationRequest);
                var cacheValue = authResponse.AccessToken;
                var cacheEntryOptions = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(_cacheTimeSpan);

                await _cache.SetAsync(CacheKeys.FundConnextAuthToken, Encoding.UTF8.GetBytes(cacheValue), cacheEntryOptions);

                return cacheValue;
            }
            catch (Exception e)
            {
                throw new UnauthorizedAccessException(e.Message);
            }
        }

        private Exception ApiExceptionHandling(ApiException e, OrderSide orderSide)
        {
            if (e is not { ErrorCode: 422, ErrorContent: string content }) return e;

            try
            {
                var error = JsonSerializer.Deserialize<FundConnextErrorResponse>(content);
                var errorCode = error?.ErrMsg?.code != null ? FundconnextFactory.NewErrorCode(orderSide, error.ErrMsg?.code!) : null;
                return errorCode != null ? new FundOrderException((FundOrderErrorCode)errorCode) : e;
            }
            catch (Exception jsonException)
            {
                _logger.Error(jsonException, "Json \"FundConnextErrorResponse\" deserialize failed");
                return e;
            }
        }

        public async Task<CustomerAccount?> GetCustomerAccountAsync(string cardId, string? passportCountry = null, CancellationToken cancellationToken = default)
        {
            var authToken = await GetAuthenticationToken();
            var resp = await _accountOpeningV5Api.ApiCustomerIndividualInvestorProfileV5GetAsync(cardNumber: cardId, passportCountry, xAuthToken: authToken, cancellationToken);
            var account = resp.Accounts.Find(a => a.AccountId.ToUpperInvariant().EndsWith('M'));
            if (account == null)
            {
                return null;
            }

            var redemptionBankAccounts = account.RedemptionBankAccounts.Select(o => new Application.Models.Bank.BankAccount(o.BankCode, o.BankAccountNo, o.BankBranchCode, o.VarDefault)).ToList();
            var subscriptionBankAccounts = account.SubscriptionBankAccounts.Select(o => new Application.Models.Bank.BankAccount(o.BankCode, o.BankAccountNo, o.BankBranchCode, o.VarDefault)).ToList();
            var customerAccount = new CustomerAccount(account.AccountId, account.IcLicense, redemptionBankAccounts, subscriptionBankAccounts, DateOnly.ParseExact(account.AccountOpenDate, DateFormatPattern, CultureInfo.InvariantCulture));
            return customerAccount;
        }

        public async Task<bool> CheckAccountExist(string accountNo,
            CancellationToken cancellationToken = default)
        {
            var cacheKey = $"{CacheKeys.FundConnextExistingAccount}_{accountNo}";
            var cached = await _cache.GetAsync(cacheKey, cancellationToken);
            if (cached != null && Encoding.UTF8.GetString(cached) == accountNo)
            {
                return true;
            }

            var authToken = await GetAuthenticationToken();

            try
            {
                await _fundConnextApi.ApiCustomerAccountGetAsync(authToken, accountNo, cancellationToken);

                var cacheEntryOptions = new DistributedCacheEntryOptions().SetAbsoluteExpiration(_cacheTimeSpan);
                await _cache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(accountNo), cacheEntryOptions, cancellationToken);

                return true;
            }
            catch (ApiException e)
            {
                var exception = ApiExceptionHandling(e, OrderSide.Buy);
                if (exception is FundOrderException { Code: FundOrderErrorCode.FOE216 })
                {
                    return false;
                }

                _logger.Error(e, "ApiCustomerAccountGetAsync failed");
                throw exception;
            }
        }
    }
}
