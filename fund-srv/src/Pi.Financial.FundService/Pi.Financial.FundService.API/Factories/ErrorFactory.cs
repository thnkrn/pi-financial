using MassTransit;
using Pi.Financial.FundService.Application.Exceptions;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.API.Factories;

public static class ErrorFactory
{
    public static FundOrderErrorCode? NewErrorCode(RequestFaultException exception)
    {
        var fundOrderException =
            exception.Fault?.Exceptions.FirstOrDefault(e =>
                e.ExceptionType.Equals(typeof(FundOrderException).ToString()));
        var errorCodeStr = fundOrderException?.Data?["Code"];
        if (errorCodeStr == null || !Enum.TryParse((string)errorCodeStr, out FundOrderErrorCode errorCode))
        {
            return null;
        }

        return errorCode;
    }
}
