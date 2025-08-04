using MassTransit;
using Pi.SetService.Application.Exceptions;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;
using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
using Pi.SetService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.SetService.Application.Commands;

public record ReviewSblOrder
{
    public required Guid Id { get; init; }
    public required SblOrderStatus Status { get; init; }
    public required Guid ReviewerId { get; init; }
    public string? RejectedReason { get; init; }
}

public record ReviewSblOrderResponse
{
    public required Guid Id { get; init; }
    public required Guid TradingAccountId { get; init; }
    public required string TradingAccountNo { get; init; }
    public required string CustomerCode { get; init; }
    public required ulong OrderId { get; init; }
    public required string Symbol { get; init; }
    public required SblOrderType Type { get; init; }
    public required int Volume { get; init; }
    public required SblOrderStatus Status { get; init; }
    public string? RejectedReason { get; init; }
    public Guid? ReviewerId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

public class ReviewSblOrderConsumer(ISblOrderRepository sblOrderRepository) : IConsumer<ReviewSblOrder>
{
    public async Task Consume(ConsumeContext<ReviewSblOrder> context)
    {
        if (context.Message.Status == SblOrderStatus.Pending)
        {
            throw new SetException(SetErrorCode.SE003);
        }

        var order = await sblOrderRepository.GetSblOrder(context.Message.Id, context.CancellationToken);

        if (order is not { Status: SblOrderStatus.Pending })
        {
            throw new SetException(SetErrorCode.SE116);
        }

        switch (context.Message.Status)
        {
            case SblOrderStatus.Approved:
                order.Approve(context.Message.ReviewerId);
                break;
            case SblOrderStatus.Rejected:
                order.Reject(context.Message.ReviewerId, context.Message.RejectedReason);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(context.Message.Status), context.Message.Status,
                    string.Empty);
        }

        sblOrderRepository.Update(order);
        await sblOrderRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);

        if (context.ResponseAddress != null)
        {
            await context.RespondAsync(new ReviewSblOrderResponse
            {
                Id = order.Id,
                TradingAccountId = order.TradingAccountId,
                TradingAccountNo = order.TradingAccountNo,
                CustomerCode = order.CustomerCode,
                OrderId = order.OrderId,
                Symbol = order.Symbol,
                Type = order.Type,
                Volume = order.Volume,
                Status = order.Status,
                RejectedReason = order.RejectedReason,
                ReviewerId = order.ReviewerId,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            });
        }
    }
}
