using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Pi.SetService.Infrastructure.Handlers;

public class HttpMessageLoggingHandler(ILogger<HttpMessageLoggingHandler> logger) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var logRequest = new
        {
            Method = request.Method.ToString(),
            Url = request.RequestUri?.ToString(),
            Headers = request.Headers,
            Body = request.Content != null ? await request.Content.ReadAsStringAsync(cancellationToken) : null
        };

        var response = await base.SendAsync(request, cancellationToken);

        var logResponse = new
        {
            StatusCode = response.StatusCode,
            Headers = response.Headers,
            Body = await response.Content.ReadAsStringAsync(cancellationToken)
        };


        using (LogContext.PushProperty("ExternalRequest", true))
        {
            logger.LogInformation("Request: {@Request} with Response: {@Response}", logRequest, logResponse);
        }

        return response;
    }
}
