using Pi.GlobalMarketData.API.Infrastructure.Exceptions;

namespace Pi.GlobalMarketData.API.Infrastructure.Mappers;

public static class MapRfcUrlToExceptionType
{
    public static string Map(string rfcUrl)
    {
        return rfcUrl switch
        {
            // 400 Bad Request
            "https://tools.ietf.org/html/rfc9110#section-15.5.1" => nameof(BadRequestException),
            // 401 Unauthorized
            "https://tools.ietf.org/html/rfc9110#section-15.5.2" => nameof(UnauthorizedErrorException),
            // 403 Forbidden
            "https://tools.ietf.org/html/rfc9110#section-15.5.3" => nameof(UnauthorizedAccessException),
            // 404 Not Found
            "https://tools.ietf.org/html/rfc9110#section-15.5.4" => nameof(NotFoundException),
            // 405 Method Not Allowed
            "https://tools.ietf.org/html/rfc9110#section-15.5.5" => nameof(NotSupportedException),
            // 406 Not Acceptable
            "https://tools.ietf.org/html/rfc9110#section-15.5.6" => nameof(NotSupportedException),
            // 407 Proxy Authentication Required
            "https://tools.ietf.org/html/rfc9110#section-15.5.7" => nameof(UnauthorizedAccessException),
            // 408 Request Timeout
            "https://tools.ietf.org/html/rfc9110#section-15.5.8" => nameof(TimeoutException),
            // 409 Conflict
            "https://tools.ietf.org/html/rfc9110#section-15.5.9" => nameof(InvalidOperationException),
            // 410 Gone
            "https://tools.ietf.org/html/rfc9110#section-15.5.10" => nameof(FileNotFoundException),
            // 411 Length Required
            "https://tools.ietf.org/html/rfc9110#section-15.5.11" => nameof(ArgumentException),
            // 412 Precondition Failed
            "https://tools.ietf.org/html/rfc9110#section-15.5.12" => nameof(InvalidOperationException),
            // 413 Payload Too Large
            "https://tools.ietf.org/html/rfc9110#section-15.5.13" => nameof(OutOfMemoryException),
            // 414 URI Too Long
            "https://tools.ietf.org/html/rfc9110#section-15.5.14" => nameof(UriFormatException),
            // 415 Unsupported Media Type
            "https://tools.ietf.org/html/rfc9110#section-15.5.15" => nameof(NotSupportedException),
            // 416 Range Not Satisfiable
            "https://tools.ietf.org/html/rfc9110#section-15.5.16" => nameof(ArgumentOutOfRangeException),
            // 417 Expectation Failed
            "https://tools.ietf.org/html/rfc9110#section-15.5.17" => nameof(InvalidOperationException),
            // 421 Misdirected Request
            "https://tools.ietf.org/html/rfc9110#section-15.5.21" => nameof(HttpRequestException),
            // 500 Internal Server Error
            "https://tools.ietf.org/html/rfc9110#section-15.6.1" => nameof(InternalServerErrorException),
            // 501 Not Implemented
            "https://tools.ietf.org/html/rfc9110#section-15.6.2" => nameof(NotImplementedException),
            // 502 Bad Gateway
            "https://tools.ietf.org/html/rfc9110#section-15.6.3" => nameof(HttpRequestException),
            // 503 Service Unavailable
            "https://tools.ietf.org/html/rfc9110#section-15.6.4" => nameof(HttpRequestException),
            // 504 Gateway Timeout
            "https://tools.ietf.org/html/rfc9110#section-15.6.5" => nameof(TimeoutException),
            _ => nameof(Exception)
        };
    }
}