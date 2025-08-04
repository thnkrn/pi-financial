using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.AtsDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.OddDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.Domain.Events.DepositEntrypoint;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
namespace Pi.WalletService.Application.Commands.Deposit;

public record InitiateDepositStateMachine(DepositEntrypointRequest RequestData);

public record InitiateDepositStateMachineSuccess(Guid CorrelationId);

public record InitiateDepositStateMachineFailed(Guid CorrelationId, string Reason);

public class InitiateDepositStateMachineConsumer : IConsumer<InitiateDepositStateMachine>
{
    private readonly ISagaUnitOfWork _sagaUnitOfWork;
    private readonly ILogger<InitiateDepositStateMachineConsumer> _logger;

    public InitiateDepositStateMachineConsumer(
        ISagaUnitOfWork sagaUnitOfWork,
        ILogger<InitiateDepositStateMachineConsumer> logger)
    {
        _sagaUnitOfWork = sagaUnitOfWork;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InitiateDepositStateMachine> context)
    {
        var requestData = context.Message.RequestData;
        try
        {
            // initiate state machine data based on channel (QR, ODD)
            switch (requestData.Channel)
            {
                case Channel.QR:
                    InitQrDeposit(requestData);
                    break;
                case Channel.ODD:
                    InitOddDeposit(requestData);
                    break;
                case Channel.ATS:
                    InitAtsDeposit(requestData);
                    break;
                default:
                    throw new InvalidDataException($"InitiateDepositStateMachineConsumer: Channel {requestData.Channel} not supported.");
            }

            // initiate state machine data based on product (GlobalEquities, NonGlobalEquities)
            switch (requestData.Product)
            {
                case Product.GlobalEquities:
                    InitGlobalTransfer(requestData);
                    break;
                case Product.Cash:
                case Product.Derivatives:
                case Product.CashBalance:
                case Product.CreditBalance:
                case Product.CreditBalanceSbl:
                    InitUpBack(requestData);
                    break;
                default:
                    throw new InvalidDataException($"InitiateDepositStateMachineConsumer: Product {requestData.Product} not supported.");
            }

            await _sagaUnitOfWork.CommitAsync();

            _logger.LogInformation("After commit");
            await context.RespondAsync(new InitiateDepositStateMachineSuccess(context.Message.RequestData.CorrelationId));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while initiating deposit state machine");
            await _sagaUnitOfWork.RollbackAsync();
            await context.RespondAsync(new InitiateDepositStateMachineFailed(context.Message.RequestData.CorrelationId, e.Message));
        }
    }

    private void InitQrDeposit(DepositEntrypointRequest request)
    {
        _logger.LogInformation("Initiate QR Deposit State Machine with request {@RequestData}", request);
        var qrDeposit = new QrDepositState
        {
            CorrelationId = request.CorrelationId,
            CurrentState = nameof(IntegrationEvents.Models.QrDepositState.Initial),
            Product = request.Product,
            Channel = request.Channel,
            QrCodeExpiredTimeInMinute = request.QrCodeExpiredTimeInMinute!.Value,
        };
        _sagaUnitOfWork.QrDepositRepository.AddAsync(qrDeposit);
    }

    private void InitOddDeposit(DepositEntrypointRequest request)
    {
        var oddDeposit = new OddDepositState
        {
            CorrelationId = request.CorrelationId,
            CurrentState = nameof(IntegrationEvents.Models.OddDepositState.Initial),
            Product = request.Product,
            Channel = request.Channel,
            ResponseAddress = request.ResponseAddress!,
            RequestId = request.RequestId
        };
        _sagaUnitOfWork.OddDepositRepository.AddAsync(oddDeposit);
    }

    private void InitAtsDeposit(DepositEntrypointRequest request)
    {
        var atsDeposit = new AtsDepositState
        {
            CorrelationId = request.CorrelationId,
            CurrentState = nameof(IntegrationEvents.Models.AtsDepositState.Initial),
            Product = request.Product,
            Channel = request.Channel,
            ResponseAddress = request.ResponseAddress!,
            RequestId = request.RequestId
        };
        _sagaUnitOfWork.AtsDepositRepository.AddAsync(atsDeposit);
    }

    private void InitUpBack(DepositEntrypointRequest request)
    {
        var upBack = new UpBackState
        {
            CorrelationId = request.CorrelationId,
            CurrentState = nameof(IntegrationEvents.Models.UpBackState.Initial),
            Product = request.Product,
            Channel = request.Channel,
            TransactionType = TransactionType.Deposit,
        };
        _sagaUnitOfWork.UpBackRepository.AddAsync(upBack);
    }

    private void InitGlobalTransfer(DepositEntrypointRequest request)
    {
        var globalTransfer = new GlobalTransferState
        {
            CorrelationId = request.CorrelationId,
            CurrentState = nameof(IntegrationEvents.Models.GlobalTransferState.Initial),
            Product = request.Product,
            Channel = request.Channel,
            TransactionType = TransactionType.Deposit,
            CustomerId = request.CustomerId!.Value,
            GlobalAccount = request.GlobalAccount!,
            RequestedCurrency = request.RequestedCurrency!.Value,
            RequestedFxRate = request.RequestedFxAmount!.Value,
            RequestedFxCurrency = request.RequestedFxCurrency!.Value,
            RequestId = request.RequestId,
            FxMarkUpRate = request.FxMarkUp!.Value
        };
        _sagaUnitOfWork.GlobalTransferRepository.AddAsync(globalTransfer);
    }
}
