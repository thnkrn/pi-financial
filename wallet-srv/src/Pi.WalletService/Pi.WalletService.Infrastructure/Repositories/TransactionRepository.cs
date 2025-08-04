using Microsoft.EntityFrameworkCore;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.AtsWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletManualAllocationAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.RecoveryAggregate;
using Pi.WalletService.Domain.AggregatesModel.RefundInfoAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Infrastructure.Extensions;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using DepositEntrypointStates = Pi.WalletService.IntegrationEvents.Models.DepositEntrypointState;
using WithdrawEntrypointStates = Pi.WalletService.IntegrationEvents.Models.WithdrawEntrypointState;

namespace Pi.WalletService.Infrastructure.Repositories;

public class TransactionRepository : ITransactionRepository
{
    private readonly WalletDbContext _walletDbContext;

    public TransactionRepository(WalletDbContext walletDbContext)
    {
        _walletDbContext = walletDbContext;
    }

    public async Task<Transaction?> GetByCorrelationId(Guid correlationId)
    {
        return await (from depositEntrypointState in _walletDbContext.Set<DepositEntrypointState>()
                .Where(d => d.CorrelationId == correlationId).DefaultIfEmpty()
                      from withdrawEntrypointState in _walletDbContext.Set<WithdrawEntrypointState>()
                          .Where(d => d.CorrelationId == correlationId).DefaultIfEmpty()
                      from oddDepositState in _walletDbContext.Set<OddDepositState>()
                          .Where(o =>
                              (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                              (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                          .DefaultIfEmpty()
                      from atsDepositState in _walletDbContext.Set<AtsDepositState>()
                          .Where(o =>
                              (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                              (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                          .DefaultIfEmpty()
                      from qrDepositState in _walletDbContext.Set<QrDepositState>()
                          .Where(o =>
                              (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                              (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                          .DefaultIfEmpty()
                      from oddWithdrawState in _walletDbContext.Set<OddWithdrawState>()
                          .Where(o =>
                              (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                              (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                          .DefaultIfEmpty()
                      from atsWithdrawState in _walletDbContext.Set<AtsWithdrawState>()
                          .Where(o =>
                              (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                              (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                          .DefaultIfEmpty()
                      from upBackState in _walletDbContext.Set<UpBackState>()
                          .Where(o =>
                              (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                              (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                          .DefaultIfEmpty()
                      from globalTransferState in _walletDbContext.Set<GlobalTransferState>()
                          .Where(o =>
                              (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                              (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                          .DefaultIfEmpty()
                      from recoveryState in _walletDbContext.Set<RecoveryState>()
                          .Where(o =>
                              (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                              (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                          .DefaultIfEmpty()
                      from refundInfo in _walletDbContext.Set<RefundInfo>()
                          .Where(r => depositEntrypointState != null
                                      && r.TicketId == depositEntrypointState.CorrelationId
                                      && r.CurrentState != RefundStatus.RefundFailed.ToString())
                          .OrderByDescending(r => r.CreatedAt)
                          .Take(1)
                          .DefaultIfEmpty()
                      from globalManualAllocate in _walletDbContext.Set<GlobalManualAllocationState>()
                          .Where(o =>
                              (depositEntrypointState != null &&
                               o.CorrelationId == depositEntrypointState.GlobalManualAllocateId) ||
                              (withdrawEntrypointState != null &&
                               o.CorrelationId == withdrawEntrypointState.GlobalManualAllocateId))
                          .DefaultIfEmpty()
                      select depositEntrypointState != null
                          ? new Transaction(
                              depositEntrypointState.CorrelationId,
                              depositEntrypointState.CurrentState,
                              depositEntrypointState.TransactionNo,
                              depositEntrypointState.UserId,
                              depositEntrypointState.AccountCode,
                              depositEntrypointState.CustomerCode,
                              depositEntrypointState.Channel,
                              depositEntrypointState.Product,
                              depositEntrypointState.Purpose,
                              depositEntrypointState.RequestedAmount,
                              depositEntrypointState.NetAmount,
                              depositEntrypointState.CustomerName,
                              depositEntrypointState.BankAccountName,
                              depositEntrypointState.BankAccountNo,
                              depositEntrypointState.BankName,
                              depositEntrypointState.BankCode,
                              depositEntrypointState.ResponseAddress,
                              depositEntrypointState.RequestId,
                              depositEntrypointState.RequesterDeviceId,
                              TransactionType.Deposit,
                              depositEntrypointState.CreatedAt,
                              depositEntrypointState.UpdatedAt
                          )
                          {
                              DepositEntrypoint = depositEntrypointState,
                              QrDeposit = qrDepositState,
                              OddDeposit = oddDepositState,
                              OddWithdraw = oddWithdrawState,
                              AtsDeposit = atsDepositState,
                              AtsWithdraw = atsWithdrawState,
                              UpBack = upBackState,
                              GlobalTransfer = globalTransferState,
                              RefundInfo = refundInfo,
                              GlobalManualAllocate = globalManualAllocate
                          }
                          : withdrawEntrypointState != null
                              ? new Transaction(
                                  withdrawEntrypointState.CorrelationId,
                                  withdrawEntrypointState.CurrentState,
                                  withdrawEntrypointState.TransactionNo,
                                  withdrawEntrypointState.UserId,
                                  withdrawEntrypointState.AccountCode,
                                  withdrawEntrypointState.CustomerCode,
                                  withdrawEntrypointState.Channel,
                                  withdrawEntrypointState.Product,
                                  withdrawEntrypointState.Purpose,
                                  withdrawEntrypointState.RequestedAmount,
                                  withdrawEntrypointState.NetAmount,
                                  withdrawEntrypointState.CustomerName,
                                  withdrawEntrypointState.BankAccountName,
                                  withdrawEntrypointState.BankAccountNo,
                                  withdrawEntrypointState.BankName,
                                  withdrawEntrypointState.BankCode,
                                  withdrawEntrypointState.ResponseAddress,
                                  withdrawEntrypointState.RequestId,
                                  withdrawEntrypointState.RequesterDeviceId,
                                  TransactionType.Withdraw,
                                  withdrawEntrypointState.CreatedAt,
                                  withdrawEntrypointState.UpdatedAt
                              )
                              {
                                  WithdrawEntrypoint = withdrawEntrypointState,
                                  QrDeposit = qrDepositState,
                                  OddDeposit = oddDepositState,
                                  OddWithdraw = oddWithdrawState,
                                  AtsWithdraw = atsWithdrawState,
                                  UpBack = upBackState,
                                  GlobalTransfer = globalTransferState,
                                  Recovery = recoveryState,
                                  GlobalManualAllocate = globalManualAllocate
                              }
                              : null).SingleAsync();
    }

    public async Task<Transaction?> GetByNo(string transactionNo, Product? product, string? userId)
    {
        var entryPoint = await (from depositEntrypointState in _walletDbContext.Set<DepositEntrypointState>()
                .Where(r => r.TransactionNo == transactionNo
                            && (product == null || r.Product == product)
                            && (userId == null || r.UserId == userId)).DefaultIfEmpty()
                                from withdrawEntrypointState in _walletDbContext.Set<WithdrawEntrypointState>()
                                    .Where(r => r.TransactionNo == transactionNo
                                                && (product == null || r.Product == product)
                                                && (userId == null || r.UserId == userId)).DefaultIfEmpty()
                                from oddDepositState in _walletDbContext.Set<OddDepositState>()
                                    .Where(o =>
                                        (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                                        (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                                    .DefaultIfEmpty()
                                from atsDepositState in _walletDbContext.Set<AtsDepositState>()
                                    .Where(o =>
                                        (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                                        (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                                    .DefaultIfEmpty()
                                from qrDepositState in _walletDbContext.Set<QrDepositState>()
                                    .Where(o =>
                                        (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                                        (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                                    .DefaultIfEmpty()
                                from oddWithdrawState in _walletDbContext.Set<OddWithdrawState>()
                                    .Where(o =>
                                        (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                                        (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                                    .DefaultIfEmpty()
                                from atsWithdrawState in _walletDbContext.Set<AtsWithdrawState>()
                                    .Where(o =>
                                        (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                                        (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                                    .DefaultIfEmpty()
                                from upBackState in _walletDbContext.Set<UpBackState>()
                                    .Where(o =>
                                        (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                                        (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                                    .DefaultIfEmpty()
                                from globalTransferState in _walletDbContext.Set<GlobalTransferState>()
                                    .Where(o =>
                                        (depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId) ||
                                        (withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId))
                                    .DefaultIfEmpty()
                                from recoveryState in _walletDbContext.Set<RecoveryState>()
                                    .Where(o => withdrawEntrypointState != null && o.CorrelationId == withdrawEntrypointState.CorrelationId)
                                    .DefaultIfEmpty()
                                from refundInfo in _walletDbContext.Set<RefundInfo>()
                                    .Where(r => depositEntrypointState != null
                                                && r.TicketId == depositEntrypointState.CorrelationId
                                                && r.CurrentState != RefundStatus.RefundFailed.ToString())
                                    .OrderByDescending(r => r.CreatedAt)
                                    .Take(1)
                                    .DefaultIfEmpty()
                                from globalManualAllocate in _walletDbContext.Set<GlobalManualAllocationState>()
                                    .Where(o =>
                                        (depositEntrypointState != null &&
                                         o.CorrelationId == depositEntrypointState.GlobalManualAllocateId) ||
                                        (withdrawEntrypointState != null &&
                                         o.CorrelationId == withdrawEntrypointState.GlobalManualAllocateId))
                                    .DefaultIfEmpty()
                                select depositEntrypointState != null
                                    ? new Transaction(
                                        depositEntrypointState.CorrelationId,
                                        depositEntrypointState.CurrentState,
                                        depositEntrypointState.TransactionNo,
                                        depositEntrypointState.UserId,
                                        depositEntrypointState.AccountCode,
                                        depositEntrypointState.CustomerCode,
                                        depositEntrypointState.Channel,
                                        depositEntrypointState.Product,
                                        depositEntrypointState.Purpose,
                                        depositEntrypointState.RequestedAmount,
                                        depositEntrypointState.NetAmount,
                                        depositEntrypointState.CustomerName,
                                        depositEntrypointState.BankAccountName,
                                        depositEntrypointState.BankAccountNo,
                                        depositEntrypointState.BankName,
                                        depositEntrypointState.BankCode,
                                        depositEntrypointState.ResponseAddress,
                                        depositEntrypointState.RequestId,
                                        depositEntrypointState.RequesterDeviceId,
                                        TransactionType.Deposit,
                                        depositEntrypointState.CreatedAt,
                                        depositEntrypointState.UpdatedAt
                                    )
                                    {
                                        DepositEntrypoint = depositEntrypointState,
                                        QrDeposit = qrDepositState,
                                        OddDeposit = oddDepositState,
                                        OddWithdraw = oddWithdrawState,
                                        AtsDeposit = atsDepositState,
                                        AtsWithdraw = atsWithdrawState,
                                        UpBack = upBackState,
                                        GlobalTransfer = globalTransferState,
                                        RefundInfo = refundInfo,
                                        GlobalManualAllocate = globalManualAllocate
                                    }
                                    : withdrawEntrypointState != null
                                        ? new Transaction(
                                            withdrawEntrypointState.CorrelationId,
                                            withdrawEntrypointState.CurrentState,
                                            withdrawEntrypointState.TransactionNo,
                                            withdrawEntrypointState.UserId,
                                            withdrawEntrypointState.AccountCode,
                                            withdrawEntrypointState.CustomerCode,
                                            withdrawEntrypointState.Channel,
                                            withdrawEntrypointState.Product,
                                            withdrawEntrypointState.Purpose,
                                            withdrawEntrypointState.RequestedAmount,
                                            withdrawEntrypointState.NetAmount,
                                            withdrawEntrypointState.CustomerName,
                                            withdrawEntrypointState.BankAccountName,
                                            withdrawEntrypointState.BankAccountNo,
                                            withdrawEntrypointState.BankName,
                                            withdrawEntrypointState.BankCode,
                                            withdrawEntrypointState.ResponseAddress,
                                            withdrawEntrypointState.RequestId,
                                            withdrawEntrypointState.RequesterDeviceId,
                                            TransactionType.Withdraw,
                                            withdrawEntrypointState.CreatedAt,
                                            withdrawEntrypointState.UpdatedAt
                                        )
                                        {
                                            WithdrawEntrypoint = withdrawEntrypointState,
                                            QrDeposit = qrDepositState,
                                            OddDeposit = oddDepositState,
                                            OddWithdraw = oddWithdrawState,
                                            AtsWithdraw = atsWithdrawState,
                                            UpBack = upBackState,
                                            GlobalTransfer = globalTransferState,
                                            Recovery = recoveryState,
                                            GlobalManualAllocate = globalManualAllocate
                                        }
                                        : null).SingleAsync();

        if (entryPoint != null)
        {
            return entryPoint;
        }

        // fallback in case of the transaction is not found in the deposit entrypoint
        // this is for the SetTrade ePay transactions
        // todo: remove once migrate the SetTrade ePay
        return await (from cashDeposit in _walletDbContext.Set<CashDepositState>()
                .Where(c => c.Channel == Channel.SetTrade && c.TransactionNo == transactionNo)
                .DefaultIfEmpty()
                      select cashDeposit != null ? new Transaction(
                          cashDeposit.CorrelationId,
                          cashDeposit.CurrentState,
                          cashDeposit.TransactionNo,
                          cashDeposit.UserId,
                          cashDeposit.AccountCode,
                          cashDeposit.CustomerCode,
                          cashDeposit.Channel,
                          cashDeposit.Product,
                          cashDeposit.Purpose,
                          cashDeposit.RequestedAmount,
                          cashDeposit.RequestedAmount,
                          null,
                          null,
                          null,
                          cashDeposit.BankName,
                          null,
                          string.Empty,
                          Guid.Empty,
                          Guid.Empty,
                          TransactionType.Deposit,
                          cashDeposit.CreatedAt,
                          cashDeposit.UpdatedAt
                      ) : null).SingleAsync();
    }

    public async Task<List<Transaction>> GetSetTradeEPayTransactions(int pageNum, int pageSize, IQueryFilter<CashDepositState>? filter, string? orderBy,
        string? orderDir)
    {
        var query = GetSetTradeEPayQuery(orderBy, orderDir, filter);
        return await (from transaction in query
                      select transaction)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<Transaction>> GetWithdrawTransactions(
        int pageNum,
        int pageSize,
        IQueryFilter<Transaction>? transactionFilters,
        string? orderBy,
        string? orderDir)
    {
        var query = GetWithdrawTransactionQuery(orderBy, orderDir);

        var transactions = await (from transaction in query
                    .WhereByFilters(transactionFilters)
                                  select transaction)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return transactions;
    }

    public async Task<List<Transaction>> GetDepositTransactions(
        int pageNum,
        int pageSize,
        IQueryFilter<Transaction>? transactionFilters,
        string? orderBy,
        string? orderDir)
    {
        var query = GetDepositTransactionQuery(orderBy, orderDir);

        var transactions = await (from transaction in query
                    .WhereByFilters(transactionFilters)
                                  select transaction)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return transactions;
    }

    public async Task<int> CountTransactions(TransactionType? type, IQueryFilter<Transaction>? depositFilters, IQueryFilter<Transaction>? withdrawFilters)
    {
        int count = 0;

        if (type is TransactionType.Deposit or null)
        {
            var query = GetDepositTransactionQuery(null, null);

            count += await (from transaction in query
                    .WhereByFilters(depositFilters)
                            select transaction).CountAsync();
        }

        if (type is TransactionType.Withdraw or null)
        {
            var query = GetWithdrawTransactionQuery(null, null);

            count += await (from transaction in query
                    .WhereByFilters(withdrawFilters)
                            select transaction).CountAsync();
        }

        return count;
    }

    public async Task<int> CountSetTradeEPayTransactions(IQueryFilter<CashDepositState>? setTradeEPayFilter)
    {
        var query = GetSetTradeEPayQuery(null, null, setTradeEPayFilter);
        return await (from transaction in query
                      select transaction).CountAsync();
    }

    public async Task<List<Transaction>> GetTransactionListByDateTime(Product product, DateTime createdAtFrom,
        DateTime createdAtTo)
    {
        var depositTransactions = await
            (from depositEntrypointState in _walletDbContext.Set<DepositEntrypointState>()
                    .Where(s => s.Product == product && s.CreatedAt >= createdAtFrom && s.CreatedAt <= createdAtTo)
                    .DefaultIfEmpty()
             from oddDepositState in _walletDbContext.Set<OddDepositState>()
                 .Where(s => depositEntrypointState != null &&
                             s.CorrelationId == depositEntrypointState.CorrelationId).DefaultIfEmpty()
             from atsDepositState in _walletDbContext.Set<AtsDepositState>()
                 .Where(s => depositEntrypointState != null &&
                             s.CorrelationId == depositEntrypointState.CorrelationId).DefaultIfEmpty()
             from qrDepositState in _walletDbContext.Set<QrDepositState>()
                 .Where(s => depositEntrypointState != null &&
                             s.CorrelationId == depositEntrypointState.CorrelationId).DefaultIfEmpty()
             from upBackState in _walletDbContext.Set<UpBackState>()
                 .Where(s => depositEntrypointState != null &&
                             s.CorrelationId == depositEntrypointState.CorrelationId).DefaultIfEmpty()
             from globalTransferState in _walletDbContext.Set<GlobalTransferState>()
                 .Where(s => depositEntrypointState != null &&
                             s.CorrelationId == depositEntrypointState.CorrelationId).DefaultIfEmpty()
             from refundInfoState in _walletDbContext.Set<RefundInfo>()
                 .Where(r => depositEntrypointState != null
                             && r.TicketId == depositEntrypointState.CorrelationId
                             && r.CurrentState != RefundStatus.RefundFailed.ToString())
                 .OrderByDescending(r => r.CreatedAt)
                 .Take(1)
                 .DefaultIfEmpty()
             from globalManualAllocate in _walletDbContext.Set<GlobalManualAllocationState>()
                 .Where(o =>
                     depositEntrypointState != null
                     && o.CorrelationId == depositEntrypointState.GlobalManualAllocateId)
                 .DefaultIfEmpty()
             select depositEntrypointState != null
                 ? new Transaction(
                     depositEntrypointState.CorrelationId,
                     depositEntrypointState.CurrentState ==
                     DepositEntrypointStates.GetName(() => DepositEntrypointStates.DepositProcessing)
                         ? depositEntrypointState.Channel == Channel.QR ? qrDepositState.CurrentState
                         : depositEntrypointState.Channel == Channel.ODD ? oddDepositState.CurrentState
                         : depositEntrypointState.Channel == Channel.ATS ? atsDepositState.CurrentState
                         : depositEntrypointState.CurrentState
                         : depositEntrypointState.Product == Product.GlobalEquities
                             ? depositEntrypointState.CurrentState ==
                               DepositEntrypointStates.GetName(
                                   () => DepositEntrypointStates.GlobalTransferProcessing)
                                 ? globalTransferState.CurrentState
                                 : depositEntrypointState.CurrentState
                             : depositEntrypointState.CurrentState ==
                               DepositEntrypointStates.GetName(() => DepositEntrypointStates.UpBackProcessing)
                                 ? upBackState.CurrentState
                                 : depositEntrypointState.CurrentState,
                     depositEntrypointState.TransactionNo,
                     depositEntrypointState.UserId,
                     depositEntrypointState.AccountCode,
                     depositEntrypointState.CustomerCode,
                     depositEntrypointState.Channel,
                     depositEntrypointState.Product,
                     depositEntrypointState.Purpose,
                     depositEntrypointState.RequestedAmount,
                     depositEntrypointState.NetAmount,
                     depositEntrypointState.CustomerName,
                     depositEntrypointState.BankAccountName,
                     depositEntrypointState.BankAccountNo,
                     depositEntrypointState.BankName,
                     depositEntrypointState.BankCode,
                     depositEntrypointState.ResponseAddress,
                     depositEntrypointState.RequestId,
                     depositEntrypointState.RequesterDeviceId,
                     TransactionType.Deposit,
                     depositEntrypointState.CreatedAt,
                     depositEntrypointState.UpdatedAt
                 )
                 {
                     QrDeposit = qrDepositState,
                     OddDeposit = oddDepositState,
                     AtsDeposit = atsDepositState,
                     UpBack = upBackState,
                     GlobalTransfer = globalTransferState,
                     RefundInfo = refundInfoState,
                     GlobalManualAllocate = globalManualAllocate
                 }
                 : null).ToListAsync();

        var withdrawTransactions = await
            (from withdrawEntrypointState in _walletDbContext.Set<WithdrawEntrypointState>()
                    .Where(s => s.Product == product && s.CreatedAt >= createdAtFrom && s.CreatedAt <= createdAtTo)
                    .DefaultIfEmpty()
             from oddWithdrawState in _walletDbContext.Set<OddWithdrawState>()
                 .Where(s => withdrawEntrypointState != null &&
                             s.CorrelationId == withdrawEntrypointState.CorrelationId).DefaultIfEmpty()
             from atsWithdrawState in _walletDbContext.Set<AtsWithdrawState>()
                 .Where(s => withdrawEntrypointState != null &&
                             s.CorrelationId == withdrawEntrypointState.CorrelationId).DefaultIfEmpty()
             from upBackState in _walletDbContext.Set<UpBackState>()
                 .Where(s => withdrawEntrypointState != null &&
                             s.CorrelationId == withdrawEntrypointState.CorrelationId).DefaultIfEmpty()
             from globalTransferState in _walletDbContext.Set<GlobalTransferState>()
                 .Where(s => withdrawEntrypointState != null &&
                             s.CorrelationId == withdrawEntrypointState.CorrelationId).DefaultIfEmpty()
             from recoveryState in _walletDbContext.Set<RecoveryState>()
                 .Where(s => withdrawEntrypointState != null &&
                             s.CorrelationId == withdrawEntrypointState.CorrelationId).DefaultIfEmpty()
             from globalManualAllocate in _walletDbContext.Set<GlobalManualAllocationState>()
                 .Where(o =>
                     withdrawEntrypointState != null
                     && o.CorrelationId == withdrawEntrypointState.GlobalManualAllocateId)
                 .DefaultIfEmpty()
             select withdrawEntrypointState != null
                 ? new Transaction(
                     withdrawEntrypointState.CorrelationId,
                     withdrawEntrypointState.CurrentState ==
                     WithdrawEntrypointStates.GetName(() => WithdrawEntrypointStates.WithdrawProcessing) ||
                     withdrawEntrypointState.CurrentState ==
                     WithdrawEntrypointStates.GetName(() => WithdrawEntrypointStates.WithdrawValidating)
                         ? withdrawEntrypointState.Channel == Channel.OnlineViaKKP
                             ? oddWithdrawState.CurrentState
                             : withdrawEntrypointState.Channel == Channel.ATS
                                 ? atsWithdrawState.CurrentState
                                 : withdrawEntrypointState.CurrentState
                         : withdrawEntrypointState.Product == Product.GlobalEquities
                             ? withdrawEntrypointState.CurrentState ==
                               WithdrawEntrypointStates.GetName(() =>
                                   WithdrawEntrypointStates.GlobalTransferProcessing)
                                 ? globalTransferState.CurrentState
                                 : withdrawEntrypointState.CurrentState
                             : withdrawEntrypointState.CurrentState ==
                               WithdrawEntrypointStates.GetName(() => WithdrawEntrypointStates.UpBackProcessing)
                                 ? upBackState.CurrentState
                                 : withdrawEntrypointState.CurrentState,
                     withdrawEntrypointState.TransactionNo,
                     withdrawEntrypointState.UserId,
                     withdrawEntrypointState.AccountCode,
                     withdrawEntrypointState.CustomerCode,
                     withdrawEntrypointState.Channel,
                     withdrawEntrypointState.Product,
                     withdrawEntrypointState.Purpose,
                     withdrawEntrypointState.RequestedAmount,
                     withdrawEntrypointState.NetAmount,
                     withdrawEntrypointState.CustomerName,
                     withdrawEntrypointState.BankAccountName,
                     withdrawEntrypointState.BankAccountNo,
                     withdrawEntrypointState.BankName,
                     withdrawEntrypointState.BankCode,
                     withdrawEntrypointState.ResponseAddress,
                     withdrawEntrypointState.RequestId,
                     withdrawEntrypointState.RequesterDeviceId,
                     TransactionType.Withdraw,
                     withdrawEntrypointState.CreatedAt,
                     withdrawEntrypointState.UpdatedAt
                 )
                 {
                     OddWithdraw = oddWithdrawState,
                     AtsWithdraw = atsWithdrawState,
                     UpBack = upBackState,
                     GlobalTransfer = globalTransferState,
                     GlobalManualAllocate = globalManualAllocate
                 }
                 : null).ToListAsync();

        if (depositTransactions.First() == null)
        {
            depositTransactions = new List<Transaction>();
        }

        if (withdrawTransactions.First() == null)
        {
            withdrawTransactions = new List<Transaction>();
        }

        return depositTransactions.Concat(withdrawTransactions)
            .Where(t => (t.Status != Status.Processing || !string.IsNullOrEmpty(t.FailedReason)) &&
                        t.GetState() != "TransferRequestFailed" && t.GetState() != "RevertTransferSuccess")
            .OrderBy(t => t.Status)
            .ThenByDescending(t => t.TransactionType)
            .ThenBy(t => t.TransactionType == TransactionType.Deposit
                ? t.Channel switch
                {
                    Channel.QR => t.QrDeposit!.PaymentReceivedDateTime,
                    Channel.ODD => t.OddDeposit!.PaymentReceivedDateTime,
                    Channel.ATS => t.AtsDeposit!.PaymentReceivedDateTime,
                    Channel.SetTrade
                        or Channel.OnlineViaKKP
                        or Channel.EForm
                        or Channel.TransferApp
                        or Channel.Unknown => throw new NotImplementedException(),
                    _ => throw new ArgumentOutOfRangeException()
                }
                : t.Channel switch
                {
                    Channel.OnlineViaKKP => t.OddWithdraw!.PaymentDisbursedDateTime,
                    Channel.ATS => t.AtsWithdraw!.PaymentDisbursedDateTime,
                    Channel.SetTrade
                        or Channel.QR
                        or Channel.EForm
                        or Channel.TransferApp
                        or Channel.Unknown => throw new NotImplementedException(),
                    _ => throw new ArgumentOutOfRangeException()
                })
            .ToList();
    }

    public async Task<List<Transaction>> GetActiveQrTransaction(string userId, Product product)
    {
        return await _walletDbContext.Set<DepositEntrypointState>()
            .Join(
                _walletDbContext.Set<QrDepositState>(),
                state => state.CorrelationId,
                qrState => qrState.CorrelationId,
                (entryPoint, qrState) => new
                {
                    DepositEntryPoint = entryPoint,
                    QrState = qrState
                })
            .Where(t =>
                t.DepositEntryPoint.UserId == userId
                && t.DepositEntryPoint.Product == product
                && t.QrState.DepositQrGenerateDateTime != null
                && t.QrState.DepositQrGenerateDateTime!.Value.AddMinutes(t.QrState.QrCodeExpiredTimeInMinute)
                    .CompareTo(DateTime.Now) >= 0
                && t.QrState.CurrentState == "WaitingForPayment")
            .Select(t => new Transaction(
                t.DepositEntryPoint.CorrelationId,
                t.DepositEntryPoint.CurrentState,
                t.DepositEntryPoint.TransactionNo,
                t.DepositEntryPoint.UserId,
                t.DepositEntryPoint.AccountCode,
                t.DepositEntryPoint.CustomerCode,
                t.DepositEntryPoint.Channel,
                t.DepositEntryPoint.Product,
                t.DepositEntryPoint.Purpose,
                t.DepositEntryPoint.RequestedAmount,
                t.DepositEntryPoint.NetAmount,
                t.DepositEntryPoint.CustomerName,
                t.DepositEntryPoint.BankAccountName,
                t.DepositEntryPoint.BankAccountNo,
                t.DepositEntryPoint.BankName,
                t.DepositEntryPoint.BankCode,
                t.DepositEntryPoint.ResponseAddress,
                t.DepositEntryPoint.RequestId,
                t.DepositEntryPoint.RequesterDeviceId,
                TransactionType.Deposit,
                t.DepositEntryPoint.CreatedAt,
                t.DepositEntryPoint.UpdatedAt
            )
            {
                DepositEntrypoint = t.DepositEntryPoint,
                QrDeposit = t.QrState
            })
            .ToListAsync();
    }

    public async Task<Guid?> GetIdByNo(string transactionNo)
    {
        var depositEntrypointStateQuery = _walletDbContext.Set<DepositEntrypointState>()
            .Where(d => d.TransactionNo == transactionNo)
            .Select(d => d.CorrelationId);

        var withdrawEntryPointStateQuery = _walletDbContext.Set<WithdrawEntrypointState>()
            .Where(d => d.TransactionNo == transactionNo)
            .Select(d => d.CorrelationId);

        var result = await depositEntrypointStateQuery
            .Union(withdrawEntryPointStateQuery)
            .FirstOrDefaultAsync();

        return result;
    }

    public Task<int> CountTransactionNoByDate(DateOnly date, TimeOnly cutOffTime, bool isThTime, string productInitial, Channel channel,
        TransactionType? transactionType = null)
    {
        throw new NotImplementedException();
    }

    public Task UpdateTransactionNo(Guid correlationId, string transactionNo)
    {
        throw new NotImplementedException();
    }

    private IQueryable<Transaction> GetDepositTransactionQuery(string? orderBy, string? orderDir)
    {
        return (from depositEntrypointState in _walletDbContext.Set<DepositEntrypointState>()
                .OrderByProperty(orderBy ?? "CreatedAt", orderDir ?? "desc")
                from qrDepositState in _walletDbContext.Set<QrDepositState>()
                    .Where(q => depositEntrypointState != null && q.CorrelationId == depositEntrypointState.CorrelationId)
                    .DefaultIfEmpty()
                from oddDepositState in _walletDbContext.Set<OddDepositState>()
                    .Where(o => depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId)
                    .DefaultIfEmpty()
                from atsDepositState in _walletDbContext.Set<AtsDepositState>()
                    .Where(o => depositEntrypointState != null && o.CorrelationId == depositEntrypointState.CorrelationId)
                    .DefaultIfEmpty()
                from upBackState in _walletDbContext.Set<UpBackState>()
                    .Where(u => depositEntrypointState != null && u.CorrelationId == depositEntrypointState.CorrelationId)
                    .DefaultIfEmpty()
                from globalTransferState in _walletDbContext.Set<GlobalTransferState>()
                    .Where(g => depositEntrypointState != null && g.CorrelationId == depositEntrypointState.CorrelationId)
                    .DefaultIfEmpty()
                from refundInfo in _walletDbContext.Set<RefundInfo>()
                    .Where(r => depositEntrypointState != null
                                && r.TicketId == depositEntrypointState.CorrelationId
                                && r.CurrentState != RefundStatus.RefundFailed.ToString())
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(1)
                    .DefaultIfEmpty()
                from globalManualAllocate in _walletDbContext.Set<GlobalManualAllocationState>()
                    .Where(o =>
                        depositEntrypointState != null
                        && o.CorrelationId == depositEntrypointState.GlobalManualAllocateId)
                    .DefaultIfEmpty()
                select new Transaction(
                    depositEntrypointState.CorrelationId,
                    depositEntrypointState.CurrentState,
                    depositEntrypointState.TransactionNo,
                    depositEntrypointState.UserId,
                    depositEntrypointState.AccountCode,
                    depositEntrypointState.CustomerCode,
                    depositEntrypointState.Channel,
                    depositEntrypointState.Product,
                    depositEntrypointState.Purpose,
                    depositEntrypointState.RequestedAmount,
                    depositEntrypointState.NetAmount,
                    depositEntrypointState.CustomerName,
                    depositEntrypointState.BankAccountName,
                    depositEntrypointState.BankAccountNo,
                    depositEntrypointState.BankName,
                    depositEntrypointState.BankCode,
                    depositEntrypointState.ResponseAddress,
                    depositEntrypointState.RequestId,
                    depositEntrypointState.RequesterDeviceId,
                    TransactionType.Deposit,
                    depositEntrypointState.CreatedAt,
                    depositEntrypointState.UpdatedAt)
                {
                    DepositEntrypoint = depositEntrypointState,
                    QrDeposit = qrDepositState,
                    AtsDeposit = atsDepositState,
                    OddDeposit = oddDepositState,
                    UpBack = upBackState,
                    GlobalTransfer = globalTransferState,
                    RefundInfo = refundInfo,
                    GlobalManualAllocate = globalManualAllocate
                });
    }

    private IQueryable<Transaction> GetWithdrawTransactionQuery(string? orderBy, string? orderDir)
    {
        return (from withdrawEntrypointState in _walletDbContext.Set<WithdrawEntrypointState>()
                .OrderByProperty(orderBy ?? "CreatedAt", orderDir ?? "desc")
                from oddWithdrawState in _walletDbContext.Set<OddWithdrawState>()
                    .Where(o => withdrawEntrypointState != null &&
                                o.CorrelationId == withdrawEntrypointState.CorrelationId)
                    .DefaultIfEmpty()
                from atsWithdrawState in _walletDbContext.Set<AtsWithdrawState>()
                    .Where(o => withdrawEntrypointState != null &&
                                o.CorrelationId == withdrawEntrypointState.CorrelationId)
                    .DefaultIfEmpty()
                from upBackState in _walletDbContext.Set<UpBackState>()
                    .Where(o => withdrawEntrypointState != null &&
                                o.CorrelationId == withdrawEntrypointState.CorrelationId)
                    .DefaultIfEmpty()
                from globalTransferState in _walletDbContext.Set<GlobalTransferState>()
                    .Where(o => withdrawEntrypointState != null &&
                                o.CorrelationId == withdrawEntrypointState.CorrelationId)
                    .DefaultIfEmpty()
                from recoveryState in _walletDbContext.Set<RecoveryState>()
                    .Where(o => withdrawEntrypointState != null &&
                                o.CorrelationId == withdrawEntrypointState.CorrelationId)
                    .DefaultIfEmpty()
                from globalManualAllocate in _walletDbContext.Set<GlobalManualAllocationState>()
                    .Where(o =>
                        withdrawEntrypointState != null
                        && o.CorrelationId == withdrawEntrypointState.GlobalManualAllocateId)
                    .DefaultIfEmpty()
                select new Transaction(
                    withdrawEntrypointState.CorrelationId,
                    withdrawEntrypointState.CurrentState,
                    withdrawEntrypointState.TransactionNo,
                    withdrawEntrypointState.UserId,
                    withdrawEntrypointState.AccountCode,
                    withdrawEntrypointState.CustomerCode,
                    withdrawEntrypointState.Channel,
                    withdrawEntrypointState.Product,
                    withdrawEntrypointState.Purpose,
                    withdrawEntrypointState.RequestedAmount,
                    withdrawEntrypointState.NetAmount,
                    withdrawEntrypointState.CustomerName,
                    withdrawEntrypointState.BankAccountName,
                    withdrawEntrypointState.BankAccountNo,
                    withdrawEntrypointState.BankName,
                    withdrawEntrypointState.BankCode,
                    withdrawEntrypointState.ResponseAddress,
                    withdrawEntrypointState.RequestId,
                    withdrawEntrypointState.RequesterDeviceId,
                    TransactionType.Withdraw,
                    withdrawEntrypointState.CreatedAt,
                    withdrawEntrypointState.UpdatedAt)
                {
                    WithdrawEntrypoint = withdrawEntrypointState,
                    OddWithdraw = oddWithdrawState,
                    AtsWithdraw = atsWithdrawState,
                    UpBack = upBackState,
                    GlobalTransfer = globalTransferState,
                    Recovery = recoveryState,
                    GlobalManualAllocate = globalManualAllocate
                });
    }

    private IQueryable<Transaction> GetSetTradeEPayQuery(string? orderBy, string? orderDir, IQueryFilter<CashDepositState>? filter)
    {
        return from cashDeposit in _walletDbContext.Set<CashDepositState>()
                .OrderByProperty(orderBy ?? "CreatedAt", orderDir ?? "desc")
                .WhereByFilters(filter)
               select new Transaction(
                   cashDeposit.CorrelationId,
                   cashDeposit.CurrentState,
                   cashDeposit.TransactionNo,
                   cashDeposit.UserId,
                   cashDeposit.AccountCode,
                   cashDeposit.CustomerCode,
                   cashDeposit.Channel,
                   cashDeposit.Product,
                   cashDeposit.Purpose,
                   cashDeposit.RequestedAmount,
                   cashDeposit.RequestedAmount,
                   null,
                   null,
                   null,
                   cashDeposit.BankName,
                   null,
                   string.Empty,
                   Guid.Empty,
                   Guid.Empty,
                   TransactionType.Deposit,
                   cashDeposit.CreatedAt,
                   cashDeposit.UpdatedAt
               );
    }
}
