using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Logging;
using Pi.TfexService.Application.Services.SetTrade;
using Pi.TfexService.Domain.Models;
using Pi.TfexService.Domain.Models.ActivitiesLog;

namespace Pi.TfexService.Application.Commands;

public record LogActivitiesRequest(
    string UserId,
    string CustomerCode,
    string AccountCode,
    RequestType RequestType,
    string? RequestBodyJson,
    string? OrderNo,
    string? ResponseBodyJson,
    DateTime? RequestedAt,
    DateTime? CompletedAt,
    bool IsSuccess,
    string? FailedReason,
    SetTradeOrder? MappedResponseOrder);

public class LogActivitiesConsumer(
    IActivitiesLogRepository activitiesLogRepository,
    ILogger<LogActivitiesConsumer> logger)
    : IConsumer<LogActivitiesRequest>
{
    public async Task Consume(ConsumeContext<LogActivitiesRequest> context)
    {
        if (string.IsNullOrEmpty(context.Message.RequestBodyJson)) return;

        try
        {
            await activitiesLogRepository.AddAsync(
                new ActivitiesLog(
                    Guid.NewGuid(),
                    context.Message.UserId,
                    context.Message.CustomerCode,
                    context.Message.AccountCode,
                    context.Message.RequestType,
                    context.Message.RequestBodyJson,
                    context.Message.OrderNo,
                    context.Message.ResponseBodyJson,
                    context.Message.RequestedAt,
                    context.Message.CompletedAt,
                    context.Message.IsSuccess,
                    context.Message.FailedReason,
                    context.Message.MappedResponseOrder?.Symbol,
                    context.Message.MappedResponseOrder?.Side.ToString(),
                    context.Message.MappedResponseOrder?.PriceType.ToString(),
                    context.Message.MappedResponseOrder?.Price,
                    context.Message.MappedResponseOrder?.Qty,
                    context.Message.MappedResponseOrder?.RejectCode.ToString(),
                    context.Message.MappedResponseOrder?.RejectReason
                ), context.CancellationToken);
            await activitiesLogRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unable to log activity with Exception: {Message}", ex.Message);
        }
    }
}