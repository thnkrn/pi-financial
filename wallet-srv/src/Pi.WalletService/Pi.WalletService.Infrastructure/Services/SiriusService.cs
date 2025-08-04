using System.Globalization;
using Microsoft.Extensions.Logging;
using Pi.Client.Sirius.Api;
using Pi.Client.Sirius.Model;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Services;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Infrastructure.Services;

public class SiriusService : ITransactionHistoryService
{
    private readonly ISiriusApi _siriusApi;
    private readonly ILogger<SiriusService> _logger;

    public SiriusService(ISiriusApi siriusApi, ILogger<SiriusService> logger)
    {
        _siriusApi = siriusApi;
        _logger = logger;
    }

    public async Task<List<Transaction>> GetTransactionHistory(
        SiriusAuthentication authentication,
        string accountId,
        Product product,
        TransactionType transactionType,
        PaginateRequest paginateRequest,
        DateOnly beginDate,
        DateOnly endDate)
    {
        try
        {
            var request = new GetTransactionHistoryRequest(
                0,
                int.Parse(accountId),
                endDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture),
                paginateRequest.PageNo,
                MapProductToAccountType(product),
                MapTransactionType(transactionType),
                beginDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture)
            );

            var response = await _siriusApi.CgsV1AccountTransactionHistoryPostAsync(authentication.Sid, authentication.DeviceId, authentication.Device, request);

            if (response.Response == null)
            {
                throw new Exception(response.Message);
            }

            return response.Response?.TransactionList
                .Select(t =>
                    new Transaction(
                        Guid.Empty,
                        string.Empty,
                        string.Empty,
                        string.Empty,
                        null,
                        MapStatusToState(transactionType, t.Status),
                        MapStatusResponse(t.Status),
                        t.TransactionNo,
                        transactionType,
                        decimal.Parse(t.Amount),
                        Currency.THB,
                        null,
                        null,
                        decimal.Parse(t.Amount),
                        null,
                        null,
                        MapPaymentMethodResponse(t.PaymentMethod),
                        product,
                        transactionType == TransactionType.Deposit ? t.FromAccountCode : t.ToAccountCode,
                        null,
                        t.BankShortName,
                        0,
                        DateTimeOffset.FromUnixTimeSeconds(t.TransactionTimestamp).DateTime.ToUniversalTime(),
                        null
                        )).ToList()!;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to Get Transaction History from Sirius: Message: {Message}", e.Message);
            return new List<Transaction>();
        }
    }

    private static Channel MapPaymentMethodResponse(string paymentMethod)
    {
        return paymentMethod switch
        {
            "QRCode" => Channel.QR,
            "ATS" => Channel.ATS,
            "TransferApp" => Channel.TransferApp,
            _ => Channel.Unknown,
        };
    }
    private static string MapStatusResponse(string status)
    {
        return status switch
        {
            "Processed" => "Success",
            "Processing" => "Pending",
            _ => "Fail"
        };
    }

    private static string MapStatusToState(TransactionType transactionType, string status)
    {
        if (transactionType == TransactionType.Deposit)
        {
            return status switch
            {
                "Processed" => "Final",
                "Processing" => "Pending",
                _ => "DepositFailed"
            };
        }

        return status switch
        {
            "Processed" => "Final",
            "Processing" => "Pending",
            _ => "WithdrawFailed"
        };
    }

    private static string MapProductToAccountType(Product product)
    {
        return product switch
        {
            Product.Cash => "Cash",
            Product.CashBalance => "Cash Balance",
            Product.CreditBalance or Product.CreditBalanceSbl => "Credit Balance/SBL",
            Product.Derivatives => "Derivative",
            _ => product.ToString()
        };
    }

    private static string MapTransactionType(TransactionType transactionType)
    {
        return transactionType switch
        {
            TransactionType.Deposit => "deposit",
            TransactionType.Withdraw => "withdraw",
            TransactionType.Transfer => "transfer",
            TransactionType.Refund => "refund",
            _ => throw new ArgumentOutOfRangeException(nameof(transactionType), transactionType, null)
        };
    }
}