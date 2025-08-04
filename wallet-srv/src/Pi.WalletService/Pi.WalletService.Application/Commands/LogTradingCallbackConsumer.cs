using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.WalletService.Domain.AggregatesModel.LogAggregate;
namespace Pi.WalletService.Application.Commands;

public record LogTradingCallback(string TransId, FreewillRequestType RequestTypeType, string Callback);

public class LogTradingCallbackConsumer : IConsumer<LogTradingCallback>
{
    private readonly IFreewillRequestLogRepository _freewillRequestLogRepository;
    private readonly ILogger<LogTradingCallbackConsumer> _logger;

    public LogTradingCallbackConsumer(IFreewillRequestLogRepository freewillRequestLogRepository, ILogger<LogTradingCallbackConsumer> logger)
    {
        _freewillRequestLogRepository = freewillRequestLogRepository;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<LogTradingCallback> context)
    {
        try
        {
            var log = await _freewillRequestLogRepository.Get(context.Message.TransId, context.Message.RequestTypeType);

            if (log == null)
            {
                throw new ArgumentException($"Unable to find log for TransId {context.Message.TransId} and RequestType {context.Message.RequestTypeType}");
            }

            log.SetCallback(context.Message.Callback);

            await _freewillRequestLogRepository.UnitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogFreewillRequest: Unable to log freewill request with Exception: {Message}", ex.Message);
        }
    }
}