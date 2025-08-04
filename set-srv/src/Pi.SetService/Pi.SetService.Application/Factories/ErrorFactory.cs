using Pi.SetService.Application.Exceptions;
using Pi.SetService.Application.Models;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;

namespace Pi.SetService.Application.Factories;

public static class ErrorFactory
{
    public static SetException NewSetException(BrokerOrderResponse brokerResponse)
    {
        if (brokerResponse.Reason == null)
        {
            return new SetException(SetErrorCode.SE201);
        }

        return brokerResponse.Reason switch
        {
            var reason when reason.Contains("P030") => new SetException(SetErrorCode.SE203),
            var reason when reason.Contains("P035") => new SetException(SetErrorCode.SE204),
            var reason when reason.Contains("P040") => new SetException(SetErrorCode.SE205),
            var reason when reason.Contains("P056") => new SetException(SetErrorCode.SE206),
            var reason when reason.Contains("P112") => new SetException(SetErrorCode.SE207),
            var reason when reason.Contains("P133") => new SetException(SetErrorCode.SE208),
            _ => new SetException(SetErrorCode.SE202, brokerResponse.Reason)
        };
    }
}
