using Microsoft.AspNetCore.Mvc;
using Pi.SetMarketData.MigrationProxy.Helpers;

namespace Pi.SetMarketData.MigrationProxy.Controllers;

[ApiController]
[Route("/")]
public class ProxyController : ControllerBase
{
    private readonly ILogger<ProxyController> _logger;
    private readonly HttpClient _httpClient;
    private readonly HttpRequestHelper _httpRequestHelper;

    public ProxyController
    (
        ILogger<ProxyController> logger,
        IHttpClientFactory httpClientFactory
    )
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("SiriusClient");
        _httpRequestHelper = new HttpRequestHelper();
    }

    [Route("{*path}")]
    [HttpGet,HttpPost,HttpPatch,HttpPut,HttpDelete,HttpHead,HttpOptions]
    public async Task<IActionResult> HandleRequest(string path)
    {
        try
        {
            HttpResponseMessage? response = await _httpRequestHelper.Request
            (
                _httpClient,
                HttpContext
            );
    
            if (response != null)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Received response - Status: {StatusCode}, Content Length: {ContentLength}", 
                    response.StatusCode, 
                    content.Length
                );
    
                var result = new ContentResult
                {
                    Content = content,
                    ContentType = response.Content.Headers.ContentType?.ToString(),
                    StatusCode = (int)response.StatusCode
                };
    
                foreach (var header in response.Headers)
                {
                    Response.Headers[header.Key] = header.Value.ToArray();
                }
    
                return result;
            }
            else
            {
                _logger.LogWarning("Received null response from RequestByUrl");
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while handling request");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}