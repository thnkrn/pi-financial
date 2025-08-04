using Pi.Common.ExtensionMethods;
using Pi.GlobalEquities.Errors;

namespace Pi.GlobalEquities.Application.Exceptions;

public sealed class GeException : Exception
{
    public override string Message { get; }
    public Error Error { get; }

    public GeException(Error error, params object?[]? ps)
    {
        Error = error;
        Data["Code"] = error.Code.ToString();
        Message = ps != null ? string.Format(error.Code.ToString(), ps) : error.Code.ToString();
    }
}
