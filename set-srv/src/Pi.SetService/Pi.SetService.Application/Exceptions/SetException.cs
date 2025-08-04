using Pi.Common.ExtensionMethods;
using Pi.SetService.Domain.AggregatesModel.ErrorAggregate;

namespace Pi.SetService.Application.Exceptions;

public sealed class SetException : Exception
{
    public override string Message { get; }
    public SetErrorCode Code { get; }

    public SetException(SetErrorCode code, params object?[]? ps)
    {
        Code = code;
        Data["Code"] = code.ToString();
        Message = ps != null ? string.Format(code.GetEnumDescription(), ps) : code.GetEnumDescription();
    }
}
