using MassTransit;
using MassTransit.Events;
using MassTransit.Metadata;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Pi.WalletService.Application.Commands;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.StateMachines.QrDeposit;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.Domain.Events.QrDeposit;
using Pi.WalletService.Domain.Events.Refund;
using Pi.WalletService.IntegrationEvents;
using StateMachine = Pi.WalletService.Application.StateMachines.QrDeposit.StateMachine;

namespace Pi.WalletService.Application.Tests.StateMachines.QrDeposit;

public class StateMachineTests : ConsumerTest
{
    private readonly ISagaRepository<QrDepositState> _sagaRepository;

    public StateMachineTests()
    {
        IntegrationEvents.Models.QrDepositState.CleanUp();
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<StateMachine, QrDepositState>(typeof(StateDefinition));
            })
            .BuildServiceProvider(true);
        _sagaRepository = Provider.GetRequiredService<ISagaRepository<QrDepositState>>();
    }

    [Fact]
    public async void Should_Handle_Transition_Qr_Deposit_State_Correctly_When_Generate_Qr_Failed()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, QrDepositState>();
        var sagaId = NewId.NextGuid();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrCodeGenerating!);

        await ResponseSagaRequest<GenerateDepositQrV2Request, Fault<GenerateDepositQrV2Request>>(new FaultEvent<GenerateDepositQrV2Request>(
            new GenerateDepositQrV2Request(sagaId),
            Guid.NewGuid(),
            new BusHostInfo(false),
            new List<ExceptionInfo>
            {
                new FaultExceptionInfo(new Exception("Error"))
            },
            new[] { "string" }));

        await Harness.Bus.Publish(
            new DepositQrFailedEvent(
                sagaId,
                "",
                null,
                null,
                null,
                0
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrDepositFailed!);
    }

    [Fact(Skip = "Flaky")]
    public async void Should_Handle_Transition_Qr_Deposit_State_Correctly_When_Payment_Callback_Succeed()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, QrDepositState>();
        var sagaId = NewId.NextGuid();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        var state = sagaHarness.Sagas.Contains(sagaId);
        state.QrCodeExpiredTimeInMinute = 5;

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrCodeGenerating!);

        await ResponseSagaRequest<GenerateDepositQrV2Request, QrCodeGeneratedV2>(new QrCodeGeneratedV2(transactionNo, "10.00", "ABCD", transactionNo, DateTime.Now));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.WaitingForPayment!);

        await Harness.Bus.Publish(
            new DepositPaymentCallbackReceived(
                transactionNo,
                0,
                (decimal)10.00,
                DateTime.Now,
                "",
                "",
                "",
                "",
                ""
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositEntrypointUpdating!);

        await ResponseSagaRequest<UpdateDepositEntrypointRequest, UpdateDepositEntrypointSuccess>(new UpdateDepositEntrypointSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentSourceValidating!);

        await ResponseSagaRequest<ValidatePaymentSourceV2, DepositValidatePaymentSourceSucceed>(new DepositValidatePaymentSourceSucceed(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentNameValidating!);

        await ResponseSagaRequest<ValidatePaymentNameV2, DepositValidatePaymentNameSucceed>(new DepositValidatePaymentNameSucceed("", ""));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentAmountValidating!);

        await ResponseSagaRequest<ValidatePaymentAmountV2, DepositValidatePaymentAmountSucceed>(new DepositValidatePaymentAmountSucceed(0));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrDepositCompleted!);
    }

    [Fact]
    public async void Should_Handle_Transition_Qr_Deposit_State_Correctly_When_Payment_Callback_Failed()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, QrDepositState>();
        var sagaId = NewId.NextGuid();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        var state = sagaHarness.Sagas.Contains(sagaId);
        state.QrCodeExpiredTimeInMinute = 5;

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrCodeGenerating!);

        await ResponseSagaRequest<GenerateDepositQrV2Request, QrCodeGeneratedV2>(new QrCodeGeneratedV2(transactionNo, "10.00", "ABCD", transactionNo, DateTime.Now));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.WaitingForPayment!);

        await Harness.Bus.Publish(
            new DepositPaymentFailed(
                transactionNo,
                0,
                (decimal)10.00,
                DateTime.Now,
                "",
                "",
                "",
                "",
                ""
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.WaitingForPayment!);
    }

    [Fact(Skip = "Flaky")]
    public async void Should_Handle_Transition_Qr_Deposit_State_Correctly_When_Payment_Callback_At_Payment_Not_Received()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, QrDepositState>();
        var sagaId = NewId.NextGuid();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        var state = sagaHarness.Sagas.Contains(sagaId);
        state.QrCodeExpiredTimeInMinute = 5;

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrCodeGenerating!);

        await ResponseSagaRequest<GenerateDepositQrV2Request, QrCodeGeneratedV2>(new QrCodeGeneratedV2(transactionNo, "10.00", "ABCD", transactionNo, DateTime.Now));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.WaitingForPayment!);

        await Harness.Bus.Publish<QrExpired>(
            new
            {
                QrStateId = sagaId
            }
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentNotReceived!);

        await Harness.Bus.Publish(
            new DepositPaymentCallbackReceived(
                transactionNo,
                0,
                (decimal)10.00,
                DateTime.Now,
                "",
                "",
                "",
                "",
                ""
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositEntrypointUpdating!);

        await ResponseSagaRequest<UpdateDepositEntrypointRequest, UpdateDepositEntrypointSuccess>(new UpdateDepositEntrypointSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentSourceValidating!);

        await ResponseSagaRequest<ValidatePaymentSourceV2, DepositValidatePaymentSourceSucceed>(new DepositValidatePaymentSourceSucceed(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentNameValidating!);

        await ResponseSagaRequest<ValidatePaymentNameV2, DepositValidatePaymentNameSucceed>(new DepositValidatePaymentNameSucceed("", ""));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentAmountValidating!);

        await ResponseSagaRequest<ValidatePaymentAmountV2, DepositValidatePaymentAmountSucceed>(new DepositValidatePaymentAmountSucceed(0));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrDepositCompleted!);
    }

    [Fact]
    public async void Should_Handle_Transition_Qr_Deposit_State_Correctly_When_Validate_Payment_Source_Failed()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, QrDepositState>();
        var sagaId = NewId.NextGuid();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        var state = sagaHarness.Sagas.Contains(sagaId);
        state.QrCodeExpiredTimeInMinute = 5;

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrCodeGenerating!);

        await ResponseSagaRequest<GenerateDepositQrV2Request, QrCodeGeneratedV2>(new QrCodeGeneratedV2(transactionNo, "10.00", "ABCD", transactionNo, DateTime.Now));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.WaitingForPayment!);

        await Harness.Bus.Publish(
            new DepositPaymentCallbackReceived(
                transactionNo,
                0,
                (decimal)10.00,
                DateTime.Now,
                "",
                "",
                "",
                "",
                ""
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositEntrypointUpdating!);

        await ResponseSagaRequest<UpdateDepositEntrypointRequest, UpdateDepositEntrypointSuccess>(new UpdateDepositEntrypointSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentSourceValidating!);

        await ResponseSagaRequest<ValidatePaymentSourceV2, Fault<ValidatePaymentSourceV2>>(new FaultEvent<ValidatePaymentSourceV2>(
            new ValidatePaymentSourceV2(sagaId),
            Guid.NewGuid(),
            new BusHostInfo(false),
            new List<ExceptionInfo>
            {
                new FaultExceptionInfo(new Exception("Error"))
            },
            new[] { "string" }));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositFailedInvalidSource!);
    }

    [Fact(Skip = "Flaky")]
    public async void Should_Handle_Transition_Qr_Deposit_State_Correctly_When_Validate_Payment_Name_Failed()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, QrDepositState>();
        var sagaId = NewId.NextGuid();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        var state = sagaHarness.Sagas.Contains(sagaId);
        state.QrCodeExpiredTimeInMinute = 5;

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrCodeGenerating!);

        await ResponseSagaRequest<GenerateDepositQrV2Request, QrCodeGeneratedV2>(new QrCodeGeneratedV2(transactionNo, "10.00", "ABCD", transactionNo, DateTime.Now));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.WaitingForPayment!);

        await Harness.Bus.Publish(
            new DepositPaymentCallbackReceived(
                transactionNo,
                0,
                (decimal)10.00,
                DateTime.Now,
                "",
                "",
                "",
                "",
                ""
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositEntrypointUpdating!);

        await ResponseSagaRequest<UpdateDepositEntrypointRequest, UpdateDepositEntrypointSuccess>(new UpdateDepositEntrypointSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentSourceValidating!);

        await ResponseSagaRequest<ValidatePaymentSourceV2, DepositValidatePaymentSourceSucceed>(new DepositValidatePaymentSourceSucceed(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentNameValidating!);

        await ResponseSagaRequest<ValidatePaymentNameV2, Fault<ValidatePaymentNameV2>>(new FaultEvent<ValidatePaymentNameV2>(
            new ValidatePaymentNameV2(sagaId),
            Guid.NewGuid(),
            new BusHostInfo(false),
            new List<ExceptionInfo>
            {
                new FaultExceptionInfo(new Exception("Error"))
            },
            new[] { "string" }));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositFailedNameMismatch!);
    }

    [Fact]
    public async void Should_Handle_Transition_Qr_Deposit_State_Correctly_When_Validate_Payment_Amount_Failed()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, QrDepositState>();
        var sagaId = NewId.NextGuid();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        var state = sagaHarness.Sagas.Contains(sagaId);
        state.QrCodeExpiredTimeInMinute = 5;

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrCodeGenerating!);

        await ResponseSagaRequest<GenerateDepositQrV2Request, QrCodeGeneratedV2>(new QrCodeGeneratedV2(transactionNo, "10.00", "ABCD", transactionNo, DateTime.Now));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.WaitingForPayment!);

        await Harness.Bus.Publish(
            new DepositPaymentCallbackReceived(
                transactionNo,
                0,
                (decimal)10.00,
                DateTime.Now,
                "",
                "",
                "",
                "",
                ""
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositEntrypointUpdating!);

        await ResponseSagaRequest<UpdateDepositEntrypointRequest, UpdateDepositEntrypointSuccess>(new UpdateDepositEntrypointSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentSourceValidating!);

        await ResponseSagaRequest<ValidatePaymentSourceV2, DepositValidatePaymentSourceSucceed>(new DepositValidatePaymentSourceSucceed(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentNameValidating!);

        await ResponseSagaRequest<ValidatePaymentNameV2, DepositValidatePaymentNameSucceed>(new DepositValidatePaymentNameSucceed("", ""));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentAmountValidating!);

        await ResponseSagaRequest<ValidatePaymentAmountV2, Fault<ValidatePaymentAmountV2>>(new FaultEvent<ValidatePaymentAmountV2>(
            new ValidatePaymentAmountV2(sagaId, 0),
            Guid.NewGuid(),
            new BusHostInfo(false),
            new List<ExceptionInfo>
            {
                new FaultExceptionInfo(new Exception("Error"))
            },
            new[] { "string" }));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositFailedAmountMismatch!);
    }

    [Fact]
    public async void Should_Handle_Transition_Qr_Deposit_State_Correctly_When_Approve_Name_Mismatch_Succeed()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, QrDepositState>();
        var sagaId = NewId.NextGuid();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        var state = sagaHarness.Sagas.Contains(sagaId);
        state.QrCodeExpiredTimeInMinute = 5;

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrCodeGenerating!);

        await ResponseSagaRequest<GenerateDepositQrV2Request, QrCodeGeneratedV2>(new QrCodeGeneratedV2(transactionNo, "10.00", "ABCD", transactionNo, DateTime.Now));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.WaitingForPayment!);

        await Harness.Bus.Publish(
            new DepositPaymentCallbackReceived(
                transactionNo,
                0,
                (decimal)10.00,
                DateTime.Now,
                "",
                "",
                "",
                "",
                ""
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositEntrypointUpdating!);

        await ResponseSagaRequest<UpdateDepositEntrypointRequest, UpdateDepositEntrypointSuccess>(new UpdateDepositEntrypointSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentSourceValidating!);

        await ResponseSagaRequest<ValidatePaymentSourceV2, DepositValidatePaymentSourceSucceed>(new DepositValidatePaymentSourceSucceed(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentNameValidating!);

        await ResponseSagaRequest<ValidatePaymentNameV2, Fault<ValidatePaymentNameV2>>(new FaultEvent<ValidatePaymentNameV2>(
            new ValidatePaymentNameV2(sagaId),
            Guid.NewGuid(),
            new BusHostInfo(false),
            new List<ExceptionInfo>
            {
                new FaultExceptionInfo(new Exception("Error"))
            },
            new[] { "string" }));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositFailedNameMismatch!);

        await Harness.Bus.Publish(
            new ApproveNameMismatch(
                transactionNo
            )
        );

        await ResponseSagaRequest<ValidatePaymentNameV2, DepositValidatePaymentNameSucceed>(new DepositValidatePaymentNameSucceed("", ""));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentAmountValidating!);

        await ResponseSagaRequest<ValidatePaymentAmountV2, DepositValidatePaymentAmountSucceed>(new DepositValidatePaymentAmountSucceed(0));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrDepositCompleted!);
    }

    [Fact(Skip = "Flaky")]
    public async void Should_Handle_Transition_Qr_Deposit_State_Correctly_When_Refund_Succeed()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, QrDepositState>();
        var sagaId = NewId.NextGuid();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        var state = sagaHarness.Sagas.Contains(sagaId);
        state.QrCodeExpiredTimeInMinute = 5;

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrCodeGenerating!);

        await ResponseSagaRequest<GenerateDepositQrV2Request, QrCodeGeneratedV2>(new QrCodeGeneratedV2(transactionNo, "10.00", "ABCD", transactionNo, DateTime.Now));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.WaitingForPayment!);

        await Harness.Bus.Publish(
            new DepositPaymentCallbackReceived(
                transactionNo,
                0,
                (decimal)10.00,
                DateTime.Now,
                "",
                "",
                "",
                "",
                ""
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositEntrypointUpdating!);

        await ResponseSagaRequest<UpdateDepositEntrypointRequest, UpdateDepositEntrypointSuccess>(new UpdateDepositEntrypointSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentSourceValidating!);

        await ResponseSagaRequest<ValidatePaymentSourceV2, DepositValidatePaymentSourceSucceed>(new DepositValidatePaymentSourceSucceed(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentNameValidating!);

        await ResponseSagaRequest<ValidatePaymentNameV2, Fault<ValidatePaymentNameV2>>(new FaultEvent<ValidatePaymentNameV2>(
            new ValidatePaymentNameV2(sagaId),
            Guid.NewGuid(),
            new BusHostInfo(false),
            new List<ExceptionInfo>
            {
                new FaultExceptionInfo(new Exception("Error"))
            },
            new[] { "string" }));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositFailedNameMismatch!);

        await Harness.Bus.Publish(
            new RefundingDeposit(
                sagaId,
                transactionNo,
                "",
                0
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.Refunding!);

        await Harness.Bus.Publish(
            new RefundSucceedEvent(
                sagaId,
                transactionNo,
                "",
                0
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.RefundSucceed!);
    }

    [Fact]
    public async void Should_Handle_Transition_Qr_Deposit_State_Correctly_When_Refund_Failed()
    {
        var sagaHarness = Harness.GetSagaStateMachineHarness<StateMachine, QrDepositState>();
        var sagaId = NewId.NextGuid();
        const string transactionNo = "transaction_no";

        await Harness.Bus.Publish(
            new QrDepositRequest(
                sagaId,
                transactionNo
            )
        );

        var state = sagaHarness.Sagas.Contains(sagaId);
        state.QrCodeExpiredTimeInMinute = 5;

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.Received!);

        await ResponseSagaRequest<LogActivity, LogActivitySuccess>(new LogActivitySuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.QrCodeGenerating!);

        await ResponseSagaRequest<GenerateDepositQrV2Request, QrCodeGeneratedV2>(new QrCodeGeneratedV2(transactionNo, "10.00", "ABCD", transactionNo, DateTime.Now));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.WaitingForPayment!);

        await Harness.Bus.Publish(
            new DepositPaymentCallbackReceived(
                transactionNo,
                0,
                (decimal)10.00,
                DateTime.Now,
                "",
                "",
                "",
                "",
                ""
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositEntrypointUpdating!);

        await ResponseSagaRequest<UpdateDepositEntrypointRequest, UpdateDepositEntrypointSuccess>(new UpdateDepositEntrypointSuccess(sagaId));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentSourceValidating!);

        await ResponseSagaRequest<ValidatePaymentSourceV2, DepositValidatePaymentSourceSucceed>(new DepositValidatePaymentSourceSucceed(transactionNo));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.PaymentNameValidating!);

        await ResponseSagaRequest<ValidatePaymentNameV2, Fault<ValidatePaymentNameV2>>(new FaultEvent<ValidatePaymentNameV2>(
            new ValidatePaymentNameV2(sagaId),
            Guid.NewGuid(),
            new BusHostInfo(false),
            new List<ExceptionInfo>
            {
                new FaultExceptionInfo(new Exception("Error"))
            },
            new[] { "string" }));

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.DepositFailedNameMismatch!);

        await Harness.Bus.Publish(
            new RefundingDeposit(
                sagaId,
                transactionNo,
                "",
                0
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.Refunding!);

        await Harness.Bus.Publish(
            new RefundFailedEvent(
                sagaId,
                transactionNo
            )
        );

        await VerifySagaExistWithCorrectState(sagaId, sagaHarness, IntegrationEvents.Models.QrDepositState.RefundFailed!);
    }

    private async Task VerifySagaExistWithCorrectState(Guid sagaId, ISagaStateMachineTestHarness<StateMachine, QrDepositState> sagaHarness, State state)
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
