using Pi.Common.Features;
using Pi.WalletService.Application.Factories;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Services;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.RefundAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Utilities;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using CashDepositState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashDepositState;
using CashWithdrawState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashWithdrawState;
using CashWithdrawStateName = Pi.WalletService.IntegrationEvents.Models.CashWithdrawState;
using DepositEntrypointState = Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate.DepositEntrypointState;
using WithdrawEntrypointState = Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate.WithdrawEntrypointState;
using DepositState = Pi.WalletService.Domain.AggregatesModel.DepositAggregate.DepositState;
using GlobalWalletTransferState = Pi.WalletService.IntegrationEvents.Models.GlobalWalletTransferState;
using WithdrawState = Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate.WithdrawState;
using DepositEntrypointMachineState = Pi.WalletService.IntegrationEvents.Models.DepositEntrypointState;
using Transaction = Pi.WalletService.Application.Models.Transaction;
using WithdrawEntrypointMachineState = Pi.WalletService.IntegrationEvents.Models.WithdrawEntrypointState;

namespace Pi.WalletService.Application.Queries;

[Obsolete("Use TransactionQueries V2")]
public class TransactionQueries : ITransactionQueries
{
    private readonly IFeatureService _featureService;
    private readonly IDepositEntrypointRepository _depositEntrypointRepository;
    private readonly IWithdrawEntrypointRepository _withdrawEntrypointRepository;
    private readonly IDepositRepository _depositRepository;
    private readonly IGlobalWalletDepositRepository _globalWalletDepositRepository;
    private readonly IWithdrawRepository _withdrawRepository;
    private readonly ICashDepositRepository _cashDepositRepository;
    private readonly IRefundRepository _refundRepository;
    private readonly ICashWithdrawRepository _cashWithdrawRepository;
    private readonly ITransactionHistoryService _transactionHistoryService;

    public TransactionQueries(
        IFeatureService featureService,
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        IDepositRepository depositRepository,
        IGlobalWalletDepositRepository globalWalletDepositRepository,
        IWithdrawRepository withdrawRepository,
        ICashDepositRepository cashDepositRepository,
        IRefundRepository refundRepository,
        ICashWithdrawRepository cashWithdrawRepository,
        ITransactionHistoryService transactionHistoryService)
    {
        _featureService = featureService;
        _depositEntrypointRepository = depositEntrypointRepository;
        _withdrawEntrypointRepository = withdrawEntrypointRepository;
        _depositRepository = depositRepository;
        _globalWalletDepositRepository = globalWalletDepositRepository;
        _withdrawRepository = withdrawRepository;
        _cashDepositRepository = cashDepositRepository;
        _refundRepository = refundRepository;
        _cashWithdrawRepository = cashWithdrawRepository;
        _transactionHistoryService = transactionHistoryService;
    }

    public async Task<T> GetTransactionByTransactionNo<T>(string userId, string transactionNo, Product product) where T : Transaction
    {
        T? transaction;
        switch (product)
        {
            case Product.Cash:
            case Product.CashBalance:
            case Product.CreditBalance:
            case Product.CreditBalanceSbl:
            case Product.Crypto:
            case Product.Derivatives:
            case Product.Funds:
                transaction = await GetTransactionByTransactionNoAndProduct(userId, product, transactionNo) as T;
                break;
            case Product.GlobalEquities:
                transaction = await GetGlobalTransferTransactionByTransactionNo(userId, transactionNo, product) as T;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(product), product, null);
        }

        if (transaction == null)
        {
            throw new InvalidDataException($"TransactionId {transactionNo} for Product {product.ToString()} Not Found");
        }

        return transaction;
    }

    public async Task<List<string>> GetFailedTransactionsByProductAndStatus(Product product, string status)
    {
        switch (product)
        {
            case Product.Cash:
            case Product.CashBalance:
            case Product.CreditBalance:
            case Product.CreditBalanceSbl:
            case Product.Crypto:
            case Product.Derivatives:
                throw new NotImplementedException();
            case Product.GlobalEquities:
                var transactions =
                    await _globalWalletDepositRepository.GetFailedTransactionGlobalTransfer(status);
                return transactions;
            case Product.Funds:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException(nameof(product), product, null);
        }
    }

    public async Task<List<ActiveTransaction>> GetActiveQrTransaction(string userId, Product product)
    {
        var transactions = await _depositRepository.GetActiveDeposit(userId, product);

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

    public async Task<List<DepositTransaction>> GetDepositTransactions(
        PaginateRequest paginateRequest,
        TransactionFilters? filters)
    {
        var queryFilter = filters != null
            ? EntityFactory.NewDepositStateFilters(filters)
            : null;
        var records = await _depositRepository.Get(
            paginateRequest.PageNo,
            paginateRequest.PageSize,
            paginateRequest.OrderBy,
            paginateRequest.OrderDirection,
            queryFilter);
        return records.Select(QueryFactory.NewDepositTransaction).ToList();
    }

    public async Task<List<WithdrawTransaction>> GetWithdrawTransactions(
        PaginateRequest paginateRequest,
        TransactionFilters? filters)
    {
        var withdrawFilters = filters != null
            ? EntityFactory.NewWithdrawStateFilters(filters)
            : null;
        var records = await _withdrawRepository.Get(
            paginateRequest.PageNo,
            paginateRequest.PageSize,
            paginateRequest.OrderBy,
            paginateRequest.OrderDirection,
            withdrawFilters);
        return records.Select(q => QueryFactory.NewWithdrawTransaction(q)).ToList();
    }

    public async Task<List<RefundTransaction>> GetRefundTransactions(
        PaginateRequest paginateRequest,
        TransactionFilters? filters)
    {
        var refundFilters = filters != null
            ? EntityFactory.NewRefundStateFilters(filters)
            : null;
        var records = await _refundRepository.Get(
            paginateRequest.PageNo,
            paginateRequest.PageSize,
            paginateRequest.OrderBy,
            paginateRequest.OrderDirection,
            refundFilters);
        return records.Select(QueryFactory.NewRefundTransaction).ToList();
    }

    public async Task<List<T>> GetTransactions<T>(
        PaginateRequest paginateRequest,
        TransactionFilters? filters,
        string? sid = null,
        string? deviceId = null,
        string? device = null,
        string? accountId = null) where T : Transaction
    {
        var depositStates = new List<DepositState>();
        var withdrawStates = new List<WithdrawState>();

        if (filters?.TransactionType is TransactionType.Deposit or null)
        {
            var depositStateFilters = filters != null
                ? EntityFactory.NewDepositStateFilters(filters)
                : null;

            depositStates = await _depositRepository
                .Get(
                    1,
                    paginateRequest.PageSize * paginateRequest.PageNo,
                    paginateRequest.OrderBy,
                    paginateRequest.OrderDirection,
                    depositStateFilters);
        }

        if (filters?.TransactionType is TransactionType.Withdraw or null)
        {
            var withdrawStateFilters = filters != null
                ? EntityFactory.NewWithdrawStateFilters(filters)
                : null;

            withdrawStates = await _withdrawRepository
                .Get(
                    1,
                    paginateRequest.PageSize * paginateRequest.PageNo,
                    paginateRequest.OrderBy,
                    paginateRequest.OrderDirection,
                    withdrawStateFilters);
        }

        if (filters is { Product: Product.GlobalEquities })
        {
            var globalStateFilter = EntityFactory.NewGlobalStateFilter(filters);

            var globalTransferRecord =
                (await _globalWalletDepositRepository.Get(paginateRequest.PageNo, paginateRequest.PageSize, paginateRequest.OrderBy, paginateRequest.OrderDirection, globalStateFilter))
                .Select(t =>
                {
                    if (t.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.DepositProcessing) ||
                        t.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.DepositFailed) ||
                        t.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.FxTransferFailed) ||
                        t.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.FxTransferInsufficientBalance) ||
                        t.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.WithdrawalProcessing) ||
                        t.CurrentState == "Final")
                    {
                        return t.TransactionType == TransactionType.Deposit
                            ? MapGlobalTransferTransactionToTransaction(t.TransactionNo ?? string.Empty, t, depositStates.Find(d => d.TransactionNo == t.TransactionNo))
                            : MapGlobalTransferTransactionToTransaction(t.TransactionNo ?? string.Empty, t, null, withdrawStates.Find(d => d.TransactionNo == t.TransactionNo));
                    }

                    return MapGlobalTransferTransactionToTransaction(t.TransactionNo ?? string.Empty, t);
                })
                .ToList();

            return (globalTransferRecord as List<T>)!;
        }

        var transactions = new List<Transaction>();

        if (filters?.TransactionType is TransactionType.Deposit or null)
        {
            var cashDepositFilters = filters != null
                ? EntityFactory.NewCashDepositFilters(filters)
                : null;
            var cashDepositRecords = (await _cashDepositRepository.Get(
                    1,
                    paginateRequest.PageSize * paginateRequest.PageNo,
                    paginateRequest.OrderBy,
                    paginateRequest.OrderDirection,
                    cashDepositFilters))
                .Select(QueryFactory.NewDepositTransaction)
                .ToList();

            var depositRecords = depositStates.Select(d =>
                {
                    if (d.CurrentState == IntegrationEvents.Models.DepositState.GetName(() => IntegrationEvents.Models.DepositState.DepositCompleted))
                    {
                        d.CurrentState = cashDepositRecords.Find(t => d.TransactionNo == t.TransactionNo)?.CurrentState;
                    }
                    return QueryFactory.NewTransaction(d);
                })
                .ToList();
            transactions.AddRange(depositRecords);
        }

        if (filters?.TransactionType is TransactionType.Withdraw or null)
        {
            var withdrawStateFilters = filters != null
                ? EntityFactory.NewCashWithdrawStateFilters(filters)
                : null;

            var withdrawRecords = (await _cashWithdrawRepository.Get(
                    1,
                    paginateRequest.PageSize * paginateRequest.PageNo,
                    paginateRequest.OrderBy,
                    paginateRequest.OrderDirection,
                    withdrawStateFilters))
                .Select(w =>
                {
                    w.RequestedAmount = withdrawStates.Find(ws => ws.TransactionNo == w.TransactionNo)?.PaymentDisbursedAmount ?? w.RequestedAmount;
                    return QueryFactory.NewTransaction(w);
                })
                .ToList();

            transactions.AddRange(withdrawRecords);
        }

        if (!string.IsNullOrWhiteSpace(sid) &&
            !string.IsNullOrWhiteSpace(deviceId) &&
            !string.IsNullOrWhiteSpace(filters?.UserId) &&
            filters is { Product: not null, TransactionType: not null } &&
            !string.IsNullOrWhiteSpace(accountId))
        {
            var now = DateTime.Now.AddHours(7);

            var outSourceTransaction =
                await _transactionHistoryService.GetTransactionHistory(
                    new SiriusAuthentication(sid, deviceId, device),
                    accountId,
                    (Product)filters.Product,
                    (TransactionType)filters.TransactionType,
                    new PaginateRequest(0, paginateRequest.PageSize * paginateRequest.PageNo, null, null),
                    DateOnly.FromDateTime(filters.CreatedAtFrom ?? now.AddMonths(-3)),
                    DateOnly.FromDateTime(filters.CreatedAtTo ?? now)
                );

            transactions.AddRange(outSourceTransaction);
        }

        return (paginateRequest.OrderDirection?.ToLower() == "desc"
            ? transactions.OrderByDescending(t => paginateRequest.OrderBy == "PaymentReceivedDateTime"
                ? t.PaymentReceivedDateTime
                : t.CreatedAt).ToList() as List<T>
            : transactions.OrderBy(t => paginateRequest.OrderBy == "PaymentReceivedDateTime"
                    ? t.PaymentReceivedDateTime
                    : t.CreatedAt).Take(new Range(paginateRequest.PageSize - (paginateRequest.PageSize * paginateRequest.PageNo), paginateRequest.PageSize * paginateRequest.PageNo))
                .ToList() as List<T>)!;
    }

    public async Task<int> CountTransactions(TransactionFilters? filters)
    {
        var count = 0;

        if (filters is { Product: Product.GlobalEquities })
        {
            var globalStateFilter = EntityFactory.NewGlobalStateFilter(filters);
            var globalTransferCount = await _globalWalletDepositRepository.CountTransactions(globalStateFilter);

            return globalTransferCount;
        }

        if (filters?.TransactionType is TransactionType.Deposit or null)
        {
            var depositStateFilters = filters != null
                ? EntityFactory.NewDepositStateFilters(filters)
                : null;
            count += await _depositRepository.CountTransactions(depositStateFilters);
        }

        if (filters?.TransactionType is TransactionType.Withdraw or null)
        {
            var withdrawFilters = filters != null
                ? EntityFactory.NewWithdrawStateFilters(filters)
                : null;
            count += await _withdrawRepository.CountTransactions(withdrawFilters);
        }

        return count;
    }

    public async Task<int> CountDepositTransactions(TransactionFilters? filters)
    {
        var depositStateFilters = filters != null
            ? EntityFactory.NewDepositStateFilters(filters)
            : null;
        return await _depositRepository.CountTransactions(depositStateFilters);
    }

    public async Task<int> CountWithdrawTransactions(TransactionFilters? filters)
    {
        var withdrawFilters = filters != null
            ? EntityFactory.NewWithdrawStateFilters(filters)
            : null;
        return await _withdrawRepository.CountTransactions(withdrawFilters);
    }

    public async Task<int> CountRefundTransactions(TransactionFilters? filters)
    {
        var refundFilters = filters != null
            ? EntityFactory.NewRefundStateFilters(filters)
            : null;
        return await _refundRepository.CountTransactions(refundFilters);
    }

    public async Task<CashDepositState?> GetCashDepositTransaction(string transactionNo)
    {
        return await _cashDepositRepository.Get(transactionNo);
    }

    public async Task<DepositEntrypointState?> GetDepositEntrypointTransaction(string transactionNo)
    {
        return await _depositEntrypointRepository.GetByTransactionNo(transactionNo);
    }

    public async Task<WithdrawEntrypointState?> GetWithdrawEntrypointTransaction(string transactionNo)
    {
        return await _withdrawEntrypointRepository.GetByTransactionNo(transactionNo);
    }

    public async Task<CashWithdrawState?> GetCashWithdrawTransaction(string transactionNo)
    {
        return await _cashWithdrawRepository.Get(transactionNo);
    }

    public async Task<T?> GetDepositTransactionByTransactionNo<T>(string transactionNo) where T : class
    {
        var depositState = await _depositRepository.GetByTransactionNo(transactionNo);

        if (depositState == null)
        {
            return null;
        }

        if (typeof(T) == typeof(GlobalTransaction))
        {
            var globalTransfer = await _globalWalletDepositRepository.GetByTransactionNo(transactionNo);
            return globalTransfer == null
                ? null
                : MapGlobalTransferTransactionToTransaction(transactionNo, globalTransfer, depositState) as T;
        }

        return QueryFactory.NewDepositTransaction(depositState) as T;
    }

    public async Task<T?> GetWithdrawTransactionByTransactionNo<T>(string transactionNo) where T : class
    {
        var withdrawState = await _withdrawRepository.GetByTransactionNo(transactionNo);

        if (withdrawState == null) return null;

        if (typeof(T) == typeof(GlobalTransaction))
        {
            var globalTransfer = await _globalWalletDepositRepository.GetByTransactionNo(transactionNo);
            return globalTransfer == null
                ? null
                : MapGlobalTransferTransactionToTransaction(transactionNo, globalTransfer, null, withdrawState) as T;
        }

        return QueryFactory.NewWithdrawTransaction(withdrawState) as T;
    }

    private async Task<Transaction> GetTransactionByTransactionNoAndProduct(string userId, Product product, string transactionNo)
    {
        var transactionType = transactionNo.Contains("DP")
            ? TransactionType.Deposit
            : TransactionType.Withdraw;
        string? currentState;

        if (transactionType == TransactionType.Deposit)
        {
            var depositState = await _depositRepository.GetByTransactionNo(userId, product, transactionNo);

            if (depositState == null)
            {
                throw new InvalidDataException();
            }

            currentState = depositState.CurrentState;

            // ReSharper disable once InvertIf doesnt make sense
            if (depositState.CurrentState == IntegrationEvents.Models.DepositState.GetName(() => IntegrationEvents.Models.DepositState.DepositCompleted))
            {
                var cashDepositState = await _cashDepositRepository.Get(userId, product, transactionNo);
                if (cashDepositState == null)
                {
                    throw new InvalidDataException();
                }

                currentState = cashDepositState.CurrentState;
            }

            return new Transaction(
                depositState.CorrelationId,
                depositState.UserId,
                depositState.AccountCode,
                depositState.CustomerCode,
                depositState.CustomerName,
                currentState,
                QueryFactory.GetTransactionStatus(depositState.CurrentState ?? string.Empty, TransactionType.Deposit, depositState.Product).ToString(),
                depositState.TransactionNo,
                TransactionType.Deposit,
                depositState.RequestedAmount,
                Currency.THB,
                depositState.QrValue,
                depositState.DepositQrGenerateDateTime,
                depositState.PaymentReceivedAmount ?? 0,
                depositState.PaymentReceivedDateTime,
                null,
                depositState.Channel,
                depositState.Product,
                depositState.BankAccountNo,
                depositState.BankAccountName,
                depositState.BankName,
                depositState.BankFee,
                depositState.CreatedAt.ToUniversalTime(),
                null
            );
        }

        WithdrawState? withdrawState = null;
        var cashWithdrawState = await _cashWithdrawRepository.Get(userId, product, transactionNo);
        if (cashWithdrawState == null)
        {
            throw new InvalidDataException();
        }

        currentState = cashWithdrawState.CurrentState;
        if (currentState ==
            CashWithdrawStateName.GetName(() =>
                CashWithdrawStateName.WithdrawalProcessing) || currentState == "Final")
        {
            withdrawState = await _withdrawRepository.GetByTransactionNo(userId, product, transactionNo);
            if (withdrawState == null)
            {
                throw new InvalidDataException();
            }
            currentState = withdrawState.CurrentState;
        }

        return new Transaction(
            cashWithdrawState.CorrelationId,
            cashWithdrawState.UserId,
            cashWithdrawState.AccountCode,
            cashWithdrawState.CustomerCode,
            null,
            currentState,
            QueryFactory.GetTransactionStatus(currentState ?? string.Empty, TransactionType.Withdraw, cashWithdrawState.Product).ToString(),
            cashWithdrawState.TransactionNo,
            TransactionType.Withdraw,
            withdrawState?.PaymentDisbursedAmount ?? cashWithdrawState.RequestedAmount,
            Currency.THB,
            null,
            null,
            null,
            null,
            withdrawState?.PaymentDisbursedDateTime,
            cashWithdrawState.Channel,
            cashWithdrawState.Product,
            cashWithdrawState.BankAccountNo,
            null,
            cashWithdrawState.BankName,
            withdrawState?.BankFee,
            cashWithdrawState.CreatedAt.ToUniversalTime(),
            withdrawState?.PaymentConfirmedAmount
        );
    }

    private async Task<GlobalTransaction> GetGlobalTransferTransactionByTransactionNo(string userId, string transactionNo, Product product)
    {
        var globalWalletTransferState = await _globalWalletDepositRepository.Get(userId, transactionNo);

        if (globalWalletTransferState == null)
        {
            throw new InvalidDataException(
                $"TransactionId {transactionNo} for Product {product.ToString()} Not Found");
        }

        DepositState? depositState = null;
        WithdrawState? withdrawState = null;

        if (globalWalletTransferState.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.DepositProcessing) ||
            globalWalletTransferState.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.DepositFailed) ||
            globalWalletTransferState.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.FxTransferFailed) ||
            globalWalletTransferState.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.FxTransferInsufficientBalance) ||
            globalWalletTransferState.CurrentState == "Final")
        {
            depositState = await _depositRepository.Get(userId, transactionNo);
        }

        if (globalWalletTransferState.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.WithdrawalProcessing) ||
            globalWalletTransferState.CurrentState == "Final")
        {
            withdrawState = await _withdrawRepository.Get(userId, transactionNo);
        }

        return MapGlobalTransferTransactionToTransaction(transactionNo, globalWalletTransferState, depositState, withdrawState);
    }

    private static List<T>? MapEntrypointToTransactions<T>(
        List<DepositEntrypointTransaction> depositEntrypointTransaction,
        List<WithdrawEntrypointTransaction> withdrawEntrypointTransaction)
    {
        return depositEntrypointTransaction.Select(t =>
            {
                string? currentState;
                string entrypointState = t.DepositEntrypoint!.CurrentState!;
                if (entrypointState == DepositEntrypointMachineState.GetName(() => DepositEntrypointMachineState.DepositProcessing))
                {
                    currentState = t.DepositEntrypoint.Channel == Channel.QR
                        ? t.QrDeposit!.CurrentState
                        : t.OddDeposit!.CurrentState;

                }
                else if (entrypointState == DepositEntrypointMachineState.GetName(() => DepositEntrypointMachineState.UpBackProcessing))
                {
                    currentState = t.UpBack!.CurrentState;
                }
                else
                {
                    currentState = entrypointState;
                }

                var channel = t.DepositEntrypoint.Channel;

                return new Transaction(
                    t.DepositEntrypoint!.CorrelationId,
                    t.DepositEntrypoint.UserId,
                    t.DepositEntrypoint.AccountCode,
                    t.DepositEntrypoint.CustomerCode,
                    t.DepositEntrypoint.CustomerName ?? string.Empty,
                    currentState ?? string.Empty,
                    QueryFactory.GetTransactionStatus(currentState ?? string.Empty, TransactionType.Deposit, t.DepositEntrypoint.Product).ToString(),
                    t.DepositEntrypoint.TransactionNo ?? string.Empty,
                    TransactionType.Deposit,
                    t.DepositEntrypoint.RequestedAmount,
                    Currency.THB,
                    channel == Channel.QR ? t.QrDeposit?.QrValue : null,
                    channel == Channel.QR ? t.QrDeposit?.DepositQrGenerateDateTime : null,
                    channel == Channel.QR ? t.QrDeposit?.PaymentReceivedAmount : t.OddDeposit!.PaymentReceivedAmount,
                    channel == Channel.QR ? t.QrDeposit?.PaymentReceivedDateTime : t.OddDeposit!.PaymentReceivedDateTime,
                    null,
                    channel,
                    t.DepositEntrypoint.Product,
                    t.DepositEntrypoint.BankAccountNo,
                    t.DepositEntrypoint.BankAccountName,
                    t.DepositEntrypoint.BankName,
                    channel == Channel.QR ? t.QrDeposit!.Fee : t.OddDeposit!.Fee,
                    t.DepositEntrypoint.CreatedAt.ToUniversalTime(),
                    null
                );
            }).Concat(withdrawEntrypointTransaction.Select(t =>
            {
                string? currentState;
                string entrypointState = t.WithdrawEntrypoint!.CurrentState!;
                if (entrypointState == WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.WithdrawProcessing) ||
                    entrypointState == WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.WithdrawValidating))
                {
                    currentState = t.OddWithdraw!.CurrentState;
                }
                else if (entrypointState == WithdrawEntrypointMachineState.GetName(() => WithdrawEntrypointMachineState.UpBackProcessing))
                {
                    currentState = t.UpBack!.CurrentState;
                }
                else
                {
                    currentState = entrypointState;
                }

                return new Transaction(
                    t.WithdrawEntrypoint!.CorrelationId,
                    t.WithdrawEntrypoint.UserId,
                    t.WithdrawEntrypoint.AccountCode,
                    t.WithdrawEntrypoint.CustomerCode,
                    t.WithdrawEntrypoint.CustomerName ?? string.Empty,
                    currentState ?? string.Empty,
                    QueryFactory.GetTransactionStatus(currentState ?? string.Empty, TransactionType.Withdraw, t.WithdrawEntrypoint.Product).ToString(),
                    t.WithdrawEntrypoint.TransactionNo ?? string.Empty,
                    TransactionType.Withdraw,
                    t.WithdrawEntrypoint.RequestedAmount,
                    Currency.THB,
                    null,
                    null,
                    null,
                    null,
                    t.OddWithdraw!.PaymentDisbursedDateTime,
                    t.WithdrawEntrypoint.Channel,
                    t.WithdrawEntrypoint.Product,
                    t.WithdrawEntrypoint.BankAccountNo,
                    t.WithdrawEntrypoint.BankAccountName,
                    t.WithdrawEntrypoint.BankName,
                    t.OddWithdraw.Fee,
                    t.WithdrawEntrypoint.CreatedAt.ToUniversalTime(),
                    t.WithdrawEntrypoint.RequestedAmount
                );
            }))
            .Where(t => (t.Status != "Pending" || !string.IsNullOrEmpty(GetFailedReason(t.CurrentState))) && t.CurrentState != "TransferRequestFailed" && t.CurrentState != "RevertTransferSuccess")
            .OrderBy(t => t.Status)
            .ThenByDescending(t => t.TransactionType)
            .ThenBy(t => t.TransactionType == TransactionType.Deposit
                ? t.PaymentReceivedDateTime
                : t.PaymentDisbursedDateTime)
            .ToList() as List<T>;
    }

    private static GlobalTransaction MapGlobalTransferTransactionToTransaction(
        string transactionNo,
        Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState globalWalletTransferState,
        DepositState? depositState = null,
        WithdrawState? withdrawState = null)
    {
        var transactionType = globalWalletTransferState.TransactionType;
        var exchangeRate = globalWalletTransferState.FxConfirmedExchangeRate is null or 0
            ? globalWalletTransferState.RequestedFxAmount
            : globalWalletTransferState.FxConfirmedExchangeRate;

        var foreignCurrency = new List<Currency?>
        {
            globalWalletTransferState.TransferCurrency,
            globalWalletTransferState.FxConfirmedCurrency,
            globalWalletTransferState.RequestedFxCurrency,
            globalWalletTransferState.RequestedCurrency
        }.Find(currency => currency != null && currency != Currency.THB);

        var (currentState, requestedAmount, transferAmount, accountNo, bankName, bankAccountNo, bankAccountName, channel, bankFee) = transactionType switch
        {
            TransactionType.Deposit => GetCurrentStateGlobalDeposit(
                transactionNo,
                globalWalletTransferState,
                depositState,
                foreignCurrency,
                transactionType,
                exchangeRate),
            TransactionType.Withdraw => GetCurrentStateForGlobalWithdraw(
                transactionNo,
                globalWalletTransferState,
                withdrawState,
                foreignCurrency,
                transactionType,
                exchangeRate),
            _ => throw new ArgumentOutOfRangeException(transactionType.ToString())
        };

        return new GlobalTransaction(
            globalWalletTransferState.CorrelationId,
            globalWalletTransferState.UserId,
            accountNo ?? string.Empty,
            globalWalletTransferState.CustomerCode,
            depositState?.CustomerName ?? string.Empty,
            currentState ?? string.Empty,
            QueryFactory.GetTransactionStatus(currentState ?? string.Empty, transactionType, Product.GlobalEquities).ToString(),
            globalWalletTransferState.TransactionNo ?? string.Empty,
            transactionType,
            requestedAmount ?? 0,
            globalWalletTransferState.RequestedCurrency,
            depositState?.QrValue,
            depositState?.DepositQrGenerateDateTime,
            transactionType == TransactionType.Deposit && depositState?.DepositQrGenerateDateTime != null
                ? depositState.DepositQrGenerateDateTime!.Value.AddMinutes(depositState.QrCodeExpiredTimeInMinute).ToUniversalTime()
                : null,
            depositState?.PaymentReceivedAmount ?? globalWalletTransferState.PaymentReceivedAmount, // GlobalWalletTransferState.PaymentReceivedAmount = DepositPaymentReceivedAmount - DepositFee
            depositState?.PaymentReceivedDateTime,
            withdrawState?.PaymentDisbursedDateTime,
            channel ?? Channel.QR,
            globalWalletTransferState.Product,
            bankAccountNo,
            bankAccountName,
            bankName,
            globalWalletTransferState.CreatedAt.ToUniversalTime(),
            globalWalletTransferState.RequestedFxAmount,
            globalWalletTransferState.TransactionType == TransactionType.Deposit
                ? foreignCurrency
                : Currency.THB,
            transactionType == TransactionType.Deposit
                ? globalWalletTransferState.FxConfirmedExchangeRate
                : globalWalletTransferState.RequestedFxAmount,
            globalWalletTransferState.FxInitiateRequestDateTime,
            globalWalletTransferState.FxConfirmedDateTime,
            globalWalletTransferState.FxConfirmedAmount,
            transactionType == TransactionType.Withdraw
                ? globalWalletTransferState.TransferFromAccount ?? globalWalletTransferState.GlobalAccount
                : globalWalletTransferState.TransferFromAccount,
            transactionType == TransactionType.Deposit
                ? globalWalletTransferState.TransferToAccount ?? globalWalletTransferState.GlobalAccount
                : globalWalletTransferState.TransferToAccount,
            transferAmount,
            globalWalletTransferState.TransferFee,
            globalWalletTransferState.TransferRequestTime,
            globalWalletTransferState.TransferCompleteTime,
            globalWalletTransferState.RefundAmount ?? 0,
            globalWalletTransferState.NetAmount ?? globalWalletTransferState.PaymentReceivedAmount ?? depositState?.PaymentReceivedAmount,
            GetFailedReason(currentState),
            bankFee,
            globalWalletTransferState.TransferFee ?? 0,
            transactionType == TransactionType.Deposit
                ? depositState?.Amount
                : withdrawState?.PaymentConfirmedAmount
        );
    }

    private static (string?, decimal?, decimal?, string?, string?, string?, string?, Channel?, decimal?) GetCurrentStateGlobalDeposit(
        string transactionNo,
        Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState globalWalletTransferState,
        DepositState? depositState,
        Currency? foreignCurrency,
        TransactionType transactionType,
        decimal? exchangeRate)
    {
        string? currentState;
        string? accountNo = null;
        string? bankName = null;
        string? bankAccountNo = null;
        string? bankAccountName = null;
        Channel? channel = Channel.QR;
        decimal? requestedAmount = globalWalletTransferState.RequestedAmount;
        decimal? bankFee = null;
        if (foreignCurrency == null)
        {
            throw new InvalidDataException(
                $"TransactionId {transactionNo} for Product {globalWalletTransferState.Product.ToString()}; Invalid foreign currency");
        }
        var (_, transferCurrency) = (Currency.THB, foreignCurrency.Value);

        if (globalWalletTransferState.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.DepositProcessing) ||
            (globalWalletTransferState.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.DepositFailed) && depositState != null))
        {
            if (depositState == null)
            {
                // handle case where deposit state doesn't get created in time
                if (globalWalletTransferState.CurrentState == "DepositProcessing")
                {
                    currentState = globalWalletTransferState.CurrentState;
                    return (currentState, requestedAmount, null, accountNo, bankName, bankAccountNo, bankAccountName, channel, bankFee);
                }

                throw new InvalidDataException(
                    $"TransactionId {transactionNo} for Product {globalWalletTransferState.Product.ToString()} Not Found");
            }

            currentState = depositState.CurrentState;
            bankFee = depositState.BankFee;
        }
        else
        {
            currentState = globalWalletTransferState.CurrentState;
        }

        if (depositState != null)
        {
            accountNo = depositState.AccountCode;
            bankName = depositState.BankName;
            bankAccountNo = depositState.BankAccountNo;
            bankAccountName = depositState.BankAccountName;
            channel = depositState.Channel;
            bankFee = depositState.BankFee;
        }

        var transferAmount = globalWalletTransferState.TransferAmount
                             ?? globalWalletTransferState.FxConfirmedAmount
                             ?? RoundingUtils.RoundExchangeTransaction(
                                 transactionType,
                                 globalWalletTransferState.RequestedCurrency,
                                 globalWalletTransferState.RequestedAmount,
                                 transferCurrency,
                                 exchangeRate!.Value
                             );

        return (currentState, requestedAmount, transferAmount, accountNo, bankName, bankAccountNo, bankAccountName, channel, bankFee);
    }

    private static (string?, decimal?, decimal?, string?, string?, string?, string?, Channel?, decimal?) GetCurrentStateForGlobalWithdraw(
        string transactionNo,
        Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState globalWalletTransferState,
        WithdrawState? withdrawState,
        Currency? foreignCurrency,
        TransactionType transactionType,
        decimal? exchangeRate)
    {
        string? currentState;
        var accountNo = withdrawState?.AccountCode;
        var bankName = withdrawState?.BankName;
        var bankAccountNo = withdrawState?.BankAccountNo;
        Channel? channel = withdrawState?.Channel ?? Channel.OnlineViaKKP;
        var bankFee = withdrawState?.BankFee ?? null;

        if (globalWalletTransferState.CurrentState == GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.WithdrawalProcessing))
        {
            if (withdrawState == null)
            {
                // handle case where deposit state doesn't get created in time
                if (globalWalletTransferState.CurrentState == "WithdrawalProcessing")
                {
                    currentState = globalWalletTransferState.CurrentState;
                    return (currentState, null, null, accountNo, bankName, bankAccountNo, null, channel, bankFee);
                }

                throw new InvalidDataException(
                    $"TransactionId {transactionNo} for Product {globalWalletTransferState.Product.ToString()} Not Found");
            }

            bankFee = withdrawState.BankFee;
            currentState = withdrawState.CurrentState;
        }
        else
        {
            currentState = globalWalletTransferState.CurrentState;
        }

        var (requestCurrency, transferCurrency) = (foreignCurrency, Currency.THB);
        var requestedAmount = requestCurrency == null
            ? 0
            : RoundingUtils.RoundExchangeTransaction(
                transactionType,
                globalWalletTransferState.RequestedCurrency,
                globalWalletTransferState.RequestedAmount,
                requestCurrency.Value,
                exchangeRate!.Value,
                roundExchangeRate: false
            );

        var transferAmount = globalWalletTransferState.TransferAmount == null
            ? 0
            : withdrawState?.PaymentDisbursedAmount ?? RoundingUtils.RoundExchangeTransaction(
                transactionType,
                globalWalletTransferState.TransferCurrency!.Value,
                globalWalletTransferState.TransferAmount!.Value,
                transferCurrency,
                globalWalletTransferState.RequestedFxAmount,
                roundExchangeRate: false
            );

        return (currentState, requestedAmount, transferAmount, accountNo, bankName, bankAccountNo, null, channel, bankFee);
    }

    private static List<T>? GetAllTransactionsByDate<T>(
        IEnumerable<DepositState> depositStateList,
        IEnumerable<WithdrawState> withdrawStateList) where T : Transaction
    {
        return depositStateList
            .Select(d =>
                new Transaction(
                    d.CorrelationId,
                    d.UserId,
                    d.AccountCode,
                    d.CustomerCode,
                    d.CustomerName,
                    d.CurrentState,
                    QueryFactory.GetTransactionStatus(d.CurrentState ?? string.Empty, TransactionType.Deposit, d.Product).ToString(),
                    d.TransactionNo,
                    TransactionType.Deposit,
                    d.RequestedAmount,
                    Currency.THB,
                    d.QrValue,
                    d.DepositQrGenerateDateTime,
                    d.PaymentReceivedAmount,
                    d.PaymentReceivedDateTime,
                    null,
                    d.Channel,
                    d.Product,
                    d.BankAccountNo,
                    d.BankAccountName,
                    d.BankName,
                    d.BankFee,
                    d.CreatedAt,
                    null
                ))
            .Concat(
                withdrawStateList
                    .Select(w =>
                        new Transaction(
                            w.CorrelationId,
                            w.UserId,
                            w.AccountCode,
                            w.CustomerCode,
                            null,
                            w.CurrentState,
                            QueryFactory.GetTransactionStatus(w.CurrentState ?? string.Empty, TransactionType.Withdraw, w.Product).ToString(),
                            w.TransactionNo,
                            TransactionType.Withdraw,
                            w.PaymentDisbursedAmount!.Value,
                            Currency.THB,
                            null,
                            w.PaymentDisbursedDateTime,
                            null,
                            null,
                            w.PaymentDisbursedDateTime,
                            w.Channel,
                            w.Product,
                            w.BankAccountNo,
                            null,
                            w.BankName,
                            w.BankFee,
                            w.CreatedAt,
                            w.PaymentConfirmedAmount
                        ))) as List<T>;
    }

    private async Task<List<T>?> GetAllGlobalEquityTransactionsByDate<T>(
        DateTime startDateTime,
        DateTime endDateTime,
        IEnumerable<DepositState> depositStateList,
        IEnumerable<WithdrawState> withdrawStateList) where T : Transaction
    {
        var globalWalletDepositStateList = await _globalWalletDepositRepository.GetListByDateTime(startDateTime, endDateTime);

        var depositTransactions = globalWalletDepositStateList
            .Join(
                depositStateList, globalState => new { globalState.TransactionNo, globalState.TransactionType },
                depositState => new { depositState.TransactionNo, TransactionType = TransactionType.Deposit },
                (globalState, depositState) => MapGlobalTransferTransactionToTransaction(globalState.TransactionNo!, globalState, depositState));


        var withdrawTransaction = globalWalletDepositStateList.Join(withdrawStateList, globalState => new { globalState.TransactionNo, globalState.TransactionType },
            withdrawState => new { withdrawState.TransactionNo, TransactionType = TransactionType.Withdraw },
            (globalState, withdrawState) => MapGlobalTransferTransactionToTransaction(globalState.TransactionNo!, globalState, withdrawState: withdrawState));

        return depositTransactions
            .Concat(withdrawTransaction)
            .Where(t => (t.Status != "Pending" || !string.IsNullOrEmpty(t.FailedReason)) && t.CurrentState != "TransferRequestFailed" && t.CurrentState != "RevertTransferSuccess")
            .OrderBy(t => t.Status)
            .ThenByDescending(t => t.TransactionType)
            .ThenBy(t => t.TransactionType == TransactionType.Deposit
                ? t.PaymentReceivedDateTime
                : t.PaymentDisbursedDateTime)
            .ToList() as List<T>;
    }

    private static string GetFailedReason(string? currentState)
    {
        return currentState switch
        {
            "FxTransferFailed" or "ManualAllocationInprogress" or "ManualAllocationFailed" => "Manual allocation in XNT",
            "FxTransferInsufficientBalance" => "Insufficient Balance",
            "DepositFailedAmountMismatch" => "Expect Amount and Received Amount Mismatch",
            "DepositFailedNameMismatch" => "Name Mismatch",
            "DepositFailedInvalidSource" => "Incorrect Source",
            "FxFailed" => "Unable to fx",
            "FxRateCompareFailed" => "Unfavorable fx (rate over)",
            "RevertTransferFailed" => "Manual allocation in XNT",
            _ => string.Empty
        };
    }
}
