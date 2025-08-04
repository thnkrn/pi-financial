using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Pi.Common.Features;
using Pi.Common.Http;
using Pi.WalletService.API.Factories;
using Pi.WalletService.API.Models;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Transaction = Pi.WalletService.Application.Models.Transaction;
using TransactionV2 = Pi.WalletService.Domain.AggregatesModel.TransactionAggregate.Transaction;


namespace Pi.WalletService.API.Controllers;

public class TransactionController : ControllerBase
{
    private readonly ITransactionQueries _transactionQueries;
    private readonly IThaiEquityQueries _thaiEquityQueries;
    private readonly IGlobalEquityQueries _globalEquityQueries;
    private readonly ITransactionQueriesV2 _transactionQueriesV2;
    private readonly IFeatureService _featureService;


    public TransactionController(
        ITransactionQueries transactionQueries,
        IThaiEquityQueries thaiEquityQueries,
        IGlobalEquityQueries globalEquityQueries,
        ITransactionQueriesV2 transactionQueriesV2,
        IFeatureService featureService
    )
    {
        _transactionQueries = transactionQueries;
        _thaiEquityQueries = thaiEquityQueries;
        _globalEquityQueries = globalEquityQueries;
        _transactionQueriesV2 = transactionQueriesV2;
        _featureService = featureService;
    }

    [HttpGet("secure/transaction/{product}/{transactionNo}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TransactionDto>))]
    public async Task<IActionResult> GetTransactionStatus(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [Required] [EnumDataType(typeof(Product))] [JsonConverter(typeof(StringEnumConverter))]
        Product product,
        [Required] string transactionNo)
    {
        try
        {
            var transactionV2 = await _transactionQueriesV2.GetTransactionByTransactionNo(transactionNo, product, userId);
            if (transactionV2 != null)
            {
                return Ok(
                    new ApiResponse<TransactionDto>(
                        new TransactionDto
                        {
                            Id = transactionV2.CorrelationId,
                            AccountNo = transactionV2.AccountCode,
                            TransactionNo = transactionV2.TransactionNo,
                            Product = transactionV2.Product,
                            TransactionType = transactionV2.TransactionType,
                            QrValue = transactionV2.QrDeposit?.QrValue,
                            CreatedAt = transactionV2.CreatedAt.ToUniversalTime(),
                            RequestExpiredAt = transactionV2.Product == Product.GlobalEquities
                                ? transactionV2.GetQrExpiredTime()
                                : null,
                            Status = transactionV2.GetWorkAroundState(),
                            Fee = transactionV2.GetFee(),
                            RequestedAmount = transactionV2.RequestedAmount,
                            RequestedCurrency = transactionV2.GetRequestedCurrency(),
                            // Global Stuff
                            RequestedFxRate = transactionV2.GetRequestedFxRate(),
                            RequestedFxCurrency = transactionV2.GetRequestedFxCurrency(),
                            TransferAmount = transactionV2.GetTransferAmount(),
                            ExchangeRate = transactionV2.GetExchangeRate(),
                            TransferFee = transactionV2.GlobalTransfer?.TransferFee,
                        }));
            }

            // Legacy shit
            if (product == Product.GlobalEquities)
            {
                var globalTransaction =
                    await _transactionQueries.GetTransactionByTransactionNo<GlobalTransaction>(userId, transactionNo,
                        product);
                var requestAmount = globalTransaction.RequestedAmount;
                var currency = globalTransaction.Currency;

                if (globalTransaction is { Currency: Currency.USD, TransactionType: TransactionType.Deposit } ||
                    (globalTransaction.ReceivedAmount ?? 0) > 0)
                {
                    requestAmount = (globalTransaction.ReceivedAmount ?? 0) > 0
                        ? globalTransaction.ReceivedAmount!.Value
                        : RoundingUtils.RoundExchangeTransaction(
                            globalTransaction.TransactionType,
                            globalTransaction.Currency,
                            globalTransaction.RequestedAmount,
                            Currency.THB,
                            globalTransaction.RequestedFxRate!.Value
                        );
                    currency = Currency.THB;
                }

                return Ok(new ApiResponse<TransactionDto>(
                    new TransactionDto
                    {
                        Id = globalTransaction.Id,
                        AccountNo = globalTransaction.AccountNo,
                        TransactionNo = globalTransaction.TransactionNo,
                        Product = globalTransaction.Product,
                        TransactionType = globalTransaction.TransactionType,
                        RequestedAmount = requestAmount,
                        RequestedCurrency = currency,
                        QrValue = globalTransaction.QrValue,
                        Status = globalTransaction.CurrentState,
                        CreatedAt = globalTransaction.CreatedAt,
                        RequestExpiredAt = globalTransaction.QrExpiredTime,
                        RequestedFxRate = globalTransaction.RequestedFxRate,
                        RequestedFxCurrency = globalTransaction.RequestedFxCurrency,
                        TransferAmount = globalTransaction.TransferAmount,
                        ExchangeRate = globalTransaction.FxRate,
                        Fee = globalTransaction.BankFee,
                        TransferFee = globalTransaction.TransferFee
                    }));
            }

            var transaction =
                await _transactionQueries.GetTransactionByTransactionNo<Transaction>(userId, transactionNo, product);

            return Ok(new ApiResponse<TransactionDto>(
                new TransactionDto
                {
                    Id = transaction.Id,
                    AccountNo = transaction.AccountNo,
                    TransactionNo = transaction.TransactionNo,
                    Product = transaction.Product,
                    TransactionType = transaction.TransactionType,
                    RequestedAmount = transaction.Amount ?? 0,
                    QrValue = transaction.QrValue,
                    Status = transaction.CurrentState == "TfexCashDepositFailed"
                        ? "CashDepositFailed"
                        : transaction.CurrentState,
                    CreatedAt = transaction.CreatedAt,
                    Fee = transaction.BankFee
                }));
        }
        catch (InvalidDataException)
        {
            return Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Transaction Not Found");
        }
    }

    [HttpGet("secure/transaction/{product}/qr/active")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<ActiveTransaction>>))]
    public async Task<IActionResult> GetActiveQrTransaction(
        [FromHeader(Name = "user-id")] [Required]
        string userId,
        [Required] [EnumDataType(typeof(Product))] [JsonConverter(typeof(StringEnumConverter))]
        Product product)
    {
        if (_featureService.IsOn(Features.StateMachineV2))
        {
            var transactionsV2 = await _transactionQueriesV2.GetActiveQrTransaction(userId, product);

            return Ok(new ApiResponse<List<ActiveTransaction>>(transactionsV2));
        }

        var transactions = await _transactionQueries.GetActiveQrTransaction(userId, product);

        return Ok(new ApiResponse<List<ActiveTransaction>>(transactions));
    }

    [Obsolete("Use secure/transactions instead")]
    [HttpGet("secure/transaction/{product}")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(ApiPaginateResponse<List<TransactionHistoryPaginate>>))]
    public async Task<IActionResult> GetTransactions(
        [FromHeader(Name = "user-id")] string? userId,
        [FromHeader(Name = "sid")] string? sid,
        [FromHeader(Name = "deviceId")] string? deviceId,
        [FromHeader(Name = "device")] string? device,
        [Required] [EnumDataType(typeof(Product))] [JsonConverter(typeof(StringEnumConverter))]
        Product product,
        [FromQuery] TransactionHistoryPaginate paginate)
    {
        if (_featureService.IsOn(Features.StateMachineV2))
        {
            var v2Filters = new TransactionFilterV2(
                paginate.Status != null
                    ? paginate.Status switch
                    {
                        TransactionStatus.Success => Status.Success,
                        TransactionStatus.Fail => Status.Fail,
                        TransactionStatus.Pending => Status.Pending,
                        _ => Status.Processing
                    }
                    : null,
                new[] { product },
                null,
                null,
                paginate.CustCode,
                paginate.AccountCode,
                null,
                null,
                null,
                paginate.State,
                paginate.CreatedAtFrom,
                paginate.CreatedAtTo,
                null,
                null,
                null,
                null,
                paginate.TransactionType,
                userId,
                new[] { "DepositPaymentNotReceived", "OtpValidationNotReceived" }
            );

            var v2Transactions = await _transactionQueriesV2.GetTransactions(
                new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy,
                    paginate.OrderDir?.ToLower() ?? "desc"),
                v2Filters,
                sid,
                deviceId,
                device,
                paginate.AccountId
            );

            var v2Total = await _transactionQueriesV2.CountTransactions(v2Filters);
            return Ok(new ApiPaginateResponse<List<TransactionHistory>>(v2Transactions.Select(t =>
                    new TransactionHistory
                    {
                        Status = t.Status == Status.Processing ? Status.Pending.ToString() : t.Status.ToString(),
                        TransactionNo = t.TransactionNo,
                        TransactionType = t.TransactionType,
                        RequestedAmount = t.GlobalTransfer != null ? t.RequestedAmount.ToString("0.00") : null,
                        RequestedCurrency = t.GlobalTransfer != null ? t.GetRequestedCurrency() : Currency.THB,
                        ToCurrency = t.GlobalTransfer != null ? t.GetRequestedFxCurrency() : null,
                        CreatedAt = t.CreatedAt.ToUniversalTime(),
                        TransferAmount = t.GetTransferAmount()?.ToString("0.00"),
                        Channel = t.Channel == Channel.ODD ? Channel.ATS : t.Channel,
                        Fee = t.GetFee()?.ToString("0.00"),
                        TransferFee = t.GlobalTransfer?.TransferFee?.ToString("0.00") ?? null,
                        BankAccount = t.GetBankAccount(),
                    }
                ).ToList(),
                paginate.Page,
                paginate.PageSize,
                v2Total,
                paginate.OrderBy,
                paginate.OrderDir
            ));
        }

        var filters = new TransactionFilters(
            null,
            product,
            paginate.State,
            null,
            null,
            paginate.CustCode,
            paginate.AccountCode,
            null,
            paginate.Status,
            null,
            null,
            paginate.CreatedAtFrom,
            paginate.CreatedAtTo,
            paginate.TransactionType,
            userId,
            new[]
            {
                "AwaitingOtpValidation",
                "WaitingForOtpValidation",
                "RequestingOtpValidationFailed",
                "OtpValidationNotReceived",
                "CashWithdrawOtpValidationNotReceived",
                "GlobalDepositPaymentNotReceived",
                "GlobalWithdrawOtpValidationNotReceived",
                "CashDepositPaymentNotReceived",
                "DepositPaymentNotReceived"
            });

        if (product == Product.GlobalEquities)
        {
            var globalTransactions = await _transactionQueries.GetTransactions<GlobalTransaction>(
                new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy,
                    paginate.OrderDir?.ToLower() ?? "desc"),
                filters);

            var geTotal = await _transactionQueries.CountTransactions(filters);

            return Ok(new ApiPaginateResponse<List<TransactionHistory>>(
                globalTransactions.Select(g =>
                {
                    var requestAmount = g is { Currency: Currency.USD, TransactionType: TransactionType.Deposit } ||
                                        (g.ReceivedAmount ?? 0) > 0
                        ? (g.ReceivedAmount ?? 0) > 0
                            ? g.ReceivedAmount!.Value
                            : RoundingUtils.RoundExchangeTransaction(
                                g.TransactionType,
                                g.Currency,
                                g.RequestedAmount,
                                Currency.THB,
                                g.FxRate ?? g.RequestedFxRate!.Value
                            )
                        : g.RequestedAmount;

                    var transferAmount = g switch
                    {
                        { TransactionType: TransactionType.Deposit } =>
                            g.TransferAmount is null or 0
                                ? RoundingUtils.RoundExchangeTransaction(
                                    g.TransactionType,
                                    g.Currency,
                                    g.RequestedAmount,
                                    Currency.USD,
                                    g.FxRate ?? g.RequestedFxRate!.Value)
                                : g.TransferAmount,
                        { TransactionType: TransactionType.Withdraw } =>
                            g.TransferAmount is null or 0
                                ? RoundingUtils.RoundExchangeTransaction(
                                    g.TransactionType,
                                    g.Currency,
                                    g.RequestedAmount,
                                    Currency.THB,
                                    g.FxRate ?? g.RequestedFxRate!.Value)
                                : g.TransferAmount,
                        _ => g.TransferAmount
                    };

                    return new TransactionHistory
                    {
                        Status = g.Status,
                        TransactionNo = g.TransactionNo,
                        TransactionType = g.TransactionType,
                        RequestedAmount = requestAmount.ToString("0.00"),
                        RequestedCurrency = g.TransactionType == TransactionType.Deposit
                            ? Currency.THB
                            : Currency.USD,
                        ToCurrency = g.RequestedFxCurrency,
                        CreatedAt = g.CreatedAt,
                        TransferAmount = transferAmount?.ToString("0.00"),
                        Channel = g.Channel,
                        Fee = g.BankFee?.ToString("0.00"),
                        TransferFee = g.TransferFee?.ToString("0.00")
                    };
                }).ToList(),
                paginate.Page,
                paginate.PageSize,
                geTotal,
                paginate.OrderBy,
                paginate.OrderDir));
        }

        var transactions = await _transactionQueries.GetTransactions<Transaction>(
            new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy,
                paginate.OrderDir?.ToLower() ?? "desc"),
            filters,
            sid,
            deviceId,
            device,
            paginate.AccountId);

        var total = await _transactionQueries.CountTransactions(filters);

        var response = new ApiPaginateResponse<List<TransactionHistory>>(transactions.Select(t => new TransactionHistory
        {
            Status = t.Status,
            CreatedAt = t.CreatedAt,
            TransferAmount =
                    (t.TransactionType == TransactionType.Deposit ? t.ReceivedAmount ?? t.Amount : t.Amount)
                    ?.ToString("0.00"),
            Channel = t.Channel,
            TransactionNo = t.TransactionNo,
            TransactionType = t.TransactionType,
            BankAccount = t.TransactionType == TransactionType.Deposit
                    ? null
                    : $"{t.BankName} • {t.BankAccountNo?.Substring(t.BankAccountNo.Length - 4, 4)}",
            Fee = t.BankFee?.ToString("0.00")
        }).ToList(),
            paginate.Page,
            paginate.PageSize,
            total,
            paginate.OrderBy,
            paginate.OrderDir
        );

        return Ok(response);
    }

    [HttpGet("internal/transactions")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<List<TransactionHistoryV2>>))]
    public async Task<IActionResult> GetTransactionHistory(
        [FromQuery] TransactionHistoryV2Paginate paginate
    )
    {
        var filters = new TransactionFilterV2(
            paginate.Status,
            paginate.Products,
            paginate.Channel,
            paginate.TransactionNo,
            paginate.CustomerCode,
            paginate.AccountCode,
            paginate.BankAccountNo,
            paginate.BankCode,
            paginate.BankName,
            paginate.State,
            paginate.CreatedAtFrom,
            paginate.CreatedAtTo,
            paginate.EffectiveDateFrom,
            paginate.EffectiveDateTo,
            paginate.PaymentReceivedFrom,
            paginate.PaymentReceivedTo,
            paginate.TransactionType,
            null,
            paginate.NotStates
        );

        // Get transactions v2
        var transactions = await _transactionQueriesV2.GetTransactions(
            new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy,
                paginate.OrderDir?.ToLower() ?? "desc"),
            filters,
            null,
            null,
            null,
            paginate.AccountId
        );

        var total = await _transactionQueriesV2.CountTransactions(filters);

        return Ok(new ApiPaginateResponse<List<TransactionHistoryV2>>(transactions.Select(t =>
                new TransactionHistoryV2
                {
                    Status = t.Status.ToString(),
                    TransactionNo = t.TransactionNo,
                    TransactionType = t.TransactionType,
                    RequestedAmount = t.RequestedAmount.ToString("0.00"),
                    RequestedCurrency = t.GetRequestedCurrency(),
                    ToCurrency = t.GetRequestedFxCurrency(),
                    CreatedAt = t.CreatedAt.ToUniversalTime(),
                    TransferAmount = t.GetTransferAmount()?.ToString("0.00"),
                    Channel = t.Channel,
                    Fee = t.GetFee()?.ToString("0.00"),
                    TransferFee = t.GlobalTransfer?.TransferFee?.ToString("0.00") ?? null,
                    BankAccount = t.GetBankAccount(),
                    State = t.GetState(),
                    Product = t.Product,
                    AccountCode = t.AccountCode,
                    CustomerName = t.CustomerName,
                    BankAccountNo = t.BankAccountNo,
                    BankAccountName = t.BankAccountName,
                    BankName = t.BankName,
                    EffectiveDateTime = t.GetEffectiveDateTime(),
                    GlobalAccount = t.GetGlobalAccount(),
                    FailedReason = t.FailedReason
                }
            ).ToList(),
            paginate.Page,
            paginate.PageSize,
            total,
            paginate.OrderBy,
            paginate.OrderDir
        ));
    }

    [HttpGet("secure/transactions")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(ApiPaginateResponse<List<TransactionHistoryV2>>))]
    public async Task<IActionResult> GetTransactionHistoryByProduct(
        [FromHeader(Name = "user-id")] string? userId,
        [FromHeader(Name = "sid")] string? sid,
        [FromHeader(Name = "deviceId")] string? deviceId,
        [FromHeader(Name = "device")] string? device,
        [FromQuery] TransactionHistoryV2Paginate paginate
    )
    {
        var filters = new TransactionFilterV2(
            paginate.Status,
            paginate.Products,
            paginate.Channel,
            paginate.TransactionNo,
            paginate.CustomerCode,
            paginate.AccountCode,
            paginate.BankAccountNo,
            paginate.BankCode,
            paginate.BankName,
            paginate.State,
            paginate.CreatedAtFrom,
            paginate.CreatedAtTo,
            paginate.EffectiveDateFrom,
            paginate.EffectiveDateTo,
            paginate.PaymentReceivedFrom,
            paginate.PaymentReceivedTo,
            paginate.TransactionType,
            userId,
            new[] { "DepositPaymentNotReceived", "OtpValidationNotReceived" }
        );

        var transactions = await _transactionQueriesV2.GetTransactions(
            new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy,
                paginate.OrderDir?.ToLower() ?? "desc"),
            filters,
            sid,
            deviceId,
            device,
            paginate.AccountId
        );

        var total = await _transactionQueriesV2.CountTransactions(filters);
        return Ok(new ApiPaginateResponse<List<TransactionHistoryV2>>(transactions.Select(t =>
                new TransactionHistoryV2
                {
                    Status = t.Status.ToString(),
                    TransactionNo = t.TransactionNo,
                    TransactionType = t.TransactionType,
                    RequestedAmount = t.GlobalTransfer != null ? t.RequestedAmount.ToString("0.00") : null,
                    RequestedCurrency = t.GlobalTransfer != null ? t.GetRequestedCurrency() : Currency.THB,
                    ToCurrency = t.GlobalTransfer != null ? t.GetRequestedFxCurrency() : null,
                    CreatedAt = t.CreatedAt.ToUniversalTime(),
                    TransferAmount = t.GetTransferAmount()?.ToString("0.00"),
                    Channel = t.Channel,
                    Fee = t.GetFee()?.ToString("0.00"),
                    TransferFee = t.GlobalTransfer?.TransferFee?.ToString("0.00") ?? null,
                    BankAccount = t.GetBankAccount(),
                    State = t.GetState(),
                    Product = t.Product,
                    AccountCode = t.AccountCode,
                    CustomerName = t.CustomerName,
                    BankAccountNo = t.BankAccountNo,
                    BankAccountName = t.BankAccountName,
                    BankName = t.BankName,
                    EffectiveDateTime = t.GetEffectiveDateTime(),
                    GlobalAccount = t.GlobalTransfer != null ? t.GetGlobalAccount() : null
                }
            ).ToList(),
            paginate.Page,
            paginate.PageSize,
            total,
            paginate.OrderBy,
            paginate.OrderDir
        ));
    }

    [Obsolete("use internal/transactions instead")]
    [HttpGet("internal/transactions-v2")]
    [ProducesResponseType(StatusCodes.Status200OK,
        Type = typeof(ApiPaginateResponse<List<TransactionHistoryV2>>))]
    public async Task<IActionResult> GetTransactionHistoriesV2(
        [FromHeader(Name = "user-id")] string? userId,
        [FromHeader(Name = "sid")] string? sid,
        [FromHeader(Name = "deviceId")] string? deviceId,
        [FromHeader(Name = "device")] string? device,
        [FromQuery] Product[]? product,
        [FromQuery] TransactionHistoryV2Paginate paginate
    )
    {
        var filters = new TransactionFilterV2(
            paginate.Status,
            product,
            paginate.Channel,
            paginate.TransactionNo,
            paginate.CustomerCode,
            paginate.AccountCode,
            paginate.BankAccountNo,
            paginate.BankCode,
            paginate.BankName,
            paginate.State,
            paginate.CreatedAtFrom,
            paginate.CreatedAtTo,
            paginate.EffectiveDateFrom,
            paginate.EffectiveDateTo,
            paginate.PaymentReceivedFrom,
            paginate.PaymentReceivedTo,
            paginate.TransactionType,
            userId,
            paginate.NotStates
        );

        // Get transactions v2
        var transactions = await _transactionQueriesV2.GetTransactions(
            new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy,
                paginate.OrderDir?.ToLower() ?? "desc"),
            filters,
            sid,
            deviceId,
            device,
            paginate.AccountId
        );

        var total = await _transactionQueriesV2.CountTransactions(filters);

        return Ok(new ApiPaginateResponse<List<TransactionHistoryV2>>(transactions.Select(t =>
                new TransactionHistoryV2
                {
                    Status = t.Status.ToString(),
                    TransactionNo = t.TransactionNo,
                    TransactionType = t.TransactionType,
                    RequestedAmount = t.GlobalTransfer != null ? t.RequestedAmount.ToString("0.00") : null,
                    RequestedCurrency = t.GlobalTransfer != null ? t.GetRequestedCurrency() : Currency.THB,
                    ToCurrency = t.GlobalTransfer != null ? t.GetRequestedFxCurrency() : null,
                    CreatedAt = t.CreatedAt.ToUniversalTime(),
                    TransferAmount = t.GetTransferAmount()?.ToString("0.00"),
                    Channel = t.Channel,
                    Fee = t.GetFee()?.ToString("0.00"),
                    TransferFee = t.GlobalTransfer?.TransferFee?.ToString("0.00") ?? null,
                    BankAccount = t.GetBankAccount(),
                    State = t.GetState(),
                    Product = t.Product,
                    AccountCode = t.AccountCode,
                    CustomerName = t.CustomerName,
                    BankAccountNo = t.BankAccountNo,
                    BankAccountName = t.BankAccountName,
                    BankName = t.BankName,
                    EffectiveDateTime = t.GetEffectiveDateTime(),
                    GlobalAccount = t.GlobalTransfer != null ? t.GetGlobalAccount() : null
                }
            ).ToList(),
            paginate.Page,
            paginate.PageSize,
            total,
            paginate.OrderBy,
            paginate.OrderDir
        ));
    }

    [HttpGet("internal/transaction/{transactionNo}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TransactionDetailsDto>))]
    public async Task<IActionResult> GetInternalTransaction(
        [Required] string transactionNo,
        [FromQuery] string? excludeInnerState
    )
    {
        var transactionV2 = await _transactionQueriesV2.GetTransactionByTransactionNo(transactionNo, excludeInnerState);
        if (transactionV2 == null)
        {
            return NotFound();
        }

        return Ok(new ApiResponse<TransactionDetailsDto>(new TransactionDetailsDto(transactionV2)));
    }

    [HttpGet("internal/transaction/{transactionNo}/email-history")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<EmailHistoryDto>>))]
    public async Task<IActionResult> GetTransactionEmailHistory([Required] string transactionNo)
    {
        var emailHistories = await _transactionQueriesV2.GetTransactionEmailHistory(transactionNo);
        return Ok(new ApiResponse<List<EmailHistoryDto>>(
            emailHistories
                .Select(e => new EmailHistoryDto(e.TransactionNo, e.EmailType.ToString(), e.SentAt))
                .ToList())
        );
    }


    [Obsolete("use internal/transactions instead")]
    [HttpGet("internal/transactions/deposit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<List<DepositTransaction>>))]
    public async Task<IActionResult> GetDepositTransactions([FromQuery] DepositTransactionPaginate paginate)
    {
        var filters = QueryFactory.NewTransactionFilters(paginate, TransactionType.Deposit);
        var orderDir = paginate.OrderDir?.ToLower() ?? "desc";
        var transactions = filters.ProductType switch
        {
            ProductType.ThaiEquity => await _thaiEquityQueries.GetDepositTransactions(
                new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy, orderDir),
                filters
            ),
            ProductType.GlobalEquity => await _globalEquityQueries.GetDepositTransactions(
                new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy, orderDir),
                filters
            ),
            _ => await _transactionQueries.GetDepositTransactions(
                new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy, orderDir),
                filters
            )
        };

        var total = filters.ProductType switch
        {
            ProductType.ThaiEquity => await _thaiEquityQueries.CountDepositTransactions(filters),
            ProductType.GlobalEquity => await _globalEquityQueries.CountDepositTransactions(filters),
            _ => await _transactionQueries.CountDepositTransactions(filters)
        };

        var response = new ApiPaginateResponse<List<DepositTransaction>>(transactions,
            paginate.Page,
            paginate.PageSize,
            total,
            paginate.OrderBy,
            paginate.OrderDir
        );

        return Ok(response);
    }

    [Obsolete("use internal/transactions instead")]
    [HttpGet("internal/transactions/withdraw")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<List<WithdrawTransaction>>))]
    public async Task<IActionResult> GetWithdrawTransactions([FromQuery] TransactionPaginate paginate)
    {
        var filters = QueryFactory.NewTransactionFilters(paginate, TransactionType.Withdraw);
        var orderDir = paginate.OrderDir?.ToLower() ?? "desc";
        var transactions = filters.ProductType switch
        {
            ProductType.ThaiEquity => await _thaiEquityQueries.GetWithdrawTransactions(
                new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy, orderDir),
                filters
            ),
            ProductType.GlobalEquity => await _globalEquityQueries.GetWithdrawTransactions(
                new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy, orderDir),
                filters
            ),
            _ => await _transactionQueries.GetWithdrawTransactions(
                new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy, orderDir),
                filters
            )
        };

        var total = filters.ProductType switch
        {
            ProductType.ThaiEquity => await _thaiEquityQueries.CountWithdrawTransactions(filters),
            ProductType.GlobalEquity => await _globalEquityQueries.CountWithdrawTransactions(filters),
            _ => await _transactionQueries.CountWithdrawTransactions(filters)
        };

        var response = new ApiPaginateResponse<List<WithdrawTransaction>>(transactions,
            paginate.Page,
            paginate.PageSize,
            total,
            paginate.OrderBy,
            paginate.OrderDir
        );

        return Ok(response);
    }

    [Obsolete("use internal/transactions instead")]
    [HttpGet("internal/transactions/refund")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<List<RefundTransaction>>))]
    public async Task<IActionResult> GetRefundTransactions([FromQuery] RefundTransactionPaginate paginate)
    {
        var filters = QueryFactory.NewTransactionFilters(paginate, TransactionType.Withdraw);
        var transactions = await _transactionQueries.GetRefundTransactions(
            new PaginateRequest(paginate.Page, paginate.PageSize, paginate.OrderBy,
                paginate.OrderDir?.ToLower() ?? "desc"),
            filters
        );
        var total = await _transactionQueries.CountRefundTransactions(filters);

        var response = new ApiPaginateResponse<List<RefundTransaction>>(transactions,
            paginate.Page,
            paginate.PageSize,
            total,
            paginate.OrderBy,
            paginate.OrderDir
        );

        return Ok(response);
    }

    [Obsolete("use internal/transaction/{transactionNo} instead")]
    [HttpGet("internal/transactions/{transactionNo}/deposit")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<DepositTransaction>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDepositTransaction([Required] string transactionNo)
    {
        DepositTransaction? transaction;
        if (transactionNo.ToUpper().StartsWith("GE"))
        {
            transaction = await _globalEquityQueries.GetDepositTransaction(transactionNo);
        }
        else
        {
            transaction = await _thaiEquityQueries.GetDepositTransaction(transactionNo);
        }

        if (transaction == null) return NotFound();

        return Ok(new ApiResponse<DepositTransaction>(transaction));
    }

    [Obsolete("use internal/transaction/{transactionNo} instead")]
    [HttpGet("internal/transactions/{transactionNo}/withdraw")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<WithdrawTransaction>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetWithdrawTransaction([Required] string transactionNo)
    {
        WithdrawTransaction? transaction;
        if (transactionNo.ToUpper().StartsWith("GE"))
        {
            transaction = await _globalEquityQueries.GetWithdrawTransaction(transactionNo);
        }
        else
        {
            transaction = await _thaiEquityQueries.GetWithdrawTransaction(transactionNo);
        }

        if (transaction == null) return NotFound();

        return Ok(new ApiResponse<WithdrawTransaction>(transaction));
    }

    [Obsolete("use internal/transaction/{transactionNo} instead")]
    [HttpGet("internal/transactions/globalEquities/{transactionNo}/{transactionType}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<GlobalTransaction>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGlobalDepositTransaction([Required] string transactionNo,
        [Required] [EnumDataType(typeof(TransactionType))] [JsonConverter(typeof(StringEnumConverter))]
        TransactionType transactionType)
    {
        GlobalTransaction? transaction = transactionType switch
        {
            TransactionType.Deposit =>
                await _transactionQueries.GetDepositTransactionByTransactionNo<GlobalTransaction>(transactionNo),
            TransactionType.Withdraw =>
                await _transactionQueries.GetWithdrawTransactionByTransactionNo<GlobalTransaction>(transactionNo),
            _ => null
        };

        if (transaction == null) return NotFound();

        return Ok(new ApiResponse<GlobalTransaction>(transaction));
    }

    [HttpGet("internal/transaction/summary")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TransactionSummaryResponse<TransactionDetailsDto>>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransactionSummary([FromQuery] TransactionSummaryRequest transactionSummaryRequest)
    {
        var (transactionSummary, transactions) = await _transactionQueriesV2.GetTransactionsSummaryByDate(
            transactionSummaryRequest.Product,
            transactionSummaryRequest.CreatedAtFrom,
            transactionSummaryRequest.CreatedAtTo
        );

        return Ok(
            new ApiResponse<TransactionSummaryResponse<TransactionDetailsDto>>(
                new TransactionSummaryResponse<TransactionDetailsDto>
                {
                    TransactionSummary = transactionSummary,
                    Transactions = transactions.Select(t => new TransactionDetailsDto(t)).ToList()
                }));
    }
}