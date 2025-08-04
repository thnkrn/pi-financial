using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.StateMachines.DepositEntrypoint;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.Domain.Events.ATS;
using Pi.WalletService.Domain.Events.DepositEntrypoint;
using Pi.WalletService.Domain.Events.ForeignExchange;
using Pi.WalletService.Domain.Events.GlobalTransfer;
using Pi.WalletService.Domain.Events.OddDeposit;
using Pi.WalletService.Domain.Events.QrDeposit;
using Pi.WalletService.Domain.Events.Refund;
using Pi.WalletService.Domain.Events.UpBack;
using Pi.WalletService.IntegrationEvents;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using Pi.WalletService.IntegrationEvents.AtsEvents;
using StateMachine = Pi.WalletService.Application.StateMachines.DepositEntrypoint.StateMachine;

namespace Pi.WalletService.Application.Tests.StateMachines.DepositEntrypoint;

public class StateMachineTests : ConsumerTest
{
    private readonly ISagaRepository<DepositEntrypointState> _sagaRepository;

    public StateMachineTests()
    {
        IntegrationEvents.Models.DepositEntrypointState.CleanUp();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<StateMachine, DepositEntrypointState>(typeof(StateDefinition));
            })
            .BuildServiceProvider(true);
        _sagaRepository = Provider.GetRequiredService<ISagaRepository<DepositEntrypointState>>();
    }

    //[Theory]
    //[InlineData(false)]
    //[InlineData(true)]
    //public async void Should_Handle_Transition_Deposit_State_Correctly_For_Non_GE(bool isHasTransactionNo)
    //{
    //    // Arrange
    //    var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
    //    var sagaId = NewId.NextGuid();
    //    const string transactionNo = "transaction_no";
    //    string userId = Guid.NewGuid().ToString();

    //    await Harness.Bus.Publish(
    //        new DepositEntrypointRequest(
    //            sagaId,
    //            userId,
    //            "77114312",
    //            "7711431",
    //            Product.Cash,
    //            Channel.ODD,
    //            Purpose.Unknown,
    //            2_000,
    //            "จอนนี่ ไทย",
    //            null,
    //            null,
    //            null,
    //            null,
    //            null,
    //            Guid.NewGuid(),
    //            Guid.NewGuid(),
    //            new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString()
    //        ));

    //    await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

    //    await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

    //    await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

    //    await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

    //    if (!isHasTransactionNo)
    //    {
    //        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

    //        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));
    //    }

    //    await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

    //    await Harness.Bus.Publish(
    //        new OddDepositRequest(
    //            sagaId
    //        )
    //    );

    //    await Harness.Bus.Publish(
    //        new DepositOddSuccessEvent(
    //            sagaId,
    //            userId,
    //            Product.Cash.ToString(),
    //            "BankCode",
    //            "BankName",
    //            "BankAccountNo",
    //            "จอนนี่ ไทย",
    //            2_000
    //        )
    //    );

    //    await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.UpBackProcessing!);

    //    await Harness.Bus.Publish(
    //        new UpBackSuccess(
    //            sagaId
    //        )
    //    );

    //    await Harness.Bus.Publish(
    //        new DepositSuccessEvent(
    //            sagaId,
    //            userId,
    //            transactionNo,
    //            DateTime.Now,
    //            Product.Cash.ToString(),
    //            2_000,
    //            "77114312",
    //            "7711431",
    //            "BankName",
    //            null,
    //            Channel.ODD
    //        )
    //    );

    //    await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Final!);
    //}

    //[Theory]
    //[InlineData(false)]
    //[InlineData(true)]
    //public async void Should_Handle_Transition_Deposit_State_Correctly_For_GE(bool isHasTransactionNo)
    //{
    //    // Arrange
    //    var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
    //    var sagaId = NewId.NextGuid();
    //    const string transactionNo = "transaction_no";
    //    string userId = Guid.NewGuid().ToString();

    //    await Harness.Bus.Publish(
    //        new DepositEntrypointRequest(
    //            sagaId,
    //            userId,
    //            "77114312",
    //            "7711431",
    //            Product.GlobalEquities,
    //            Channel.ODD,
    //            Purpose.Unknown,
    //            2_000,
    //            "จอนนี่ ไทย",
    //            null,
    //            null,
    //            null,
    //            null,
    //            null,
    //            Guid.NewGuid(),
    //            Guid.NewGuid(),
    //            new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString()
    //        ));

    //    await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

    //    await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

    //    await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

    //    await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

    //    if (!isHasTransactionNo)
    //    {
    //        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

    //        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));
    //    }

    //    await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

    //    await Harness.Bus.Publish(
    //        new OddDepositRequest(
    //            sagaId
    //        )
    //    );

    //    await Harness.Bus.Publish(
    //        new DepositOddSuccessEvent(
    //            sagaId,
    //            userId,
    //            Product.GlobalEquities.ToString(),
    //            "BankCode",
    //            "BankName",
    //            "BankAccountNo",
    //            "จอนนี่ ไทย",
    //            2_000
    //        )
    //    );

    //    await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.GlobalTransferProcessing!);

    //    await Harness.Bus.Publish(
    //        new GlobalTransferRequest(
    //            sagaId
    //        )
    //    );

    //    await Harness.Bus.Publish(
    //        new GlobalTransferSuccess(
    //            sagaId,
    //            TransactionType.Deposit
    //        )
    //    );

    //    await Harness.Bus.Publish(
    //        new GlobalDepositSuccessEvent(
    //            sagaId,
    //            userId,
    //            transactionNo
    //        )
    //    );

    //    await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Final!);
    //}

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_ODD_Deposit_Failed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.Cash,
                Channel.ODD,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new OddDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new DepositOddFailedEvent(
                sagaId,
                "Fail"
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositFailed!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_Upback_Failed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.Cash,
                Channel.ODD,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new OddDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new DepositOddSuccessEvent(
                sagaId,
                userId,
                Product.Cash.ToString(),
                2_000
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.UpBackProcessing!);

        await Harness.Bus.Publish(
            new UpBackFailed(
                sagaId,
                "Fail"
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositFailed!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_QR_Deposit_Failed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.Cash,
                Channel.QR,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        await Harness.Bus.Publish(
            new DepositQrFailedEvent(
                sagaId,
                "Fail",
                null,
                null,
                null,
                (decimal)10.00
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositFailed!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_QR_Deposit_Succeed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.Cash,
                Channel.QR,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        await Harness.Bus.Publish(
            new DepositQrSuccessEvent(
                sagaId
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.UpBackProcessing!);

        await Harness.Bus.Publish(
            new UpBackRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new UpBackSuccess(
                sagaId
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Final!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_Refund_Succeed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.GlobalEquities,
                Channel.QR,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        await Harness.Bus.Publish(
            new DepositRefundSucceed(
                sagaId,
                Guid.NewGuid()
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositFailed!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_Request_OTP_Failed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.Cash,
                Channel.ATS,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new AtsDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new DepositOtpValidationFailed(
                sagaId,
                "Fail"
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositFailed!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_OTP_Not_Received_Failed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.Cash,
                Channel.ATS,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new AtsDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new OtpValidationNotReceived(
                sagaId,
                "Fail"
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositPaymentNotReceived!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_Payment_Not_Received_Failed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.Cash,
                Channel.ATS,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new AtsDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new PaymentNotReceived(
                sagaId,
                "Fail"
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositPaymentNotReceived!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_Confirm_Received_At_Payment_Not_Received_State_Succeed()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.Cash,
                Channel.ATS,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new AtsDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new PaymentNotReceived(
                sagaId,
                "Fail"
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositPaymentNotReceived!);

        await Harness.Bus.Publish(
            new DepositQrSuccessEvent(
                sagaId
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.UpBackProcessing!);

        await Harness.Bus.Publish(
            new UpBackRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new UpBackSuccess(
                sagaId
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Final!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_Global_Transfer_Failed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.GlobalEquities,
                Channel.ATS,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new AtsDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new DepositAtsSuccessEvent(
                sagaId,
                2_000
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.GlobalTransferProcessing!);

        await Harness.Bus.Publish(
            new GlobalTransferRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new GlobalTransferFailed(
                sagaId,
                TransactionType.Deposit,
                "Failed",
                2_000
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositFailed!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_ATS_Deposit_Failed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.Cash,
                Channel.ATS,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new AtsDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new DepositAtsFailedEvent(
                sagaId,
                "Fail"
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositFailed!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_ATS_Deposit_GE_Succeed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.GlobalEquities,
                Channel.ATS,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new AtsDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new DepositAtsSuccessEvent(
                sagaId,
                2_000
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.GlobalTransferProcessing!);

        await Harness.Bus.Publish(
            new GlobalTransferRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new GlobalTransferSuccess(
                sagaId,
                2_000
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Final!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_ATS_Deposit_Non_GE_Succeed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.Cash,
                Channel.ATS,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new AtsDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new DepositAtsSuccessEvent(
                sagaId,
                2_000
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.UpBackProcessing!);

        await Harness.Bus.Publish(
            new UpBackRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new UpBackSuccess(
                sagaId
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Final!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_Failed_By_Operation()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.Cash,
                Channel.ATS,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new AtsDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new DepositAtsSuccessEvent(
                sagaId,
                2_000
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.UpBackProcessing!);

        await Harness.Bus.Publish(
            new UpdateTransactionStatusFailedEvent(
                sagaId
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositFailed!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_Success_By_Operation()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.Cash,
                Channel.ATS,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new AtsDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new DepositAtsSuccessEvent(
                sagaId,
                2_000
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.UpBackProcessing!);

        await Harness.Bus.Publish(
            new UpdateTransactionStatusSuccessEvent(
                sagaId
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Final!);
    }

    [Fact]
    public async void Should_Handle_Transition_Deposit_State_Correctly_When_Global_Manual_Allocation_Succeed()
    {
        // Arrange
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, DepositEntrypointState>();
        var sagaId = NewId.NextGuid();
        string userId = Guid.NewGuid().ToString();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new DepositEntrypointRequest(
                sagaId,
                userId,
                "77114312",
                "7711431",
                Product.GlobalEquities,
                Channel.ATS,
                Purpose.Unknown,
                2_000,
                null,
                "จอนนี่ ไทย",
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                0_55,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Uri("loopback://localhost/NutchanonsMacBookPro_ReSharperTestRunner_bus_moyoyyrmz5uq3k1abdpz8xm4nk").ToString(),
                DateTime.Now
            ));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Initiate!);

        await ResponseSagaRequest<InitiateDepositStateMachine, InitiateDepositStateMachineSuccess>(new InitiateDepositStateMachineSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.TransactionNoGenerating!);

        await ResponseSagaRequest<GenerateTransactionNo, TransactionNoGenerated>(new TransactionNoGenerated(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.DepositProcessing!);

        await Harness.Bus.Publish(
            new AtsDepositRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new DepositAtsSuccessEvent(
                sagaId,
                2_000
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.DepositEntrypointState.GlobalTransferProcessing!);

        await Harness.Bus.Publish(
            new GlobalTransferRequest(
                sagaId
            )
        );

        await Harness.Bus.Publish(
            new GlobalManualAllocationSuccessEvent(
                sagaId,
                sagaId,
                transactionNo,
                "FromAccount",
                "ToAccount",
                2_000,
                Currency.THB,
                DateTime.Now,
                DateTime.Now
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, sagaHarness.StateMachine.Final!);
    }

    private async Task VerifySagaExistWithCorrectState(Guid sagaId, ISagaStateMachineTestHarness<StateMachine, DepositEntrypointState> sagaHarness, State state)
    {
        var existsId = await _sagaRepository.ShouldContainSagaInState(sagaId, sagaHarness.StateMachine, x => x.GetState(state.Name), Harness.TestTimeout);
        Assert.True(existsId.HasValue, "Saga was not created using the MessageId");
    }

    private async Task ResponseSagaRequest<T, TR>(TR responseMessage) where T : class
    {
        Assert.True(await Harness.Published.Any<T>());
        var context = Harness.Published.Select<T>().First().Context;
        var responseEndpoint = await Harness.Bus.GetSendEndpoint(context.ResponseAddress!);
        await responseEndpoint.Send(responseMessage!, callback: ctx => ctx.RequestId = context.RequestId);
    }
}
