using System.Globalization;
using System.Runtime.Serialization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.QrDepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events.Deposit;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands.Deposit;

public record GenerateDepositQrRequest(Guid CorrelationId, decimal ThbAmount, string AccountCode, Product Product, int ExpireTimeInMinute, string TransactionNo);

public record GenerateDepositQrV2Request(Guid CorrelationId);

[Serializable]
public class UnableToGenerateQrException : Exception
{
    public UnableToGenerateQrException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public UnableToGenerateQrException()
    {
    }

    public UnableToGenerateQrException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected UnableToGenerateQrException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

[Serializable]
public class AmountExceedAllowedAmountException : Exception
{
    public AmountExceedAllowedAmountException(string message, Exception? innerException) : base(message, innerException)
    {
    }

    public AmountExceedAllowedAmountException()
    {
    }

    public AmountExceedAllowedAmountException(string message)
        : base(message)
    {
    }

    // Without this constructor, deserialization will fail
    protected AmountExceedAllowedAmountException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

public class GenerateQrConsumer :
    SagaConsumer,
    IConsumer<GenerateDepositQrRequest>,
    IConsumer<GenerateDepositQrV2Request>
{
    private readonly IQrDepositRepository _qrDepositRepository;
    private readonly IBankService _cgsBankService;
    private readonly ILogger<GenerateQrConsumer> _logger;
    private readonly decimal _fee;

    public GenerateQrConsumer(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        IQrDepositRepository qrDepositRepository,
        IBankService cgsBankService,
        ILogger<GenerateQrConsumer> logger,
        IOptionsSnapshot<FeeOptions> fee) : base(depositEntrypointRepository, withdrawEntrypointRepository)
    {
        _qrDepositRepository = qrDepositRepository;
        _cgsBankService = cgsBankService;
        _logger = logger;
        var depositFee = decimal.Parse(fee.Value.KKP.DepositFee);
        _fee = depositFee > 0 ? depositFee : 1;
    }

    public async Task Consume(ConsumeContext<GenerateDepositQrRequest> context)
    {
        var resp = await GenerateQrCode(context.Message.TransactionNo, context.Message.ThbAmount, context.Message.AccountCode, context.Message.Product, context.Message.ExpireTimeInMinute);
        await context.RespondAsync(resp);
    }

    public async Task Consume(ConsumeContext<GenerateDepositQrV2Request> context)
    {
        var depositEntrypoint = await GetDepositEntrypointByIdAsync(context.Message.CorrelationId);
        var qrDeposit = await _qrDepositRepository.GetAsync(state => state.CorrelationId == context.Message.CorrelationId);

        if (depositEntrypoint == null || qrDeposit == null)
        {
            throw new UnableToGenerateQrException("Unable to generate QR");
        }

        var resp = await GenerateQrCode(depositEntrypoint.TransactionNo!, depositEntrypoint.RequestedAmount, depositEntrypoint.AccountCode, depositEntrypoint.Product, qrDeposit.QrCodeExpiredTimeInMinute);
        await context.RespondAsync(new QrCodeGeneratedV2(depositEntrypoint.TransactionNo!, resp.QrValue, resp.QrTransactionRef, resp.QrTransactionNo, resp.QrGenerateDateTime));
    }

    private async Task<QrCodeGenerated> GenerateQrCode(
        string transactionNo,
        decimal requestedAmount,
        string accountCode,
        Product product,
        int expireTimeInMinute)
    {
        var transactionRefCode = $"RF{DateTime.Now.ToString("yyyyMMddHHmmssffff", CultureInfo.InvariantCulture)}";

        // Amount Should be validated before reaching this state but will validate anyway just in case
        if (requestedAmount < _fee || requestedAmount > 2_000_000)
        {
            throw new AmountExceedAllowedAmountException($"Requested amount should between 1 to 2,000,000 but get {requestedAmount}");
        }

        try
        {
            var resp = await _cgsBankService.GenerateQR(
                transactionNo,
                requestedAmount,
                transactionRefCode,
                accountCode,
                product.ToString().ToUpper(),
                expireTimeInMinute);

            return new QrCodeGenerated(resp.QRValue, transactionRefCode, transactionNo, DateTime.Now);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Transaction No: {TransactionNo} GenerateQRConsumer: Unable to Generate QR with Exception: {Message}", transactionNo, e.Message);
            throw new UnableToGenerateQrException($"Unable to Generate QR");
        }
    }
}
