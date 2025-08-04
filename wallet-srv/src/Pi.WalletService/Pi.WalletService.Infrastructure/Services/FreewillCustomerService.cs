using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.Financial.Client.Freewill.Api;
using Pi.Financial.Client.Freewill.Model;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Services.CustomerService;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.LogAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using static Pi.WalletService.Application.Services.CustomerService.ICustomerService;

namespace Pi.WalletService.Infrastructure.Services
{
    public static class PurposeExtension
    {
        public static string ToFreewillPurposeString(this Purpose purpose)
        {
            return purpose switch
            {
                _ => "E"
            };
        }
    }

    [Serializable]
    public class FreewillDepositException : Exception
    {
        public FreewillDepositException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public FreewillDepositException()
        {
        }

        public FreewillDepositException(string message)
            : base(message)
        {
        }

        // Without this constructor, deserialization will fail
        protected FreewillDepositException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class FreewillWithdrawException : Exception
    {
        public FreewillWithdrawException(string message, Exception? innerException) : base(message, innerException)
        {
        }

        public FreewillWithdrawException()
        {
        }

        public FreewillWithdrawException(string message)
            : base(message)
        {
        }

        // Without this constructor, deserialization will fail
        protected FreewillWithdrawException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    public class FreewillCustomerService : ICustomerService
    {
        private readonly static IDictionary<string, string> QrBankCodes = new Dictionary<string, string>()
        {
            { "SCB", "0E" },
            { "BBL", "1E" },
            { "KBANK", "2E" },
            { "KKP", "4E" },
            { "KKPATS", "4C" },
            { "KTB", "5E" },
            { "TTB", "97" },
            { "CIMBT", "71" },
            { "UOBT", "96" },
            { "BAY", "94" },
            { "LHBANK", "0Z" }
        };

        private readonly static IDictionary<string, string> SetTradeBankCodes = new Dictionary<string, string>()
        {
            { "SCB", "92" },
            { "BBL", "91" },
            { "KBANK", "93" },
            { "KTB", "95" }
        };

        private readonly ICustomerModuleApi _freeWillClient;
        private readonly ILogger<FreewillCustomerService> _logger;
        private readonly ICreditModuleApi _freewillCreditModule;
        private readonly IFreewillRequestLogRepository _freewillRequestLogRepository;
        private readonly bool _useMockValues;
        private const string DateFormat = "yyyyMMdd";
        private const string ResponseDateFormat = "dd/MM/yyyy";
        private const string TimeFormat = "HH:mm:ss";

        public FreewillCustomerService(
            ICustomerModuleApi freeWillClient,
            ILogger<FreewillCustomerService> logger,
            IConfiguration configuration,
            ICreditModuleApi freewillCreditModule,
            IFreewillRequestLogRepository freewillRequestLogRepository
        )
        {
            _freeWillClient = freeWillClient;
            _logger = logger;
            _useMockValues = configuration.GetValue<bool>("Freewill:UseMockValues");
            _freewillCreditModule = freewillCreditModule;
            _freewillRequestLogRepository = freewillRequestLogRepository;
        }

        [Obsolete("This can produces incorrect result, Use another overload instead")]
        public async Task<BankAccount?> GetCustomerBankAccount(string custCode)
        {
            if (_useMockValues)
            {
                return await Task.FromResult(new BankAccount("014", "1112000099", "0000"));
            }
            var dateTimeNow = DateUtils.GetThDateTimeNow();
            var sendDate = dateTimeNow.ToString(DateFormat);
            var sendTime = dateTimeNow.ToString(TimeFormat);
            var referId = GenerateReferId("QB", dateTimeNow);
            var getBankInfoReq = new GetBankAccInfoRequest
            (
                referId: referId,
                custcode: custCode,
                sendDate: sendDate,
                sendTime: sendTime
            );
            GetBankAccInfoResponse? bankAccountResp = null;
            try
            {
                bankAccountResp = await _freeWillClient.QueryCustomerBankAccountInfoAsync(getBankInfoReq);
                if (bankAccountResp.ResultList == null) return null;

                var bankAccount = bankAccountResp.ResultList
                    .Where(r => DateTime.UtcNow.AddHours(7).Date
                                    .CompareTo(
                                        DateTime.ParseExact(
                                            string.IsNullOrWhiteSpace(r.Effdate)
                                                ? dateTimeNow.ToString(ResponseDateFormat)
                                                : r.Effdate, ResponseDateFormat,
                                            CultureInfo.InvariantCulture).Date) >=
                                0 &&
                                r is { Transtype: "TRADE", Rptype: "P" })
                    .Select(r => new BankAccount(r.Bankcode, r.Bankaccno, r.Bankbranchcode))
                    .FirstOrDefault();

                return bankAccount;
            }
            finally
            {
                await LogFreewill(getBankInfoReq.ReferId, null, getBankInfoReq.ToJson(), bankAccountResp?.ToJson(), FreewillRequestType.QueryCustomerBankAccountInfo);
            }
        }

        public async Task<BankAccount?> GetCustomerBankAccount(string tradingAccountNo, TransactionType transactionType)
        {
            var custCode = TradingAccountUtils.GetCustCodeFromTradingAccountNo(tradingAccountNo);
            var product = TradingAccountUtils.FindProductFromTradingAccount(tradingAccountNo);
            return await GetCustomerBankAccount(custCode, product, transactionType);
        }

        public async Task<BankAccount?> GetCustomerBankAccount(string custCode, Product product, TransactionType transactionType)
        {
            if (_useMockValues)
            {
                return await Task.FromResult(new BankAccount("014", "1112000099", "0000"));
            }

            var dateTimeNow = DateUtils.GetThDateTimeNow();
            var sendDate = dateTimeNow.ToString(DateFormat);
            var sendTime = dateTimeNow.ToString(TimeFormat);
            var referId = GenerateReferId("QB", dateTimeNow);
            var getBankInfoReq = new GetBankAccInfoRequest
            (
                referId: referId,
                custcode: custCode,
                sendDate: sendDate,
                sendTime: sendTime
            );
            GetBankAccInfoResponse? bankAccountResp = null;

            try
            {
                bankAccountResp = await _freeWillClient.QueryCustomerBankAccountInfoAsync(getBankInfoReq);
                if (bankAccountResp.ResultList == null) return null;

                var rpType = transactionType switch
                {
                    TransactionType.Deposit => "R",
                    TransactionType.Withdraw => "P",
                    TransactionType.Transfer or TransactionType.Refund
                        => throw new InvalidDataException("Invalid transaction type for GetCustomerBankAccount"),
                    _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
                };

                var acctCode = product switch
                {
                    Product.Cash => "CC",
                    Product.CashBalance => "CH",
                    Product.CreditBalance => "CB",
                    Product.CreditBalanceSbl => "BB",
                    Product.Crypto => "CT",
                    Product.Derivatives => "TF",
                    Product.GlobalEquities => "XU",
                    Product.Funds => "UT",
                    _ => throw new ArgumentOutOfRangeException(nameof(product), product, null)
                };

                var currentDate = dateTimeNow.Date;
                var availableBankAccount = bankAccountResp.ResultList
                    .Where(r =>
                    {
                        if (string.IsNullOrWhiteSpace(r.Effdate) || string.IsNullOrWhiteSpace(r.Enddate))
                        {
                            return false;
                        }

                        var effectiveDate = DateTime.ParseExact(r.Effdate, ResponseDateFormat, CultureInfo.InvariantCulture).Date;
                        var endEffectiveDate = DateTime.ParseExact(r.Enddate, ResponseDateFormat, CultureInfo.InvariantCulture).Date;

                        return currentDate.CompareTo(effectiveDate) >= 0
                               && currentDate.CompareTo(endEffectiveDate) <= 0
                               && r.Rptype == rpType
                               && r.Custcode == custCode
                               && r.Transtype != "MEDIA";
                    })
                    .OrderByDescending(
                        t =>
                            DateTime.ParseExact(
                                t.Enddate,
                                ResponseDateFormat,
                                CultureInfo.InvariantCulture).Date
                    )
                    .ToList();

                if (!availableBankAccount.Any())
                {
                    _logger.LogInformation("No Available Bank Account for Customer: {CustCode}", custCode);
                    return null;
                }

                var bankAccount = availableBankAccount.Find(b => b.Acctcode == acctCode && b.Transtype == "WD") ??
                                  availableBankAccount.Find(b => b.Acctcode == acctCode && b.Transtype == "TRADE") ??
                                  availableBankAccount.Find(b => b.Account.Split("-").Last() == "8" && b.Transtype == "WD") ??
                                  availableBankAccount.Find(b => b.Account.Split("-").Last() == "8" && b.Transtype == "TRADE");

                if (bankAccount == null)
                {
                    _logger.LogInformation("No Bank Account Match with Product {Product} for Customer: {CustCode}", product.ToString(), custCode);
                    return null;
                }

                return new BankAccount(bankAccount.Bankcode, bankAccount.Bankaccno, bankAccount.Bankbranchcode);
            }
            finally
            {
                await LogFreewill(getBankInfoReq.ReferId, null, getBankInfoReq.ToJson(), bankAccountResp?.ToJson(), FreewillRequestType.QueryCustomerBankAccountInfo);
            }
        }

        public async Task<decimal> GetAccountCreditKycLimit(string custCode, string accountCode)
        {
            if (_useMockValues)
            {
                return await Task.FromResult(12120000);
            }

            var dateTimeNow = DateUtils.GetThDateTimeNow();
            var sendDate = dateTimeNow.ToString(DateFormat);
            var sendTime = dateTimeNow.ToString(TimeFormat);
            var referId = GenerateReferId("QI", dateTimeNow);

            var getAccountInfoReq = new GetAccountInfoRequest(referId, sendDate, sendTime, custCode);
            GetAccountInfoResponse? getAccountInfoResp = null;

            try
            {
                getAccountInfoResp = await _freeWillClient.QueryCustomerAccountInfoAsync(getAccountInfoReq);
                var first = getAccountInfoResp.AccountList.Find(a => a.Account.Replace("-", string.Empty) == accountCode.Replace("-", string.Empty));

                var appCreditLine = first?.Appcreditline;

                if (!string.IsNullOrEmpty(appCreditLine))
                {
                    return decimal.Parse(appCreditLine, NumberStyles.Float);
                }

                _logger.LogError("App credit line is null or empty for accountCode: {AccountCode}", accountCode);
                throw new InvalidDataException(
                    $"GetAccountCreditKycLimit: App credit line is null or empty for accountCode: {accountCode}");
            }
            finally
            {
                await LogFreewill(getAccountInfoReq.ReferId, null, getAccountInfoReq.ToJson(), getAccountInfoResp?.ToJson(), FreewillRequestType.QueryCustomerAccountInfo);
            }
        }

        public async Task<QueryAtsResponse> QueryAts(string accountNo, string rpType)
        {
            var dateTimeNow = DateUtils.GetThDateTimeNow();
            var sendDate = dateTimeNow.ToString(DateFormat);
            var sendTime = dateTimeNow.ToString(TimeFormat);
            var referId = GenerateReferId("QA", dateTimeNow);
            var queryAtsRequest = new QueryATSRequest(referId, sendDate, sendTime, accountNo.Replace("-", string.Empty), rpType);
            QueryATSResponse? response = null;

            try
            {
                response = await _freewillCreditModule.QueryATSAsync(queryAtsRequest);
                return new QueryAtsResponse(
                    response.ReferId,
                    response.SendDate,
                    response.SendTime,
                    response.AccountNo,
                    response.BankCode,
                    response.BankEname,
                    response.BankTname,
                    response.BankBranchCode,
                    response.BankBranchEname,
                    response.BankBranchTname,
                    response.BankAccountNo,
                    response.ResultCode,
                    response.Reason
                );
            }
            catch (Exception e)
            {
                _logger.LogError(e, "FreewillQueryATS: Failed to query ATS from Freewill. Error: {Message}", e.Message);
                return new QueryAtsResponse(
                    referId,
                    sendDate,
                    sendTime,
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    ResultCode._906,
                    "Internal Server Error"
                );
            }
            finally
            {
                await LogFreewill(queryAtsRequest.ReferId, null, queryAtsRequest.ToJson(), response?.ToJson(), FreewillRequestType.QueryATS);
            }
        }

        public async Task<string> QueryCustomerAccountNo(string custCode, string accountCode)
        {
            var dateTimeNow = DateUtils.GetThDateTimeNow();
            var sendDate = dateTimeNow.ToString(DateFormat);
            var sendTime = dateTimeNow.ToString(TimeFormat);
            var referId = GenerateReferId("QC", dateTimeNow);

            var getAccountInfoReq = new GetAccountInfoRequest(referId, sendDate, sendTime, custCode);
            GetAccountInfoResponse? getAccountInfoResp = null;

            try
            {
                getAccountInfoResp = await _freeWillClient.QueryCustomerAccountInfoAsync(getAccountInfoReq);
            }
            finally
            {
                await LogFreewill(getAccountInfoReq.ReferId, null, getAccountInfoReq.ToJson(), getAccountInfoResp?.ToJson(), FreewillRequestType.QueryCustomerAccountInfo);
            }

            var accountNo = getAccountInfoResp.AccountList.Find(a => a.Account.Replace("-", string.Empty) == accountCode.Replace("-", string.Empty))?.Account;
            return accountNo ?? string.Empty;
        }

        public async Task<CustomerServiceResponse> DepositCashAsync(
            string custCode,
            string transId,
            string accountNo,
            decimal amount,
            Purpose purpose,
            string paymentType,
            string clearingBank,
            string remark)
        {
            var dateTimeNow = DateUtils.GetThDateTimeNow();
            var sendDate = dateTimeNow.ToString(DateFormat);
            var sendTime = dateTimeNow.ToString(TimeFormat);
            var referId = GenerateReferId("DH", dateTimeNow);

            var depositCashRequest = new DepositCashRequest(
                referId,
                sendDate,
                sendTime,
                custCode,
                transId,
                accountNo.Replace("-", string.Empty),
                amount.ToString("0.00", CultureInfo.InvariantCulture),
                sendDate,
                sendDate,
                purpose.ToFreewillPurposeString(),
                paymentType,
                clearingBank,
                remark);

            UpdateBankAccountInfo200Response? response = null;
            try
            {
                response = await _freewillCreditModule.DepositCashAsync(depositCashRequest);
                return new CustomerServiceResponse(
                    transId,
                    response.ResultCode.ToString()!,
                    response.ReferId,
                    response.SendDate,
                    response.SendTime,
                    response.Reason,
                    response.Applicationid);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "DepositCashAsync: Failed deposit to Freewill. Error: {Message}", e.Message);
                throw new FreewillDepositException("Freewill Internal Errors.");
            }
            finally
            {
                await LogFreewill(depositCashRequest.ReferId, depositCashRequest.TransId, depositCashRequest.ToJson(), response?.ToJson(), FreewillRequestType.DepositCash);
            }
        }

        public async Task<CustomerServiceResponse> WithdrawAnyPayTypeAsync(
            string transId,
            string accountNo,
            decimal amount,
            string paymentType,
            string clearingBank,
            string remark,
            string? bankCode = "",
            string? bankBranchCode = "",
            string? bankAccountNo = "",
            string? sourceBank = "",
            string? channel = "")
        {
            var dateTimeNow = DateUtils.GetThDateTimeNow();
            var sendDate = dateTimeNow.ToString(DateFormat);
            var sendTime = dateTimeNow.ToString(TimeFormat);
            var referId = GenerateReferId("WA", dateTimeNow);

            var withdrawAnyPayTypeRequest = new WithdrawAnyPaytypeRequest(
                referId,
                sendDate,
                sendTime,
                transId,
                accountNo.Replace("-", string.Empty),
                amount.ToString("0.00", CultureInfo.InvariantCulture),
                sendDate,
                sendDate,
                bankCode!,
                bankBranchCode!,
                bankAccountNo!,
                paymentType,
                sourceBank!,
                channel!,
                clearingBank,
                remark);

            UpdateBankAccountInfo200Response? response = null;
            try
            {
                var currentBalance = await QueryWithdrawalBalance(accountNo.Replace("-", string.Empty));

                if (amount > currentBalance)
                {
                    throw new FreewillWithdrawException("Current balance is less than requested amount");
                }

                response = await _freewillCreditModule.WithdrawAnyPaytypeAsync(withdrawAnyPayTypeRequest);
                return new CustomerServiceResponse(
                    transId,
                    response.ResultCode.ToString()!,
                    response.ReferId,
                    response.SendDate,
                    response.SendTime,
                    response.Reason,
                    response.Applicationid);
            }
            catch (Exception e) when (e is not InvalidDataException && e is not FreewillWithdrawException)
            {
                _logger.LogError(e, "WithdrawAnyPayTypeAsync: Failed withdraw from Freewill. Error: {Message}",
                    e.Message);
                throw new Exception("Freewill Internal Errors.");
            }
            finally
            {
                await LogFreewill(
                    withdrawAnyPayTypeRequest.ReferId,
                    withdrawAnyPayTypeRequest.TransId,
                    withdrawAnyPayTypeRequest.ToJson(),
                    response?.ToJson(),
                    FreewillRequestType.WithdrawAnyPaytype
                );
            }
        }

        public async Task<CustomerServiceResponse> DepositAtsAsync(
            string transId,
            string accountNo,
            decimal amount,
            string bankCode,
            string bankAccountNo,
            string remark,
            string bankBranchCode)
        {
            var dateTimeNow = DateUtils.GetThDateTimeNow();
            var sendDate = dateTimeNow.ToString(DateFormat);
            var sendTime = dateTimeNow.ToString(TimeFormat);
            var referId = GenerateReferId("DS", dateTimeNow);

            UpdateBankAccountInfo200Response? response = null;
            var request = new DepositAtsRequest(
                referId,
                sendDate,
                sendTime,
                transId,
                accountNo,
                amount.ToString(CultureInfo.InvariantCulture),
                sendDate,
                sendDate,
                bankCode,
                bankBranchCode,
                bankAccountNo,
                remark);
            try
            {
                response = await _freewillCreditModule.DepositAtsAsync(request);
                return new CustomerServiceResponse(
                    transId,
                    response.ResultCode.ToString()!,
                    response.ReferId,
                    response.SendDate,
                    response.SendTime,
                    response.Reason,
                    response.Applicationid);

            }
            catch (Exception ex) when (ex is not InvalidDataException)
            {
                _logger.LogError(ex, "DepositAtsAsync: Failed deposit ats from Freewill. Error: {Message}",
                    ex.Message);
                throw new FreewillDepositException("Freewill Internal Errors.");
            }
            finally
            {
                await LogFreewill(
                    request.ReferId,
                    request.TransId,
                    request.ToJson(),
                    response?.ToJson(),
                    FreewillRequestType.DepositATS
                );
            }
        }

        public async Task<CustomerServiceResponse> WithdrawAtsAsync(
            string transId,
            string accountNo,
            decimal amount,
            string bankCode,
            string bankAccountNo,
            string remark,
            DateOnly effectiveDate,
            string bankBranchCode)
        {
            var dateTimeNow = DateUtils.GetThDateTimeNow();
            var sendDate = dateTimeNow.ToString(DateFormat);
            var sendTime = dateTimeNow.ToString(TimeFormat);
            var referId = GenerateReferId("WS", dateTimeNow);

            UpdateBankAccountInfo200Response? response = null;
            var request = new WithdrawAtsRequest(
                referId,
                sendDate,
                sendTime,
                transId,
                accountNo,
                amount.ToString(CultureInfo.InvariantCulture),
                sendDate,
                effectiveDate.ToString(DateFormat),
                bankCode,
                bankBranchCode,
                bankAccountNo,
                remark);
            try
            {
                response = await _freewillCreditModule.WithdrawAtsAsync(request);
                return new CustomerServiceResponse(
                    transId,
                    response.ResultCode.ToString()!,
                    response.ReferId,
                    response.SendDate,
                    response.SendTime,
                    response.Reason,
                    response.Applicationid);

            }
            catch (Exception ex) when (ex is not InvalidDataException)
            {
                _logger.LogError(ex, "WithdrawAtsAsync: Failed withdraw ats from Freewill. Error: {Message}",
                    ex.Message);
                throw new FreewillDepositException("Freewill Internal Errors.");
            }
            finally
            {
                await LogFreewill(
                    request.ReferId,
                    request.TransId,
                    request.ToJson(),
                    response?.ToJson(),
                    FreewillRequestType.WithdrawATS
                );
            }
        }

        public async Task<decimal> QueryWithdrawalBalance(string accountNo)
        {
            var dateTimeNow = DateUtils.GetThDateTimeNow();
            var sendDate = dateTimeNow.ToString(DateFormat);
            var sendTime = dateTimeNow.ToString(TimeFormat);
            var referId = GenerateReferId("QW", dateTimeNow);
            QueryWithdrawalAmountResponse? response = null;
            var request = new QueryWithdrawalAmountRequest(
                referId,
                sendDate,
                sendTime,
                accountNo.Replace("-", string.Empty),
                effectiveDate: sendDate);

            try
            {
                response = await _freewillCreditModule.QueryWithdrawalAmountAsync(request);
                var first = response.WithdrawalList.Find(w => w.EffectivePeriod == "T");
                var withdrawalAmount = first?.WithdrawalAmount;
                if (!string.IsNullOrEmpty(withdrawalAmount))
                {
                    return decimal.Parse(withdrawalAmount, NumberStyles.Float);
                }

                if (int.TryParse(response.ResultCode?.ToString(), out var resultCode)
                    && resultCode == (int)FreewillUtils.FreewillResultCode.DepositWithdrawDisabled)
                {
                    _logger.LogError("Account No: {accountNo}, Failed with reason: {reason}", accountNo, response.Reason);
                    throw new DepositWithdrawDisabledException(response.Reason);
                }

                _logger.LogError("Freewill withdrawal amount of date T is null or empty");
                throw new InvalidDataException("Cannot find withdrawal amount of date T");
            }
            catch (Exception ex) when (ex is not InvalidDataException && ex is not DepositWithdrawDisabledException)
            {
                _logger.LogError("Cannot query withdrawal amount exception: {exception}", ex);
                throw new Exception("Freewill Internal Errors.");
            }
            finally
            {
                await LogFreewill(referId, null, request.ToJson(), response?.ToJson(),
                    FreewillRequestType.QueryWithdrawalAmount);
            }
        }

        public string GetPaymentTypeCode(Channel channel, TransactionType transactionType)
        {
            return channel switch
            {
                Channel.SetTrade => "01",
                Channel.QR or Channel.ODD or Channel.OnlineViaKKP => "18",
                Channel.ATS => "02",
                Channel.EForm => transactionType == TransactionType.Transfer
                    ? "96"
                    : "95",
                Channel.TransferApp => "17",
                _ => string.Empty
            };
        }

        public string GetBankCode(string bankName, Channel channel)
        {
            if (channel is Channel.QR or Channel.OnlineViaKKP)
            {
                if (QrBankCodes.TryGetValue("KKP", out var kkpCode))
                {
                    return kkpCode;
                }

                throw new InvalidDataException($"Invalid bank name: {bankName}");
            }

            var bankCodes = channel switch
            {
                Channel.SetTrade => SetTradeBankCodes,
                _ => QrBankCodes
            };

            if (bankCodes.TryGetValue(bankName, out var code))
            {
                return code;
            }

            throw new InvalidDataException($"Invalid bank name: {bankName}");
        }

        private async Task LogFreewill(string referId, string? transId, string request, string? response, FreewillRequestType type)
        {
            try
            {
                _freewillRequestLogRepository.Create(new FreewillRequestLog(referId, transId, request, response, type));
                await _freewillRequestLogRepository.UnitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogFreewillRequest: Unable to log freewill request with Exception: {Message}", ex.Message);
            }
        }

        private static string GenerateReferId(String prefix, DateTime dateTimeNow)
        {
            var randomNumber = GenerateRandomNumber();
            return $"{prefix}{dateTimeNow.ToString($"yyyyMMddHHmmss{randomNumber}")}";
        }

        private static int GenerateRandomNumber()
        {
            using var rng = RandomNumberGenerator.Create();
            var randomBytes = new byte[2];
            rng.GetBytes(randomBytes);

            var randomNumber = BitConverter.ToUInt16(randomBytes, 0) % 10000;

            return randomNumber;
        }
    }
}
