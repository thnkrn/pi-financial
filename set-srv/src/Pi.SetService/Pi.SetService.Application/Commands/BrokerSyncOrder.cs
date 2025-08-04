// using MassTransit;
// using Microsoft.Extensions.Logging;
// using Pi.OnePort.IntegrationEvents;
// using Pi.SetService.Application.Factories;
// using Pi.SetService.Application.Services.OnboardService;
// using Pi.SetService.Application.Utils;
// using Pi.SetService.Domain.AggregatesModel.FinancialAssetAggregate;
// using Pi.SetService.Domain.AggregatesModel.TradingAggregate;
// using Pi.SetService.Domain.Events;
//
// namespace Pi.SetService.Application.Commands;
//
// public class BrokerOrderConsumer(
//     IOnboardService onboardService,
//     IEquityOrderStateRepository equityOrderStateRepository,
//     ILogger<BrokerOrderConsumer> logger) : IConsumer<OnePortBrokerOrderCreated>
// {
//     public async Task Consume(ConsumeContext<OnePortBrokerOrderCreated> context)
//     {
//         var custCode = TradingAccountHelper.GetCustCodeBySetAccountNo(context.Message.AccountId);
//
//         var tradingAccounts =
//             await onboardService.GetSetTradingAccountsByCustCodeAsync(custCode, context.CancellationToken);
//         var tradingAccount = tradingAccounts.Find(q => q.AccountNo == context.Message.AccountId);
//
//         if (tradingAccount == null)
//         {
//             logger.LogError("{EventName} but trading account not found for cust code: {CustCode}", nameof(context.Message), custCode);
//             return;
//         }
//
//         var filters = new EquityOrderStateFilters
//         {
//             BrokerOrderId = context.Message.FisOrderId,
//             CreatedDate = DateOnly.FromDateTime(context.Message.TransactionDateTime)
//         };
//         var orderStates = await equityOrderStateRepository.GetEquityOrderStates(filters);
//         var orderState = orderStates.FirstOrDefault(q => q.BrokerOrderId == context.Message.FisOrderId);
//
//         // TODO: Remove sync from Change, Cancel consumers or exclude this sync when it's already exist
//         if (orderState != null)
//         {
//             return;
//         }
//
//         try
//         {
//             var orderType = IntegrationEventFactory.NewOrderType(context.Message.Type);
//             var orderSide = IntegrationEventFactory.NewSide(context.Message.Side);
//             var orderAction = DomainFactory.NewOrderAction(orderSide, orderType);
//
//             var msg = new SyncCreateOrderReceived
//             {
//                 CorrelationId = Guid.NewGuid(),
//                 TradingAccountId = tradingAccount.Id,
//                 TradingAccountNo = tradingAccount.AccountNo,
//                 TradingAccountType = tradingAccount.TradingAccountType,
//                 CustomerCode = tradingAccount.CustomerCode,
//                 EnterId = context.Message.EnterId,
//                 BrokerOrderId = context.Message.FisOrderId,
//                 ConditionPrice = IntegrationEventFactory.NewConditionPrice(context.Message.ConPrice),
//                 OrderStatus = OrderStatus.Pending,
//                 SecSymbol = context.Message.Symbol,
//                 Volume = decimal.ToInt32(context.Message.Volume),
//                 PubVolume = decimal.ToInt32(context.Message.PubVolume),
//                 OrderSide = orderSide,
//                 OrderAction = orderAction,
//                 Condition = IntegrationEventFactory.NewCondition(context.Message.Condition),
//                 OrderNo = context.Message.RefOrderId,
//                 Price = context.Message.Price,
//                 MatchedVolume = null,
//                 OrderType = orderType,
//                 Ttf = IntegrationEventFactory.NewTtf(context.Message.Ttf),
//                 FailedReason = null,
//                 OrderDateTime = context.Message.TransactionDateTime
//             };
//             await context.Publish(msg);
//         }
//         catch (ArgumentOutOfRangeException e)
//         {
//             logger.LogError(e, "Broker order consume got error: {Error}", e.Message);
//         }
//     }
// }
