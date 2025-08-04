using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.AtsWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
namespace Pi.WalletService.Domain;

public interface ISagaUnitOfWork
{
    IQrDepositRepository QrDepositRepository { get; }
    IOddDepositRepository OddDepositRepository { get; }
    IAtsDepositRepository AtsDepositRepository { get; }
    IOddWithdrawRepository OddWithdrawRepository { get; }
    IAtsWithdrawRepository AtsWithdrawRepository { get; }
    IUpBackRepository UpBackRepository { get; }
    IGlobalTransferRepository GlobalTransferRepository { get; }
    void Commit();
    void Rollback();
    Task CommitAsync();
    Task RollbackAsync();
}
