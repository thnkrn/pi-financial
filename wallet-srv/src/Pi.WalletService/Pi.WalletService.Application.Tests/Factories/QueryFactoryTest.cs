using Pi.WalletService.Application.Factories;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.IntegrationEvents.Models;
using CashDepositState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashDepositState;
using CashWithdrawState = Pi.WalletService.Domain.AggregatesModel.CashAggregate.CashWithdrawState;
using DepositState = Pi.WalletService.Domain.AggregatesModel.DepositAggregate.DepositState;
using GlobalWalletTransferState = Pi.WalletService.IntegrationEvents.Models.GlobalWalletTransferState;
using WithdrawState = Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate.WithdrawState;

namespace Pi.WalletService.Application.Tests.Factories;

public class QueryFactoryTest
{
    [Fact]
    public void Should_Return_DepositTransaction_When_NewDepositTransaction_With_DepositState()
    {
        // Arrange
        var deposit = new DepositState();

        // Act
        var actual = QueryFactory.NewDepositTransaction(deposit);

        // Assert
        Assert.IsType<DepositTransaction>(actual);
    }

    [Fact]
    public void Should_Return_DepositTransaction_With_PendingStatus_When_NewDepositTransaction_With_DepositStateCompleted()
    {
        // Arrange
        var deposit = new DepositState()
        {
            CurrentState = IntegrationEvents.Models.DepositState.GetName(() => IntegrationEvents.Models.DepositState.DepositCompleted)
        };

        // Act
        var actual = QueryFactory.NewDepositTransaction(deposit);

        // Assert
        Assert.Equal(TransactionStatus.Pending, actual.Status);
    }

    [Fact]
    public void Should_Return_DepositTransaction_When_NewDepositTransaction_With_CashDeposit()
    {
        // Arrange
        var deposit = new CashDepositState();

        // Act
        var actual = QueryFactory.NewDepositTransaction(deposit);

        // Assert
        Assert.IsType<DepositTransaction>(actual);
    }

    [Fact]
    public void Should_Return_DepositTransaction_With_ExpectedData_When_NewDepositTransaction_With_CashDeposit()
    {
        // Arrange
        var cashDepositState = new CashDepositState() { CorrelationId = Guid.NewGuid(), RequestedAmount = 100 };

        // Act
        var actual = QueryFactory.NewDepositTransaction(cashDepositState);

        // Assert
        Assert.Equal(cashDepositState.RequestedAmount, actual.Amount);
        Assert.Equal(0, actual.QrCodeExpiredTimeInMinute);
        Assert.Null(actual.ReceivedAmount);
        Assert.Null(actual.PaymentReceivedAmount);
        Assert.Null(actual.CustomerName);
        Assert.Null(actual.BankAccountName);
        Assert.Null(actual.QrGenerateDateTime);
        Assert.Null(actual.QrExpiredTime);
        Assert.Null(actual.QrTransactionNo);
        Assert.Null(actual.QrValue);
        Assert.Null(actual.QrTransactionRef);
        Assert.Null(actual.BankFee);
        Assert.Null(actual.BankCode);
        Assert.Null(actual.BankAccountNo);
        Assert.Null(actual.EffectiveDateTime);
    }

    [Fact]
    public void Should_Return_DepositTransaction_With_ExpectedEffectiveDate_When_NewDepositTransaction_With_CashDeposit()
    {
        // Arrange
        var cashDepositState = new CashDepositState()
        {
            CurrentState = IntegrationEvents.Models.CashDepositState.GetName(() => IntegrationEvents.Models.CashDepositState.CashDepositCompleted),
            UpdatedAt = DateTime.Now,
        };

        // Act
        var actual = QueryFactory.NewDepositTransaction(cashDepositState);

        // Assert
        Assert.Equal(cashDepositState.UpdatedAt, actual.EffectiveDateTime);
    }

    [Fact]
    public void Should_Return_DepositTransaction_And_EffectiveDateTime_Equal_PaymentReceivedDateTime_When_NewDepositTransaction_With_DepositState()
    {
        // Arrange
        var deposit = new DepositState()
        {
            PaymentReceivedDateTime = DateTime.Now
        };

        // Act
        var actual = QueryFactory.NewDepositTransaction(deposit);

        // Assert
        Assert.Equal(deposit.PaymentReceivedDateTime, actual.EffectiveDateTime);
    }

    [Fact]
    public void Should_Return_DepositTransaction_When_NewDepositTransaction_With_GlobalDepositTransaction()
    {
        // Arrange
        var deposit = new DepositState();
        var globalDeposit = new GlobalDepositTransaction() { DepositState = deposit };

        // Act
        var actual = QueryFactory.NewDepositTransaction(globalDeposit);

        // Assert
        Assert.IsType<DepositTransaction>(actual);
    }

    [Fact]
    public void Should_Return_DepositTransaction_With_SuccessStatus_When_NewDepositTransaction_With_GlobalDepositTransaction()
    {
        // Arrange
        var deposit = new DepositState();
        var globalDeposit = new GlobalDepositTransaction()
        {
            DepositState = deposit,
            GlobalWalletTransferState = new Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState()
            {
                CurrentState = "Final",
            }
        };

        // Act
        var actual = QueryFactory.NewDepositTransaction(globalDeposit);

        // Assert
        Assert.Equal(TransactionStatus.Success, actual.Status);
    }

    [Fact]
    public void Should_Return_DepositTransaction_With_FailedStatus_When_NewDepositTransaction_With_GlobalDepositTransaction_And_DepositFailed()
    {
        // Arrange
        var deposit = new DepositState()
        {
            CurrentState = IntegrationEvents.Models.DepositState.GetName(() => IntegrationEvents.Models.DepositState.DepositFailed),
        };
        var globalDeposit = new GlobalDepositTransaction()
        {
            DepositState = deposit,
            GlobalWalletTransferState = new Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState()
            {
                CurrentState = IntegrationEvents.Models.GlobalWalletTransferState.GetName(() => IntegrationEvents.Models.GlobalWalletTransferState.DepositProcessing),
            }
        };

        // Act
        var actual = QueryFactory.NewDepositTransaction(globalDeposit);

        // Assert
        Assert.Equal(TransactionStatus.Fail, actual.Status);
    }

    [Fact]
    public void Should_Return_DepositTransaction_With_ExpectedData_When_NewDepositTransaction_With_GlobalDepositTransaction_And_DepositCompleted()
    {
        // Arrange
        var deposit = new DepositState()
        {
            CurrentState = IntegrationEvents.Models.DepositState.GetName(() => IntegrationEvents.Models.DepositState.DepositCompleted),
            FailedReason = "DepositStateFailed"
        };
        var globalDeposit = new GlobalDepositTransaction()
        {
            DepositState = deposit,
            GlobalWalletTransferState = new Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState()
            {
                CurrentState = GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.FxTransferring),
                FailedReason = "GlobalWalletTransferStateFailed"
            }
        };

        // Act
        var actual = QueryFactory.NewDepositTransaction(globalDeposit);

        // Assert
        Assert.Equal(TransactionStatus.Pending, actual.Status);
        Assert.Equal(globalDeposit.GlobalWalletTransferState.CurrentState, actual.CurrentState);
        Assert.Equal(globalDeposit.GlobalWalletTransferState.FailedReason, actual.FailedReason);
    }

    [Fact]
    public void Should_Return_DepositTransaction_With_ExpectedData_When_NewDepositTransaction_With_DepositRefundSuccess()
    {
        // Arrange
        var deposit = new DepositState()
        {
            CurrentState = IntegrationEvents.Models.DepositState.GetName(() => IntegrationEvents.Models.DepositState.DepositRefundSucceed),
        };
        var globalDeposit = new GlobalDepositTransaction()
        {
            DepositState = deposit,
            GlobalWalletTransferState = new Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState()
            {
                CurrentState = GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.DepositFailed),
            }
        };

        // Act
        var actual = QueryFactory.NewDepositTransaction(globalDeposit);

        // Assert
        Assert.Equal(TransactionStatus.Fail, actual.Status);
        Assert.Equal(deposit.CurrentState, actual.CurrentState);
    }

    [Fact]
    public void Should_Return_DepositTransaction_And_EffectiveDateTime_Equal_TransferCompleteTime_When_NewDepositTransaction_With_GlobalDepositTransaction()
    {
        // Arrange
        var deposit = new DepositState()
        {
            PaymentReceivedDateTime = DateTime.Today
        };
        var globalDeposit = new GlobalDepositTransaction()
        {
            DepositState = deposit,
            GlobalWalletTransferState = new Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState()
            {
                TransferCompleteTime = DateTime.Now
            }
        };

        // Act
        var actual = QueryFactory.NewDepositTransaction(globalDeposit);

        // Assert
        Assert.Equal(globalDeposit.GlobalWalletTransferState.TransferCompleteTime, actual.EffectiveDateTime);
    }

    [Fact]
    public void Should_Return_DepositTransaction_And_ExpectedEffectiveDateTime_When_NewDepositTransaction_With_ThaiDepositTransaction()
    {
        // Arrange
        var deposit = new DepositState()
        {
            PaymentReceivedDateTime = DateTime.Today,
            CurrentState = IntegrationEvents.Models.DepositState.GetName(() => IntegrationEvents.Models.DepositState.DepositCompleted),
        };
        var depositTransaction = new ThaiDepositTransaction()
        {
            DepositState = deposit,
            CashDepositState = new CashDepositState()
            {
                CurrentState = IntegrationEvents.Models.CashDepositState.GetName(() => IntegrationEvents.Models.CashDepositState.CashDepositCompleted),
                UpdatedAt = DateTime.Now,
            }
        };

        // Act
        var actual = QueryFactory.NewDepositTransaction(depositTransaction);

        // Assert
        Assert.Equal(depositTransaction.CashDepositState.UpdatedAt, actual.EffectiveDateTime);
    }

    [Fact]
    public void Should_Return_DepositTransaction_And_ExpectedData_When_NewDepositTransaction_With_ThaiDepositTransaction()
    {
        // Arrange
        var deposit = new DepositState()
        {
            CurrentState = IntegrationEvents.Models.DepositState.GetName(() => IntegrationEvents.Models.DepositState.DepositCompleted),
            FailedReason = "DepositStateFailed"
        };
        var depositTransaction = new ThaiDepositTransaction()
        {
            DepositState = deposit,
            CashDepositState = new CashDepositState()
            {
                CurrentState = IntegrationEvents.Models.CashDepositState.GetName(() => IntegrationEvents.Models.CashDepositState.CashDepositTradingPlatformUpdating),
                FailedReason = "CashDepositStateFailed",
            }
        };

        // Act
        var actual = QueryFactory.NewDepositTransaction(depositTransaction);

        // Assert
        Assert.Equal(depositTransaction.CashDepositState.CurrentState, actual.CurrentState);
        Assert.Equal(depositTransaction.CashDepositState.FailedReason, actual.FailedReason);
    }

    [Fact]
    public void Should_Return_WithdrawTransaction_And_ExpectedData_When_NewWithdrawTransaction_With_GlobalWithdrawTransaction()
    {
        // Arrange
        var withdraw = new WithdrawState()
        {
            CurrentState = IntegrationEvents.Models.WithdrawState.GetName(() => IntegrationEvents.Models.WithdrawState.WithdrawalProcessing),
            FailedReason = "WithdrawStateFailed"
        };
        var withdrawTransaction = new GlobalWithdrawTransaction()
        {
            WithdrawState = withdraw,
            GlobalWalletTransferState = new Domain.AggregatesModel.GlobalWalletAggregate.GlobalWalletTransferState()
            {
                CurrentState = GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.TransferRequestFailed),
                FailedReason = "GlobalWalletTransferStateFailed",
            }
        };

        // Act
        var actual = QueryFactory.NewWithdrawTransaction(withdrawTransaction);

        // Assert
        Assert.Equal(withdrawTransaction.GlobalWalletTransferState.CurrentState, actual.CurrentState);
        Assert.Equal(withdrawTransaction.GlobalWalletTransferState.FailedReason, actual.FailedReason);
    }

    [Fact]
    public void Should_Return_WithdrawTransaction_And_ExpectedData_When_NewWithdrawTransaction_With_ThaiWithdrawTransaction()
    {
        // Arrange
        var withdraw = new WithdrawState()
        {
            CurrentState = IntegrationEvents.Models.WithdrawState.GetName(() => IntegrationEvents.Models.WithdrawState.WithdrawalProcessing),
            FailedReason = "WithdrawStateFailed"
        };
        var withdrawTransaction = new ThaiWithdrawTransaction()
        {
            WithdrawState = withdraw,
            CashWithdrawState = new CashWithdrawState()
            {
                CurrentState = IntegrationEvents.Models.CashWithdrawState.GetName(() => IntegrationEvents.Models.CashWithdrawState.TransferRequestFailed),
                FailedReason = "CashWithdrawStateFailed",
            }
        };

        // Act
        var actual = QueryFactory.NewWithdrawTransaction(withdrawTransaction);

        // Assert
        Assert.Equal(withdrawTransaction.CashWithdrawState.CurrentState, actual.CurrentState);
        Assert.Equal(withdrawTransaction.CashWithdrawState.FailedReason, actual.FailedReason);
    }
}
