using Pi.Financial.Client.Freewill.Model;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Services.CustomerService
{
    public interface ICustomerService
    {
        public record QueryAtsResponse(
            string ReferId,
            string SendDate,
            string SendTime,
            string AccountNo,
            string BankCode,
            string BankEName,
            string BankTName,
            string BankBranchCode,
            string BankBranchEName,
            string BankBranchTName,
            string BankAccountNo,
            ResultCode ResultCode,
            string Reason);

        public record CustomerServiceResponse(
            string TransId,
            string ResultCode,
            string ReferId,
            string SendDate,
            string SendTime,
            string Reason,
            string ApplicationId
        );

        [Obsolete("This can produces incorrect result, Use another overload instead")]
        Task<BankAccount?> GetCustomerBankAccount(string custCode);
        Task<BankAccount?> GetCustomerBankAccount(string tradingAccountNo, TransactionType transactionType);
        Task<BankAccount?> GetCustomerBankAccount(string custCode, Product product, TransactionType transactionType);

        Task<decimal> GetAccountCreditKycLimit(string custCode, string accountCode);

        Task<QueryAtsResponse> QueryAts(string accountNo, string rpType);

        Task<string> QueryCustomerAccountNo(string custCode, string accountCode);

        Task<CustomerServiceResponse> DepositCashAsync(
            string custCode,
            string transId,
            string accountNo,
            decimal amount,
            Purpose purpose,
            string paymentType,
            string clearingBank,
            string remark);

        Task<CustomerServiceResponse> DepositAtsAsync(
            string transId,
            string accountNo,
            decimal amount,
            string bankCode,
            string bankAccountNo,
            string remark,
            string bankBranchCode = "");

        Task<CustomerServiceResponse> WithdrawAnyPayTypeAsync(
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
            string? channel = "");

        Task<CustomerServiceResponse> WithdrawAtsAsync(
            string transId,
            string accountNo,
            decimal amount,
            string bankCode,
            string bankAccountNo,
            string remark,
            DateOnly effectiveDate,
            string bankBranchCode = "");

        Task<decimal> QueryWithdrawalBalance(string accountNo);

        string GetPaymentTypeCode(Channel channel, TransactionType transactionType);

        string GetBankCode(string bankName, Channel channel);
    }
}