using System.Globalization;
using System.Net.Mime;
using MassTransit;
using MassTransit.Configuration;
using MassTransit.Serialization;
using Microsoft.Extensions.Options;
using Pi.WalletService.Application.Commands.Deposit;
using Pi.WalletService.Application.Models;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Services.Event;
using Pi.WalletService.Application.Services.UserService;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalTransfer;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.IntegrationEvents.AggregatesModel;
using CashDepositState = Pi.WalletService.IntegrationEvents.Models.CashDepositState;
using CashWithdrawState = Pi.WalletService.IntegrationEvents.Models.CashWithdrawState;
using GlobalWalletTransferState = Pi.WalletService.IntegrationEvents.Models.GlobalWalletTransferState;
using GlobalTransferState = Pi.WalletService.IntegrationEvents.Models.GlobalTransferState;
using WithdrawState = Pi.WalletService.IntegrationEvents.Models.WithdrawState;
using UpBackMachineState = Pi.WalletService.IntegrationEvents.Models.UpBackState;
using OddWithdrawMachineState = Pi.WalletService.IntegrationEvents.Models.OddWithdrawState;
using AtsWithdrawMachineState = Pi.WalletService.IntegrationEvents.Models.AtsWithdrawState;

namespace Pi.WalletService.Infrastructure.Services;

public class EventGeneratorService : IEventGeneratorService
{
    private readonly IDepositEntrypointRepository _depositEntrypointRepository;
    private readonly IWithdrawEntrypointRepository _withdrawEntrypointRepository;
    private readonly IDepositRepository _depositRepository;
    private readonly IQrDepositRepository _qrDepositRepository;
    private readonly IWithdrawRepository _withdrawRepository;
    private readonly ICashDepositRepository _cashDepositRepository;
    private readonly ICashWithdrawRepository _cashWithdrawRepository;
    private readonly IGlobalWalletDepositRepository _globalWalletRepository;
    private readonly IGlobalTransferRepository _globalTransferRepository;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserService _userService;
    private readonly IBus _bus;
    private readonly int _maxPollingWaitingTime;

    public EventGeneratorService(IBus bus,
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        IQrDepositRepository qrDepositRepository,
        IDepositRepository depositRepository,
        ICashWithdrawRepository cashWithdrawRepository,
        ICashDepositRepository cashDepositRepository,
        IWithdrawRepository withdrawRepository,
        IGlobalWalletDepositRepository globalWalletRepository,
        IGlobalTransferRepository globalTransferRepository,
        ITransactionRepository transactionRepository,
        IUserService userService,
        IOptions<MockOptions> mockOptions)
    {
        _depositEntrypointRepository = depositEntrypointRepository;
        _withdrawEntrypointRepository = withdrawEntrypointRepository;
        _depositRepository = depositRepository;
        _qrDepositRepository = qrDepositRepository;
        _bus = bus;
        _cashWithdrawRepository = cashWithdrawRepository;
        _cashDepositRepository = cashDepositRepository;
        _withdrawRepository = withdrawRepository;
        _userService = userService;
        _globalWalletRepository = globalWalletRepository;
        _globalTransferRepository = globalTransferRepository;
        _transactionRepository = transactionRepository;
        _maxPollingWaitingTime = mockOptions.Value.MaxPollingWaitingTimeInSeconds;
    }

    public async Task<KkpDeposit> GenerateKkpDepositEvent(string transactionNo, MockQrDepositEventState eventName, CancellationToken cancellationToke)
    {
        var transaction = await _depositRepository.GetByTransactionNo(transactionNo);
        if (transaction == null)
        {
            throw new InvalidDataException("Transaction not found");
        }

        var isSuccess = eventName != MockQrDepositEventState.DepositFailed;
        var user = await _userService.GetUserInfoByCustomerCode(transaction.CustomerCode);
        var name = $"{user.FirstnameEn} {user.LastnameEn}";

        var payerName = eventName switch
        {
            MockQrDepositEventState.DepositCompleted
                or MockQrDepositEventState.DepositFailed
                or MockQrDepositEventState.InvalidPaymentDateTime => name,
            MockQrDepositEventState.DepositFailedNameMismatch => "wrong name",
            MockQrDepositEventState.DepositFailedInvalidSource => "บจก.ทรู มันนี่ เพื่อเก็บรักษาเงินรับล่วง",
            _ => throw new ArgumentOutOfRangeException(nameof(eventName), eventName, null)
        };

        var paymentDateTime = DateUtils.GetThDateTimeNow();
        if (eventName == MockQrDepositEventState.InvalidPaymentDateTime)
        {
            paymentDateTime = paymentDateTime.AddDays(1);
        }

        var payload = new KkpDeposit(
            isSuccess,
            (double)transaction.RequestedAmount!,
            transaction.CustomerCode,
            transaction.Product.ToString(),
            transaction.QrTransactionNo!,
            "unused",
            paymentDateTime.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
            payerName!,
            "1112000099",
            "014"
        );

        await _bus.Publish(payload, context =>
        {
            context.SetGroupId("1");
            context.SetDeduplicationId(Guid.NewGuid().ToString());
            context.ContentType = new ContentType("application/json");
            context.Serializer = new SystemTextJsonRawMessageSerializerFactory(RawSerializerOptions.All).CreateSerializer();
        }, cancellationToke);

        return payload;
    }

    public async Task<bool> MockGlobalTransaction(string transactionNo, MockGlobalTransactionReasons mockDepositWithdrawReason, CancellationToken cancellationToken)
    {
        try
        {
            switch (mockDepositWithdrawReason)
            {
                case MockGlobalTransactionReasons.FxTransferFailed:
                    await _globalWalletRepository.UpdateGlobalAccountByTransactionNo(transactionNo, "MOCK1000.0000");
                    return true;
                case MockGlobalTransactionReasons.FxRateCompareFailed:
                    await _globalWalletRepository.UpdateRequestedFxAmountByTransactionNo(transactionNo, (decimal)1.11);
                    return true;
                case MockGlobalTransactionReasons.RevertTransferFailed:
                    return await LoopUpdate(() =>
                        _globalWalletRepository.UpdateGlobalAccountByTransactionNoAndState(transactionNo,
                            GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.WithdrawalProcessing),
                            "MOCK10000.0000"), cancellationToken);
                case MockGlobalTransactionReasons.TransferRequestFailed:
                    return await LoopUpdate(() =>
                        _globalWalletRepository.UpdateGlobalAccountByTransactionNoAndState(transactionNo,
                            GlobalWalletTransferState.GetName(() => GlobalWalletTransferState.TransferRequestValidating),
                            "MOCK10000.0000"), cancellationToken);
                default:
                    throw new InvalidDataException(
                        $"MockGlobalTransactionReasons {mockDepositWithdrawReason} not supported");
            }
        }
        catch (KeyNotFoundException)
        {
            throw new InvalidDataException($"TransactionNo ${transactionNo} not found");
        }
    }

    public async Task<bool> MockDepositWithdrawTransaction(string transactionNo, TransactionType transactionType, Product product, MockDepositWithdrawReasons mockDepositWithdrawReason, CancellationToken cancellationToken)
    {
        if (product == Product.GlobalEquities && mockDepositWithdrawReason != MockDepositWithdrawReasons.KkpFailed)
        {
            throw new InvalidDataException($"Product {product} not supported for {mockDepositWithdrawReason}.");
        }

        switch (transactionType)
        {
            case TransactionType.Deposit:
                return await MockDepositState(transactionNo, product, mockDepositWithdrawReason, cancellationToken);
            case TransactionType.Withdraw:
                return await MockWithdrawState(transactionNo, product, mockDepositWithdrawReason, cancellationToken);
            default:
                throw new InvalidDataException($"Transaction type {transactionType} not supported.");
        }
    }

    // MOCK V2

    public async Task<KkpDeposit> GenerateKkpDepositEventV2(string transactionNo, MockQrDepositEventStateV2 eventName, CancellationToken cancellationToken)
    {
        var depositEntrypoint = await _depositEntrypointRepository.GetByTransactionNo(transactionNo);
        if (depositEntrypoint == null)
        {
            throw new InvalidDataException("Transaction not found");
        }

        // find qr transaction no
        var qrTransaction = await _qrDepositRepository.GetAsync(c => c.CorrelationId == depositEntrypoint.CorrelationId, cancellationToken);
        if (qrTransaction == null)
        {
            throw new InvalidDataException("Qr transaction not found");
        }

        var isSuccess = eventName != MockQrDepositEventStateV2.QrDepositFailed;
        var user = await _userService.GetUserInfoByCustomerCode(depositEntrypoint.CustomerCode);
        var name = $"{user.FirstnameEn} {user.LastnameEn}";

        var payerName = eventName switch
        {
            MockQrDepositEventStateV2.QrDepositCompleted
                or MockQrDepositEventStateV2.QrDepositFailed
                or MockQrDepositEventStateV2.FailedAmountMismatch
                or MockQrDepositEventStateV2.InvalidPaymentDateTime => name,
            MockQrDepositEventStateV2.FailedNameMismatch => "wrong name",
            MockQrDepositEventStateV2.FailedInvalidSource => "บจก.ทรู มันนี่ เพื่อเก็บรักษาเงินรับล่วง",
            _ => throw new ArgumentOutOfRangeException(nameof(eventName), eventName, null)
        };

        var paymentDateTime = DateUtils.GetThDateTimeNow();
        if (eventName == MockQrDepositEventStateV2.InvalidPaymentDateTime)
        {
            paymentDateTime = paymentDateTime.AddDays(10);
        }

        var requestedAmount = (double)depositEntrypoint.RequestedAmount;
        if (eventName == MockQrDepositEventStateV2.FailedAmountMismatch)
        {
            requestedAmount *= 2;
        }

        var payload = new KkpDeposit(
            isSuccess,
            requestedAmount,
            depositEntrypoint.CustomerCode,
            depositEntrypoint.Product.ToString(),
            qrTransaction.QrTransactionNo!,
            "unused",
            paymentDateTime.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture),
            payerName,
            "1112000099",
            "014"
        );

        await _bus.Publish(payload, context =>
        {
            context.SetGroupId("1");
            context.SetDeduplicationId(Guid.NewGuid().ToString());
            context.ContentType = new ContentType("application/json");
            context.Serializer = new SystemTextJsonRawMessageSerializerFactory(RawSerializerOptions.All).CreateSerializer();
        }, cancellationToken);

        return payload;
    }

    public async Task<bool> MockGlobalTransactionV2(string transactionNo, TransactionType transactionType, MockGlobalTransactionReasons mockReason,
        CancellationToken cancellationToken)
    {
        try
        {
            // find global transfer by transaction no
            var transactionId = await _transactionRepository.GetIdByNo(transactionNo);
            if (transactionId == null)
            {
                throw new KeyNotFoundException();
            }

            switch (mockReason)
            {
                case MockGlobalTransactionReasons.FxRateCompareFailed:
                    await _globalTransferRepository.UpdateRequestedFxAmountById(transactionId.Value, (decimal)1.11);
                    return true;
                // Only Deposit
                case MockGlobalTransactionReasons.FxTransferFailed:
                    if (transactionType != TransactionType.Deposit)
                    {
                        throw new InvalidDataException("FxTransferFailed only support Deposit");
                    }
                    await _globalTransferRepository.UpdateGlobalAccountById(transactionId.Value, "MOCK1000.0000");
                    return true;
                // Only Withdraw
                case MockGlobalTransactionReasons.RevertTransferFailed:
                    if (transactionType != TransactionType.Withdraw)
                    {
                        throw new InvalidDataException("RevertTransferFailed only support Withdraw");
                    }
                    return await LoopUpdate(() =>
                        _globalTransferRepository.UpdateGlobalAccountByIdAndState(transactionId.Value,
                            GlobalTransferState.GetName(() => GlobalTransferState.WithdrawFxTransferring),
                            "MOCK10000.0000"), cancellationToken);
                case MockGlobalTransactionReasons.FxTransferInsufficientBalance:
                    throw new Exception("FxTransferInsufficientBalance not supported yet." +
                                        "You should be able to manually transfer Exante balance (endpoint: /debug/exante/transfer) to test this case." +
                                        "And then transfer it back.");
                case MockGlobalTransactionReasons.FxFailed:
                    if (transactionType != TransactionType.Deposit)
                    {
                        throw new InvalidDataException("FxFailed only support Deposit");
                    }

                    return await LoopUpdate(() =>
                        _globalTransferRepository.UpdateFxTransactionIdAndState(transactionId.Value,
                            GlobalTransferState.GetName(() => GlobalTransferState.FxValidating),
                            "0000000000000000000000000000"), cancellationToken);
                case MockGlobalTransactionReasons.TransferRequestFailed:
                    if (transactionType != TransactionType.Withdraw)
                    {
                        throw new InvalidDataException("TransferRequestFailed only support Withdraw");
                    }
                    return await LoopUpdate(() =>
                        _globalTransferRepository.UpdateCurrencyAndState(transactionId.Value,
                            GlobalTransferState.GetName(() => GlobalTransferState.FxQueryTransaction),
                            Currency.USD, Currency.USD), cancellationToken);

                default:
                    throw new InvalidDataException(
                        $"MockGlobalTransactionReasons {mockReason} not supported");
            }
        }
        catch (KeyNotFoundException e)
        {
            throw new InvalidDataException($"TransactionNo ${transactionNo} not found : ${e.Message}");
        }
    }

    public async Task<bool> MockDepositWithdrawTransactionV2(string transactionNo, TransactionType transactionType, Product product, MockDepositWithdrawReasons mockDepositWithdrawReason, CancellationToken cancellationToken)
    {
        if (product == Product.GlobalEquities && mockDepositWithdrawReason != MockDepositWithdrawReasons.KkpFailed)
        {
            throw new InvalidDataException($"Product {product} not supported for {mockDepositWithdrawReason}.");
        }

        switch (transactionType)
        {
            case TransactionType.Deposit:
                return await MockDepositStateV2(transactionNo, product, mockDepositWithdrawReason, cancellationToken);
            case TransactionType.Withdraw:
                return await MockWithdrawStateV2(transactionNo, product, mockDepositWithdrawReason, cancellationToken);
            default:
                throw new InvalidDataException($"Transaction type {transactionType} not supported.");
        }
    }

    // Private
    private async Task<bool> MockWithdrawState(string transactionNo, Product product, MockDepositWithdrawReasons mockDepositWithdrawReason, CancellationToken cancellationToken)
    {
        switch (mockDepositWithdrawReason)
        {
            case MockDepositWithdrawReasons.FreewillFailed:
                var stateName = product == Product.Derivatives
                    ? CashWithdrawState.GetName(() => CashWithdrawState.CashWithdrawWaitingForTFexPlatform)
                    : CashWithdrawState.GetName(() => CashWithdrawState.CashWithdrawWaitingForOtpValidation);
                return await LoopUpdate(() =>
                    _cashWithdrawRepository.UpdateAccountCodeByTransactionNoAndState(
                        transactionNo,
                        stateName,
                        "1234567890"), cancellationToken);
            case MockDepositWithdrawReasons.TfexFailed:
                if (product != Product.Derivatives) throw new InvalidDataException($"Product {product} not supported for MockTfexFailed.");
                return await LoopUpdate(() =>
                    _cashWithdrawRepository.UpdateAccountCodeByTransactionNoAndState(
                        transactionNo,
                        CashWithdrawState.GetName(() => CashWithdrawState.CashWithdrawWaitingForOtpValidation),
                    "1234567890"), cancellationToken);
            case MockDepositWithdrawReasons.KkpFailed:
                return await LoopUpdate(() =>
                    _withdrawRepository.UpdateBankAccountByTransactionNoAndState(transactionNo,
                        WithdrawState.GetName(() => WithdrawState.WaitingForConfirmation),
                        "1234567890"), cancellationToken);
            default:
                throw new InvalidDataException($"MockStateReason {mockDepositWithdrawReason} not supported for Withdraw.");
        }
    }

    private async Task<bool> MockDepositState(string transactionNo, Product product, MockDepositWithdrawReasons mockDepositWithdrawReason, CancellationToken cancellationToken)
    {
        switch (mockDepositWithdrawReason)
        {
            case MockDepositWithdrawReasons.FreewillFailed:
                return await LoopUpdate(() =>
                    _cashDepositRepository.UpdateAccountCodeByTransactionNoAndState(
                        transactionNo,
                        CashDepositState.GetName(() => CashDepositState.Received),
                        "1234567890"), cancellationToken);
            case MockDepositWithdrawReasons.TfexFailed:
                if (product != Product.Derivatives) throw new InvalidDataException($"Product {product} not supported for MockTfexFailed.");
                return await LoopUpdate(() =>
                    _cashDepositRepository.UpdateAccountCodeByTransactionNoAndState(
                        transactionNo,
                        CashDepositState.GetName(() => CashDepositState.CashDepositWaitingForGateway),
                        "1234567890"), cancellationToken);
            default:
                throw new InvalidDataException($"MockStateReason {mockDepositWithdrawReason} not supported for Deposit.");
        }
    }

    private async Task<bool> MockWithdrawStateV2(string transactionNo, Product product,
        MockDepositWithdrawReasons mockDepositWithdrawReason, CancellationToken cancellationToken)
    {
        // find channel from transaction no
        if (String.IsNullOrWhiteSpace(transactionNo) || transactionNo.Length < 2)
        {
            throw new InvalidDataException("TransactionNo is invalid");
        }
        var channel = transactionNo.Substring(0, 2) switch
        {
            "AS" => Channel.ATS,
            "DH" or "GE" => Channel.OnlineViaKKP,
            _ => throw new InvalidDataException("Channel not found")
        };

        switch (mockDepositWithdrawReason)
        {
            case MockDepositWithdrawReasons.FreewillFailed:
                if (channel == Channel.ATS)
                {
                    return await LoopUpdate(() =>
                        _withdrawEntrypointRepository.UpdateAccountCodeByTransactionNoAndAtsState(transactionNo,
                            AtsWithdrawMachineState.GetName(() => AtsWithdrawMachineState.RequestingWithdrawAts),
                            "1234567890"), cancellationToken);
                }

                var stateName = product == Product.Derivatives
                    ? UpBackMachineState.GetName(() => UpBackMachineState.WithdrawUpdatingAccountBalance)
                    : UpBackMachineState.GetName(() => UpBackMachineState.WithdrawReceived);
                return await LoopUpdate(() =>
                    _withdrawEntrypointRepository.UpdateAccountCodeByTransactionNoAndUpBackState(transactionNo,
                        stateName,
                        "1234567890"), cancellationToken);
            case MockDepositWithdrawReasons.TfexFailed:
                if (product != Product.Derivatives) throw new InvalidDataException($"Product {product} not supported for MockTfexFailed.");
                return await LoopUpdate(() =>
                    _withdrawEntrypointRepository.UpdateAccountCodeByTransactionNoAndUpBackState(transactionNo,
                        UpBackMachineState.GetName(() => UpBackMachineState.WithdrawUpdatingTradingPlatform),
                        "1234567890"), cancellationToken);
            case MockDepositWithdrawReasons.KkpFailed:
                return await LoopUpdate(() =>
                    _withdrawEntrypointRepository.UpdateBankAccountNoByTransactionNoAndOddState(transactionNo,
                        OddWithdrawMachineState.GetName(() => OddWithdrawMachineState.WaitingForConfirmation),
                        "1234567890"), cancellationToken);
            default:
                throw new InvalidDataException($"MockStateReason {mockDepositWithdrawReason} not supported for Withdraw.");
        }
    }

    private async Task<bool> MockDepositStateV2(string transactionNo, Product product,
        MockDepositWithdrawReasons mockDepositWithdrawReasons, CancellationToken cancellationToken)
    {
        switch (mockDepositWithdrawReasons)
        {
            case MockDepositWithdrawReasons.FreewillFailed:
                return await LoopUpdate(() =>
                    _depositEntrypointRepository.UpdateAccountCodeByTransactionNoAndUpBackState(
                        transactionNo,
                        UpBackMachineState.GetName(() => UpBackMachineState.DepositReceived),
                        "1234567890"), cancellationToken);
            case MockDepositWithdrawReasons.TfexFailed:
                if (product != Product.Derivatives) throw new InvalidDataException($"Product {product} not supported for MockTfexFailed.");
                return await LoopUpdate(() =>
                    _depositEntrypointRepository.UpdateAccountCodeByTransactionNoAndUpBackState(
                        transactionNo,
                        UpBackMachineState.GetName(() => UpBackMachineState.DepositWaitingForGateway),
                        "1234567890"), cancellationToken);
            default:
                throw new InvalidDataException($"MockStateReason {mockDepositWithdrawReasons} not supported for Deposit.");
        }
    }
    private async Task<bool> LoopUpdate(Func<Task<bool>> updateFunction, CancellationToken cancellationToken)
    {
        var startTime = DateTime.Now;
        var maxWaitTime = TimeSpan.FromSeconds(_maxPollingWaitingTime);
        var pollingInterval = TimeSpan.FromMilliseconds(50);

        while (DateTime.Now - startTime < maxWaitTime)
        {
            var isUpdate = await updateFunction();
            if (isUpdate)
            {
                return true;
            }

            await Task.Delay(pollingInterval, cancellationToken);
        }

        return false;
    }

}
