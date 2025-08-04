using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Queries;
using Pi.WalletService.Application.Services.OnboardService;
using Pi.WalletService.Domain;
using Pi.WalletService.Domain.AggregatesModel.AtsWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.OddWithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.UpBackAggregate;
using Pi.WalletService.Domain.Events.WithdrawEntrypoint;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Withdraw;

public record InitiateWithdrawStateMachine(WithdrawEntrypointRequest RequestData);

public record InitiateWithdrawStateMachineSuccess(
    Guid CorrelationId,
    BankAccountInfo? BankAccountInfo,
    CustomerInfo? CustomerInfo);
public record InitiateWithdrawStateMachineFailed(Guid CorrelationId, string Reason);

public class InitiateWithdrawStateMachineConsumer : IConsumer<InitiateWithdrawStateMachine>
{
    private readonly ISagaUnitOfWork _sagaUnitOfWork;
    private readonly IOnboardService _onboardService;
    private readonly IWalletQueries _walletQueries;
    private readonly ILogger<InitiateWithdrawStateMachineConsumer> _logger;

    public InitiateWithdrawStateMachineConsumer(
        ISagaUnitOfWork sagaUnitOfWork,
        ILogger<InitiateWithdrawStateMachineConsumer> logger,
        IOnboardService onboardService,
        IWalletQueries walletQueries)
    {
        _sagaUnitOfWork = sagaUnitOfWork;
        _logger = logger;
        _onboardService = onboardService;
        _walletQueries = walletQueries;
    }

    public async Task Consume(ConsumeContext<InitiateWithdrawStateMachine> context)
    {
        var requestData = context.Message.RequestData;
        try
        {
            // initiate state machine data based on channel (OnlineViaKKP, ATS)
            switch (requestData.Channel)
            {
                case Channel.OnlineViaKKP:
                    InitOddWithdraw(requestData);
                    break;
                case Channel.ATS:
                    InitAtsWithdraw(requestData);
                    break;
                default:
                    throw new InvalidDataException($"InitiateWithdrawStateMachineConsumer: Channel {requestData.Channel} not supported.");
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
                    throw new InvalidDataException($"InitiateWithdrawStateMachineConsumer: Product {requestData.Product} not supported.");
            }

            await _sagaUnitOfWork.CommitAsync();

            _logger.LogInformation("After commit");
            await context.RespondAsync(await BuildSuccessResponse(requestData));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error while initiating withdraw state machine");
            await _sagaUnitOfWork.RollbackAsync();
            await context.RespondAsync(new InitiateWithdrawStateMachineFailed(context.Message.RequestData.CorrelationId, e.Message));
        }
    }

    private async Task<InitiateWithdrawStateMachineSuccess> BuildSuccessResponse(WithdrawEntrypointRequest request)
    {
        // if channel is not ATS or ODD we will received customerInfo and bankInfo once bank process done.
        if (request.Channel is not (Channel.ATS or Channel.ODD))
        {
            return new InitiateWithdrawStateMachineSuccess(request.CorrelationId, null, null);
        }

        // pre-get needed information for ATS and ODD
        var customerInfo = await _onboardService.GetCustomerInfo(request.CustomerCode);
        var bankInfo = await _walletQueries.GetBankAccount(
            request.UserId,
            request.CustomerCode,
            request.Product,
            TransactionType.Deposit);
        return new InitiateWithdrawStateMachineSuccess(request.CorrelationId, bankInfo, customerInfo);
    }

    private void InitOddWithdraw(WithdrawEntrypointRequest request)
    {
        var oddWithdraw = new OddWithdrawState
        {
            CorrelationId = request.CorrelationId,
            CurrentState = nameof(IntegrationEvents.Models.OddWithdrawState.Initial),
            Product = request.Product,
            Channel = request.Channel,
            ResponseAddress = request.ResponseAddress!,
            RequestId = request.RequestId
        };
        _sagaUnitOfWork.OddWithdrawRepository.AddAsync(oddWithdraw);
    }

    private void InitAtsWithdraw(WithdrawEntrypointRequest request)
    {
        var atsWithdraw = new AtsWithdrawState
        {
            CorrelationId = request.CorrelationId,
            CurrentState = nameof(IntegrationEvents.Models.AtsWithdrawState.Initial),
            Product = request.Product,
            Channel = request.Channel,
            ResponseAddress = request.ResponseAddress!,
            RequestId = request.RequestId
        };
        _sagaUnitOfWork.AtsWithdrawRepository.AddAsync(atsWithdraw);
    }

    private void InitUpBack(WithdrawEntrypointRequest request)
    {
        var upBack = new UpBackState
        {
            CorrelationId = request.CorrelationId,
            CurrentState = nameof(IntegrationEvents.Models.UpBackState.Initial),
            Product = request.Product,
            Channel = request.Channel,
            TransactionType = TransactionType.Withdraw,
        };
        _sagaUnitOfWork.UpBackRepository.AddAsync(upBack);
    }

    private void InitGlobalTransfer(WithdrawEntrypointRequest request)
    {
        var globalTransfer = new GlobalTransferState
        {
            CorrelationId = request.CorrelationId,
            CurrentState = nameof(IntegrationEvents.Models.GlobalTransferState.Initial),
            Product = request.Product,
            Channel = request.Channel,
            TransactionType = TransactionType.Withdraw,
            CustomerId = request.CustomerId!.Value,
            GlobalAccount = request.GlobalAccount!,
            RequestedCurrency = request.RequestedCurrency!.Value,
            RequestedFxCurrency = Currency.THB,
            FxTransactionId = request.FxTransactionId,
            RequestId = request.RequestId,

            // HotFix to enable GE Notification V2 on failed case
            // TODO: Refactor GE D/W Field to be the same after migrate V2
            TransferAmount = request.RequestedAmount,
            FxMarkUpRate = request.FxMarkUp!.Value
        };
        _sagaUnitOfWork.GlobalTransferRepository.AddAsync(globalTransfer);
    }
}
