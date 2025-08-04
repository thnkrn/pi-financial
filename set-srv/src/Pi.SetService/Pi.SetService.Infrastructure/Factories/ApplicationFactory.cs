using Pi.Client.OnePort.GW.TCP.Model;
using Pi.SetService.Application.Models;
using static Pi.Client.OnePort.GW.TCP.Model.PiOnePortTCPModelsPacketsDataTransferDataTransferOrderAcknowledgementResponse7K;

namespace Pi.SetService.Infrastructure.Factories;

public static class ApplicationFactory
{
    public static BrokerOrderResponse NewBrokerOrderResponse(PiOnePortTCPModelsPacketsDataTransferDataTransferOrderAcknowledgementResponse7K response7K)
    {
        return new BrokerOrderResponse
        {
            OrderNo = response7K.RefOrderId,
            BrokerOrderId = response7K.FisOrderId.TrimStart('0'),
            Reason = response7K.Reason,
            Status = response7K.OrderStatus != null ? EntityFactory.NewBrokerOrderStatus((OrderStatusEnum)response7K.OrderStatus) : null,
            Source = response7K.Source != null ? EntityFactory.NewSource((SourceEnum)response7K.Source) : null,
            ExecutionTransType = response7K.ExecutionTransType != null ? EntityFactory.NewExecutionTransType((ExecutionTransTypeEnum)response7K.ExecutionTransType) : null,
            ExecutionTransRejectType = response7K.ExecutionTransRejectType != null ? EntityFactory.NewExecutionTransRejectType(response7K.ExecutionTransRejectType) : null
        };
    }

    public static BrokerOrderResponse NewBrokerOrderResponse(PiOnePortTCPModelsPacketsDataTransferDataTransferOrderChangeResponse7N response7K)
    {
        return new BrokerOrderResponse
        {
            OrderNo = response7K.RefOrderId,
            BrokerOrderId = response7K.FisOrderId.TrimStart('0'),
            Reason = response7K.Reason,
            Status = response7K.OrderStatus != null ? EntityFactory.NewBrokerOrderStatus((OrderStatusEnum)response7K.OrderStatus) : null,
            Source = response7K.Source != null ? EntityFactory.NewSource((SourceEnum)response7K.Source) : null,
            ExecutionTransType = response7K.ExecutionTransType != null ? EntityFactory.NewExecutionTransType((ExecutionTransTypeEnum)response7K.ExecutionTransType) : null,
            ExecutionTransRejectType = response7K.ExecutionTransRejectType != null ? EntityFactory.NewExecutionTransRejectType((ExecutionTransRejectTypeEnum)response7K.ExecutionTransRejectType) : null
        };
    }
}
