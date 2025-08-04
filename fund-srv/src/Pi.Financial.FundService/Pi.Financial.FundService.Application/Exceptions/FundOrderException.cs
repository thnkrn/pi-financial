using Pi.Common.ExtensionMethods;
using Pi.Financial.FundService.Domain.AggregatesModel.TradingAggregate;

namespace Pi.Financial.FundService.Application.Exceptions;

public sealed class FundOrderException : Exception
{
    public FundOrderErrorCode Code { get; set; }

    public FundOrderException(FundOrderErrorCode code) : base(code.GetEnumDescription())
    {
        Code = code;
        Data["Code"] = code.ToString();
    }
}
