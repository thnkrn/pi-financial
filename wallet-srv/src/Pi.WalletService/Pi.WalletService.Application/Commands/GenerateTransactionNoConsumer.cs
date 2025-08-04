using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Utilities;
using Pi.WalletService.Domain.AggregatesModel.CashAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositAggregate;
using Pi.WalletService.Domain.AggregatesModel.DepositEntrypointAggregate;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using Pi.WalletService.Domain.AggregatesModel.TransactionAggregate;
using Pi.WalletService.Domain.AggregatesModel.WithdrawEntrypointAggregate;
using Pi.WalletService.Domain.Events;
using Pi.WalletService.IntegrationEvents.AggregatesModel;

namespace Pi.WalletService.Application.Commands;

public record GenerateTransactionNo(Guid CorrelationId, Product Product, Channel Channel, TransactionType TransactionType, bool IsV2);

public class GenerateTransactionNoConsumer : IConsumer<GenerateTransactionNo>
{
    private readonly IDepositEntrypointRepository _depositEntrypointRepository;
    private readonly IWithdrawEntrypointRepository _withdrawEntrypointRepository;
    private readonly IDepositRepository _depositRepository;
    private readonly IGlobalWalletDepositRepository _globalWalletDepositRepository;
    private readonly ICashWithdrawRepository _cashWithdrawRepository;
    private readonly IOptionsSnapshot<TransactionNoCutOffTimeOptions> _transactionNoCutOffTimeOptions;
    private readonly ILogger<GenerateTransactionNoConsumer> _logger;

    public GenerateTransactionNoConsumer(
        IDepositEntrypointRepository depositEntrypointRepository,
        IWithdrawEntrypointRepository withdrawEntrypointRepository,
        IDepositRepository depositRepository,
        IGlobalWalletDepositRepository globalWalletDepositRepository,
        ICashWithdrawRepository cashWithdrawRepository,
        IOptionsSnapshot<TransactionNoCutOffTimeOptions> transactionNoCutOffTimeOptions,
        ILogger<GenerateTransactionNoConsumer> logger
    )
    {
        _depositEntrypointRepository = depositEntrypointRepository;
        _withdrawEntrypointRepository = withdrawEntrypointRepository;
        _depositRepository = depositRepository;
        _globalWalletDepositRepository = globalWalletDepositRepository;
        _cashWithdrawRepository = cashWithdrawRepository;
        _transactionNoCutOffTimeOptions = transactionNoCutOffTimeOptions;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GenerateTransactionNo> context)
    {
        var transactionNo = context.Message.IsV2
            ? await GetTransactionNoV2(context.Message.Product, context.Message.Channel, context.Message.TransactionType, context.Message.CorrelationId)
            : await GetTransactionNo(context.Message.Product, context.Message.Channel, context.Message.TransactionType, context.Message.CorrelationId);

        await context.RespondAsync(new TransactionNoGenerated(transactionNo));
    }

    private async Task<string> GetTransactionNo(Product product, Channel channel, TransactionType transactionType, Guid correlationId)
    {
        string transactionNo;

        switch (product)
        {
            case Product.Cash:
            case Product.CashBalance:
            case Product.CreditBalance:
            case Product.CreditBalanceSbl:
            case Product.Derivatives:
                transactionNo = transactionType switch
                {
                    TransactionType.Deposit => await GenerateTransactionNo(_depositRepository, "DH", channel,
                        transactionType, correlationId),
                    TransactionType.Withdraw => await GenerateTransactionNo(_cashWithdrawRepository, "DH", channel,
                        transactionType, correlationId),
                    TransactionType.Transfer
                        or TransactionType.Refund
                        or TransactionType.Unknown => throw new NotImplementedException(),
                    _ => throw new ArgumentException($"Transaction type not found: {transactionType}.")
                };
                break;
            case Product.GlobalEquities:
                transactionNo = await GenerateTransactionNo(_globalWalletDepositRepository, "GE", channel, transactionType, correlationId);
                break;
            case Product.Crypto:
            case Product.Funds:
            case Product.Unknown:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException(product.ToString());
        }

        return transactionNo;
    }

    private async Task<string> GetTransactionNoV2(Product product, Channel channel, TransactionType transactionType,
        Guid correlationId)
    {
        var transactionNoPrefix = GetTransactionNoPrefix(channel, product);
        switch (transactionType)
        {
            case TransactionType.Deposit:
                return await GenerateTransactionNo(_depositEntrypointRepository, transactionNoPrefix, channel, transactionType, correlationId);
            case TransactionType.Withdraw:
                return await GenerateTransactionNo(_withdrawEntrypointRepository, transactionNoPrefix, channel, transactionType, correlationId);
            case TransactionType.Transfer:
            case TransactionType.Refund:
            case TransactionType.Unknown:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException($"TransactionType {transactionType} not found.");
        }
    }

    private string GetTransactionNoPrefix(Channel channel, Product product)
    {
        string prefix;
        switch (product)
        {
            case Product.Cash:
            case Product.CashBalance:
            case Product.CreditBalance:
            case Product.CreditBalanceSbl:
            case Product.Derivatives:
                prefix = channel switch
                {
                    Channel.ATS => "AS",
                    Channel.ODD => "OD",
                    Channel.QR
                        or Channel.SetTrade
                        or Channel.OnlineViaKKP
                        or Channel.EForm
                        or Channel.TransferApp => "DH",
                    Channel.Unknown => throw new NotImplementedException(),
                    _ => throw new ArgumentOutOfRangeException($"Channel not found {channel}")
                };
                break;
            case Product.GlobalEquities:
                prefix = channel switch
                {
                    Channel.ATS => "GA",
                    Channel.ODD => "GO",
                    Channel.QR
                        or Channel.SetTrade
                        or Channel.OnlineViaKKP
                        or Channel.EForm
                        or Channel.TransferApp => "GE",
                    Channel.Unknown => throw new NotImplementedException(),
                    _ => throw new ArgumentOutOfRangeException($"Channel not found {channel}")
                };
                break;
            case Product.Crypto:
            case Product.Funds:
            case Product.Unknown:
                throw new NotImplementedException();
            default:
                throw new ArgumentOutOfRangeException($"Product not found {product}");
        }

        return prefix;
    }

    private async Task<string> GenerateTransactionNo<T>(T repository, string transactionNoPrefix, Channel channel, TransactionType transactionType, Guid transactionCorrelationId) where T : ITransactionRepository
    {
        var thDateTime = DateUtils.GetThDateTimeNow();
        var cutOffTime = TimeOnly.Parse(transactionNoPrefix switch
        {
            "DH" or "OD" or "AS" => _transactionNoCutOffTimeOptions.Value.NonGe,
            "GE" or "GO" or "GA" => _transactionNoCutOffTimeOptions.Value.Ge,
            _ => throw new ArgumentOutOfRangeException(transactionNoPrefix)
        });

        if (cutOffTime != TimeOnly.MinValue)
        {
            // if 00:00 < thDateTime < cutOffTime, then subtract 1 day
            if (TimeOnly.FromDateTime(thDateTime).IsBetween(TimeOnly.MinValue, cutOffTime))
            {
                thDateTime = thDateTime.AddDays(-1);
            }
        }

        var date = DateOnly.FromDateTime(thDateTime);

        int i;
        string transactionNo;
        do
        {
            i = 1;
            var count = (transactionPrefix: transactionNoPrefix, transactionType) switch
            {
                ("DH" or "OD" or "AS", TransactionType.Deposit) =>
                    await _depositRepository.CountTransactionNoByDate(date, cutOffTime, true, transactionNoPrefix, channel, transactionType) +
                    await _depositEntrypointRepository.CountTransactionNoByDate(date, cutOffTime, true, transactionNoPrefix, channel, transactionType),
                ("DH" or "OD" or "AS", TransactionType.Withdraw) =>
                    await _cashWithdrawRepository.CountTransactionNoByDate(date, cutOffTime, true, transactionNoPrefix, channel, transactionType) +
                    await _withdrawEntrypointRepository.CountTransactionNoByDate(date, cutOffTime, true, transactionNoPrefix, channel, transactionType),
                ("GE" or "GO" or "GA", TransactionType.Deposit) =>
                    await _globalWalletDepositRepository.CountTransactionNoByDate(date, cutOffTime, true, transactionNoPrefix, channel, transactionType) +
                    await _depositEntrypointRepository.CountTransactionNoByDate(date, cutOffTime, true, transactionNoPrefix, channel, transactionType),
                ("GE" or "GO" or "GA", TransactionType.Withdraw) =>
                    await _globalWalletDepositRepository.CountTransactionNoByDate(date, cutOffTime, true, transactionNoPrefix, channel, transactionType) +
                    await _withdrawEntrypointRepository.CountTransactionNoByDate(date, cutOffTime, true, transactionNoPrefix, channel, transactionType),
                _ => 0
            };
            var nextNo = count + 1;
            var prefix = transactionType == TransactionType.Deposit ? "DP" : "WS";
            transactionNo = $"{transactionNoPrefix}{prefix}{thDateTime:yyyyMMdd}{nextNo:00000}";
            try
            {
                await repository.UpdateTransactionNo(transactionCorrelationId, transactionNo);
            }
            catch (DuplicateTransactionNoException)
            {
                i = 0;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unable to Generate Deposit Global TransactionNo. Message: {Message}",
                    ex.Message);
                throw;
            }
        } while (i == 0);

        return transactionNo;
    }
}