using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Client.PaymentService.Api;
using Pi.Client.PaymentService.Client;
using Pi.Client.PaymentService.Model;
using Pi.WalletService.Application.Services.PaymentService;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.Exceptions;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.Infrastructure.Options;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentApi _paymentApi;
    private readonly ILogger<PaymentService> _logger;
    private readonly Guid _apiKey;

    public PaymentService(
        IPaymentApi paymentApi,
        IOptionsSnapshot<PaymentServiceOptions> options,
        ILogger<PaymentService> logger)
    {
        _paymentApi = paymentApi;
        _apiKey = Guid.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(options.Value.ApiKey)));
        _logger = logger;
    }

    public async Task<string> TransferViaOdd(
        string transactionNo,
        decimal transferAmount,
        TransactionType transactionType,
        string customerBankCode,
        string customerBankAccountNo,
        string customerBankAccountName,
        string customerBankAccountTaxId,
        string customerNo,
        Product product
    )
    {
        try
        {
            var paymentType = transactionType switch
            {
                TransactionType.Deposit => PiPaymentServiceDomainAggregateModelsPaymentAggregateTransferType.Debit,
                TransactionType.Withdraw => PiPaymentServiceDomainAggregateModelsPaymentAggregateTransferType.Credit,
                _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
            };

            // round transfer amount to have 2 decimal points
            var roundedTransferAmount = transactionType switch
            {
                TransactionType.Deposit => RoundingUtils.RoundTransactionValue(TransactionType.Deposit, transferAmount,
                    Currency.THB),
                TransactionType.Withdraw => RoundingUtils.RoundTransactionValue(TransactionType.Withdraw,
                    transferAmount, Currency.THB),
                _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
            };

            var request = new PiPaymentServiceAPIModelsTransferRequestDto(
                transactionNo,
                roundedTransferAmount,
                customerBankCode,
                customerBankAccountNo,
                customerBankAccountName,
                customerBankAccountTaxId,
                customerNo,
                product.ToString().ToUpper());

            var response = await _paymentApi.InternalPaymentTransferTypePostWithHttpInfoAsync(
                _apiKey,
                paymentType,
                request);

            if (response.StatusCode != HttpStatusCode.OK || !response.Data.Data.IsSuccess)
            {
                throw new UnableToTransferViaOddException(response.Data.Data.StatusMessage);
            }

            return response.Data.Data.ReferenceNo;
        }
        catch (ApiException e)
        {
            _logger.LogError(
                e,
                "Transaction No: {TransactionNo} PaymentService: Error occurred while calling Payment. {Exception}",
                transactionNo,
                e.ErrorContent);

            var problemDetails = JsonSerializer.Deserialize<ProblemDetails>(e.ErrorContent.ToString() ?? string.Empty);
            Exception err;
            switch (problemDetails?.Title)
            {
                case PaymentErrorCodes.FinnetInsufficientBalance:
                    err = new FinnetInsufficientBalanceException($"PaymentService error: Finnet insufficient account balance. [[{PaymentErrorCodes.FinnetInsufficientBalance}]]");
                    break;
                default:
                    err = new UnableToTransferViaOddException("PaymentService error: Unable to transfer via Odd.");
                    break;
            }

            throw err;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Transaction No: {TransactionNo} PaymentService: Unable to call Payment. {Exception}",
                transactionNo,
                e.Message);
            throw new Exception($"Unable to call Payment");
        }
    }
}
