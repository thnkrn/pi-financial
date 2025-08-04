using Pi.WalletService.Application.Factories;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries.Filters;
using Pi.WalletService.Application.Services;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.UtilityAggregate;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Transaction = Pi.WalletService.Domain.AggregatesModel.TransactionAggregate.Transaction;
using TransactionSummary = Pi.WalletService.Application.Models.TransactionSummary;

namespace Pi.WalletService.Application.Queries;

public class TransactionQueriesV2 : ITransactionQueriesV2
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly ITransactionHistoryService _transactionHistoryService;
    private readonly IEmailHistoryRepository _emailHistoryRepository;


    public TransactionQueriesV2(
        ITransactionRepository transactionRepository,
        ITransactionHistoryService transactionHistoryService,
        IEmailHistoryRepository emailHistoryRepository
        )
    {
        _transactionRepository = transactionRepository;
        _transactionHistoryService = transactionHistoryService;
        _emailHistoryRepository = emailHistoryRepository;
    }

    public async Task<List<ActiveTransaction>> GetActiveQrTransaction(string userId, Product product)
    {
        var transactions = await _transactionRepository.GetActiveQrTransaction(userId, product);

        return transactions
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new ActiveTransaction(
                    t.CorrelationId,
                    t.TransactionNo ?? string.Empty,
                    t.Product,
                    t.CreatedAt.ToUniversalTime()
                )
            )
            .ToList();
    }

    public async Task<Transaction?> GetTransactionById(Guid correlationId)
    {
        return await _transactionRepository.GetByCorrelationId(correlationId);
    }

    public async Task<Transaction?> GetTransactionByTransactionNo(string transactionNo, string? excludeState)
    {
        var result = await _transactionRepository.GetByNo(transactionNo, null, null);

        if (excludeState == null) return result;

        if (result is { QrDeposit: not null } && result.QrDeposit.CurrentState == excludeState)
        {
            result.QrDeposit = null;
        }
        if (result is { OddDeposit: not null } && result.OddDeposit.CurrentState == excludeState)
        {
            result.OddDeposit = null;
        }
        if (result is { AtsDeposit: not null } && result.AtsDeposit.CurrentState == excludeState)
        {
            result.AtsDeposit = null;
        }

        if (result is { AtsWithdraw: not null } && result.AtsWithdraw.CurrentState == excludeState)
        {
            result.AtsWithdraw = null;
        }
        if (result is { UpBack: not null } && result.UpBack.CurrentState == excludeState)
        {
            result.UpBack = null;
        }
        if (result is { GlobalTransfer: not null } && result.GlobalTransfer.CurrentState == excludeState)
        {
            result.GlobalTransfer = null;
        }
        if (result is { OddWithdraw: not null } && result.OddWithdraw.CurrentState == excludeState)
        {
            result.OddWithdraw = null;
        }
        if (result is { Recovery: not null } && result.Recovery.CurrentState == excludeState)
        {
            result.Recovery = null;
        }
        if (result is { GlobalManualAllocate: not null } && result.GlobalManualAllocate.CurrentState == excludeState)
        {
            result.GlobalManualAllocate = null;
        }

        return result;
    }

    public async Task<Transaction?> GetTransactionByTransactionNo(string transactionNo, Product product, string? userId)
    {
        return await _transactionRepository.GetByNo(transactionNo, product, userId);
    }

    public async Task<(TransactionSummary, List<Transaction>)> GetTransactionsSummaryByDate(Product product,
        DateTime createdAtFrom, DateTime createdAtTo)
    {
        TransactionSummary? transactionSummary;
        var excludeState = new[] { "DepositPaymentNotReceived", "OtpValidationNotReceived" };
        var transactions = await _transactionRepository.GetTransactionListByDateTime(product, createdAtFrom, createdAtTo);
        transactions = transactions
            .Where(t => !excludeState.Contains(t.GetState())
                        && t is not { Status: Status.Fail, TransactionType: TransactionType.Deposit or TransactionType.Withdraw })
            .ToList();
        if (product == Product.GlobalEquities)
        {
            transactionSummary = GetTransactionSummaryForGlobalProduct(transactions);
        }
        else
        {
            throw new NotImplementedException();
        }

        return (transactionSummary, transactions);
    }

    private static TransactionSummary GetTransactionSummaryForGlobalProduct(List<Transaction> transactions)
    {
        var successQrDepositTransactions =
            transactions.Where(t => t is { Status: Status.Success, TransactionType: TransactionType.Deposit, Channel: Channel.QR }).ToList();
        var successOddDepositTransactions =
            transactions.Where(t => t is { Status: Status.Success, TransactionType: TransactionType.Deposit, Channel: Channel.ODD or Channel.ATS }).ToList();
        var successWithdrawTransactions = transactions.Where(t => t is { Status: Status.Success, TransactionType: TransactionType.Withdraw }).ToList();
        var failedDepositTransactions = transactions.Where(t => t is { Status: Status.Fail, TransactionType: TransactionType.Deposit }).ToList();
        var pendingManualAllocationDepositTransactions =
            transactions.Where(t => t is { Status: Status.Pending, TransactionType: TransactionType.Deposit, FailedReason: FailedDescription.ManualAllocationXnt }).ToList();
        var pendingManualAllocationWithdrawTransactions =
            transactions.Where(t => t is { Status: Status.Pending, TransactionType: TransactionType.Withdraw, FailedReason: FailedDescription.ManualAllocationXnt }).ToList();
        var pendingDepositTransactions = transactions.Where(t => t is { Status: Status.Pending, TransactionType: TransactionType.Deposit } && t.FailedReason != FailedDescription.ManualAllocationXnt).ToList();
        var pendingWithdrawTransactions = transactions.Where(t => t is { Status: Status.Pending, TransactionType: TransactionType.Withdraw } && t.FailedReason != FailedDescription.ManualAllocationXnt).ToList();
        var refundTransactions = transactions.Where(t => t is { TransactionType: TransactionType.Deposit, RefundInfo.Amount: > 0 }).ToList();
        var sbaDepositTransaction = transactions.Where(t =>
            (t.Status == Status.Success || t.FailedReason is FailedDescription.ManualAllocationXnt or FailedDescription.InsufficientBalance) && t.TransactionType == TransactionType.Deposit).ToList();

        var successOddKBankTransactions = successOddDepositTransactions.Where(t => t.BankCode == BankCode.KBank).ToList();
        var successOddScbTransactions = successOddDepositTransactions.Where(t => t.BankCode == BankCode.Scb).ToList();
        var successOddKtbTransactions = successOddDepositTransactions.Where(t => t.BankCode == BankCode.Ktb).ToList();
        var successOddBblTransactions = successOddDepositTransactions.Where(t => t.BankCode == BankCode.Bbl).ToList();

        var sumSuccessQrDepositAmountThb = successQrDepositTransactions.Sum(t => t.GetPaymentReceivedAmount() ?? 0);
        var sumSuccessQrDepositAmountUsd = successQrDepositTransactions.Sum(t => t.GetTransferAmount() ?? t.GetFxConfirmedAmount() ?? 0);
        var sumSuccessOddKBankTransactionsAmountThb = successOddKBankTransactions.Sum(t => t.GetPaymentReceivedAmount() ?? 0);
        var sumSuccessOddKBankTransactionsAmountUsd = successOddKBankTransactions.Sum(t => t.GetTransferAmount() ?? t.GetFxConfirmedAmount() ?? 0);
        var sumSuccessOddScbTransactionsAmountThb = successOddScbTransactions.Sum(t => t.GetPaymentReceivedAmount() ?? 0);
        var sumSuccessOddScbTransactionsAmountUsd = successOddScbTransactions.Sum(t => t.GetTransferAmount() ?? t.GetFxConfirmedAmount() ?? 0);
        var sumSuccessOddKtbTransactionsAmountThb = successOddKtbTransactions.Sum(t => t.GetPaymentReceivedAmount() ?? 0);
        var sumSuccessOddKtbTransactionsAmountUsd = successOddKtbTransactions.Sum(t => t.GetTransferAmount() ?? t.GetFxConfirmedAmount() ?? 0);
        var sumSuccessOddBblTransactionsAmountThb = successOddBblTransactions.Sum(t => t.GetPaymentReceivedAmount() ?? 0);
        var sumSuccessOddBblTransactionsAmountUsd = successOddBblTransactions.Sum(t => t.GetTransferAmount() ?? t.GetFxConfirmedAmount() ?? 0);
        var sumSuccessWithdrawAmountThb = successWithdrawTransactions.Sum(t => t.GetTransferAmount() ?? 0);
        var sumSuccessWithdrawAmountUsd = successWithdrawTransactions.Sum(t => t.RequestedAmount);

        var sumFailedDepositAmountThb = failedDepositTransactions.Sum(t => t.GetPaymentReceivedAmount() ?? 0);
        var sumFailedDepositAmountUsd = failedDepositTransactions.Sum(t => t.GetFxConfirmedAmount() ?? 0);

        var sumPendingManualAllocationDepositAmountThb = pendingManualAllocationDepositTransactions.Sum(t => t.GetPaymentReceivedAmount() ?? 0);
        var sumPendingManualAllocationDepositAmountUsd = pendingManualAllocationDepositTransactions.Sum(t => t.GetTransferAmount() ?? 0);
        var sumPendingManualAllocationWithdrawAmountThb = pendingManualAllocationWithdrawTransactions.Sum(t => t.NetAmount ?? 0);
        var sumPendingManualAllocationWithdrawAmountUsd = pendingManualAllocationWithdrawTransactions.Sum(t => t.RequestedAmount);

        var sumPendingDepositAmountThb = pendingDepositTransactions.Where(t => t.GlobalTransfer!.FailedReason != "Insufficient Balance").Sum(t => t.NetAmount ?? 0);
        var sumPendingDepositAmountUsd = pendingDepositTransactions.Where(t => t.FailedReason == "Insufficient Balance").Sum(t => t.GetFxConfirmedAmount() ?? 0);
        var sumPendingWithdrawAmountThb = pendingWithdrawTransactions.Sum(t => t.GetTransferAmount() ?? 0);
        var sumPendingWithdrawAmountUsd = pendingWithdrawTransactions.Sum(t => t.RequestedAmount);

        var sumSbaDepositAmountThb = sbaDepositTransaction.Sum(t => t.NetAmount ?? 0);
        var sumSbaDepositAmountUsd = sbaDepositTransaction.Sum(t => t.GetFxConfirmedAmount() ?? 0);
        var sumSbaWithdrawAmountThb = successWithdrawTransactions.Sum(t => t.GetTransferAmount() ?? 0);
        var sumSbaWithdrawAmountUsd = successWithdrawTransactions.Sum(t => t.RequestedAmount);

        var totalThb = transactions.Sum(t => t.TransactionType == TransactionType.Deposit
            ? t.NetAmount ?? 0
            : (t.NetAmount ?? 0) * -1);
        var totalUsd = transactions.Sum(t => t.TransactionType == TransactionType.Deposit
            ? t.GetTransferAmount() ?? 0
            : t.RequestedAmount * -1);

        return new TransactionSummary(
            successQrDepositTransactions.Count,
            successOddKBankTransactions.Count,
            successOddScbTransactions.Count,
            successOddKtbTransactions.Count,
            successOddBblTransactions.Count,
            successWithdrawTransactions.Count,
            failedDepositTransactions.Count,
            pendingManualAllocationDepositTransactions.Count,
            pendingManualAllocationWithdrawTransactions.Count,
            refundTransactions.Count,
            sbaDepositTransaction.Count,
            successWithdrawTransactions.Count,
            transactions.Count,
            sumSuccessQrDepositAmountThb,
            sumSuccessOddKBankTransactionsAmountThb,
            sumSuccessOddScbTransactionsAmountThb,
            sumSuccessOddKtbTransactionsAmountThb,
            sumSuccessOddBblTransactionsAmountThb,
            sumSuccessWithdrawAmountThb,
            sumFailedDepositAmountThb,
            sumPendingManualAllocationDepositAmountThb,
            sumPendingManualAllocationWithdrawAmountThb,
            refundTransactions.Sum(t => t.RefundInfo?.Amount ?? 0),
            sumSbaDepositAmountThb,
            sumSbaWithdrawAmountThb,
            totalThb,
            sumSuccessQrDepositAmountUsd,
            sumSuccessOddKBankTransactionsAmountUsd,
            sumSuccessOddScbTransactionsAmountUsd,
            sumSuccessOddKtbTransactionsAmountUsd,
            sumSuccessOddBblTransactionsAmountUsd,
            sumSuccessWithdrawAmountUsd,
            sumFailedDepositAmountUsd,
            sumPendingManualAllocationDepositAmountUsd,
            sumPendingManualAllocationWithdrawAmountUsd,
            0,
            sumSbaDepositAmountUsd,
            sumSbaWithdrawAmountUsd,
            totalUsd,
            pendingDepositTransactions.Count,
            sumPendingDepositAmountThb,
            sumPendingDepositAmountUsd,
            pendingWithdrawTransactions.Count,
            sumPendingWithdrawAmountThb,
            sumPendingWithdrawAmountUsd
        );
    }

    public async Task<List<Transaction>> GetTransactions(
        PaginateRequest paginateRequest,
        TransactionFilterV2? filters,
        string? sid = null,
        string? deviceId = null,
        string? device = null,
        string? accountId = null)
    {
        var transactions = new List<Transaction>();

        if (filters is { Channel: Channel.SetTrade })
        {
            var setTradeEPayFilters = EntityFactory.NewSetTradeEPayFilter(filters);
            var setTradeEPayTransactions = await _transactionRepository
                .GetSetTradeEPayTransactions(
                    1,
                    paginateRequest.PageSize * paginateRequest.PageNo,
                    setTradeEPayFilters,
                    paginateRequest.OrderBy,
                    paginateRequest.OrderDirection
                );
            transactions.AddRange(setTradeEPayTransactions);
        }
        else
        {
            if (filters?.TransactionType is TransactionType.Deposit or null)
            {
                var depositTransactionFilters = filters != null
                    ? EntityFactory.NewDepositTransactionV2Filters(filters)
                    : null;

                var depositTransactions = await _transactionRepository
                    .GetDepositTransactions(
                        1,
                        paginateRequest.PageSize * paginateRequest.PageNo,
                        depositTransactionFilters,
                        paginateRequest.OrderBy,
                        paginateRequest.OrderDirection
                    );

                transactions.AddRange(depositTransactions);
            }

            if (filters?.TransactionType is TransactionType.Withdraw or null)
            {
                var withdrawEntrypointFilter = filters != null
                    ? EntityFactory.NewWithdrawTransactionV2Filters(filters)
                    : null;


                var withdrawTransactions = await _transactionRepository
                    .GetWithdrawTransactions(
                        1,
                        paginateRequest.PageSize * paginateRequest.PageNo,
                        withdrawEntrypointFilter,
                        paginateRequest.OrderBy,
                        paginateRequest.OrderDirection
                    );

                transactions.AddRange(withdrawTransactions);
            }
        }

        if (!string.IsNullOrWhiteSpace(sid) &&
            !string.IsNullOrWhiteSpace(deviceId) &&
            filters is { Product: not null, TransactionType: not null } &&
            !string.IsNullOrWhiteSpace(accountId))
        {
            var now = DateTime.Now.AddHours(7);

            var outSourceTransaction =
                await _transactionHistoryService.GetTransactionHistory(
                    new SiriusAuthentication(sid, deviceId, device),
                    accountId,
                    (Product)filters.Product.FirstOrDefault()!,
                    (TransactionType)filters.TransactionType,
                    new PaginateRequest(0, paginateRequest.PageSize * paginateRequest.PageNo, null, null),
                    DateOnly.FromDateTime(filters.CreatedAtFrom ?? now.AddMonths(-3)),
                    DateOnly.FromDateTime(filters.CreatedAtTo ?? now)
                );

            transactions.AddRange(outSourceTransaction.Select(t =>
                new Transaction(
                    t.Id,
                    t.CurrentState,
                    t.TransactionNo,
                    t.UserId,
                    "",
                    t.CustomerCode,
                    t.Channel,
                    t.Product,
                    t.TransactionType == TransactionType.Deposit ? Purpose.Collateral : Purpose.Withdraw,
                    t.Amount!.Value,
                    t.ReceivedAmount,
                    t.CustomerName,
                    t.BankAccountName,
                    t.BankAccountNo,
                    t.BankName,
                    null,
                    "",
                    null,
                    null,
                    t.TransactionType,
                    t.CreatedAt,
                    t.CreatedAt
                    )
            ));
        }

        // slices the list of transactions based on the paginate request
        transactions = transactions
            .Skip(paginateRequest.PageSize * (paginateRequest.PageNo - 1))
            .Take(paginateRequest.PageSize)
            .ToList();

        return transactions;
    }

    public async Task<int> CountTransactions(TransactionFilterV2? filters)
    {
        if (filters is { Channel: Channel.SetTrade })
        {
            var setTradeEPayFilters = EntityFactory.NewSetTradeEPayFilter(filters);
            return await _transactionRepository.CountSetTradeEPayTransactions(setTradeEPayFilters);
        }

        DepositTransactionFilters? depositTransactionFilters = null;
        WithdrawTransactionFilters? withdrawTransactionFilters = null;

        if (filters?.TransactionType is TransactionType.Deposit or null)
        {
            depositTransactionFilters = filters != null
                ? EntityFactory.NewDepositTransactionV2Filters(filters)
                : null;
        }

        if (filters?.TransactionType is TransactionType.Withdraw or null)
        {
            withdrawTransactionFilters = filters != null
                ? EntityFactory.NewWithdrawTransactionV2Filters(filters)
                : null;
        }

        return await _transactionRepository.CountTransactions(filters?.TransactionType, depositTransactionFilters, withdrawTransactionFilters);
    }

    public async Task<List<EmailHistory>> GetTransactionEmailHistory(string transactionNo)
    {
        return await _emailHistoryRepository.GetByTransactionNo(transactionNo);
    }
}