using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.AtsWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
namespace Pi.WalletService.Infrastructure.UnitOfWork;

public class SagaUnitOfWork : ISagaUnitOfWork
{
    private readonly WalletDbContext _dbContext;

    public SagaUnitOfWork(
        WalletDbContext dbContext,
        IQrDepositRepository qrDepositRepository,
        IOddDepositRepository oddDepositRepository,
        IOddWithdrawRepository oddWithdrawRepository,
        IAtsDepositRepository atsDepositRepository,
        IAtsWithdrawRepository atsWithdrawRepository,
        IUpBackRepository upBackRepository,
        IGlobalTransferRepository globalTransferRepository)
    {
        _dbContext = dbContext;
        QrDepositRepository = qrDepositRepository;
        OddDepositRepository = oddDepositRepository;
        OddWithdrawRepository = oddWithdrawRepository;
        AtsDepositRepository = atsDepositRepository;
        AtsWithdrawRepository = atsWithdrawRepository;
        UpBackRepository = upBackRepository;
        GlobalTransferRepository = globalTransferRepository;
    }

    public IQrDepositRepository QrDepositRepository { get; }
    public IOddDepositRepository OddDepositRepository { get; }
    public IOddWithdrawRepository OddWithdrawRepository { get; }
    public IAtsDepositRepository AtsDepositRepository { get; }
    public IAtsWithdrawRepository AtsWithdrawRepository { get; }
    public IUpBackRepository UpBackRepository { get; }
    public IGlobalTransferRepository GlobalTransferRepository { get; }

    public void Commit()
        => _dbContext.SaveChanges();

    public async Task CommitAsync()
        => await _dbContext.SaveChangesAsync();

    public void Rollback()
        => _dbContext.Dispose();

    public async Task RollbackAsync()
        => await _dbContext.DisposeAsync();
}
