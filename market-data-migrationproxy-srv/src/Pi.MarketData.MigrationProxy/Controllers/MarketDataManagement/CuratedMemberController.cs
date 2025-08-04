using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pi.MarketData.Domain.Models.Response;
using Pi.MarketData.MigrationProxy.API.Helpers;
using Pi.MarketData.MigrationProxy.API.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi.MarketData.MigrationProxy.API.Controllers;

[ApiController]
[Route("market-data-management")]
public class CuratedMemberController : ControllerBase
{
    private readonly ILogger<CuratedMemberController> _logger;
    private readonly IHttpRequestHelper _httpRequestHelper;
    private readonly HttpClient _setHttpClient;
    private readonly HttpClient _geHttpClient;
    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="httpRequestHelper"></param>
    public CuratedMemberController
    (
        ILogger<CuratedMemberController> logger,
        IHttpClientFactory httpClientFactory,
        IHttpRequestHelper httpRequestHelper
    )
    {
        _logger = logger;
        _setHttpClient = httpClientFactory.CreateClient("SETClient");
        _geHttpClient = httpClientFactory.CreateClient("GEClient");
        _httpRequestHelper = httpRequestHelper;
    }

    [HttpGet("curated-members", Name = "GetAllCuratedMember")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [SwaggerIgnore]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> GetCuratedMember()
    {
        var query = HttpContext.Request.QueryString.Value;
        var setResponse = GetResponseContent(_setHttpClient, query);
        var geResponse = GetResponseContent(_geHttpClient, query);

        await Task.WhenAll(setResponse, geResponse);

        var wrapper = new ResponseWrapper
        {
            Set = HttpResponseHelper.ParseResponseContent(await setResponse),
            Ge = HttpResponseHelper.ParseResponseContent(await geResponse)
        };

        return Content(JsonConvert.SerializeObject(wrapper), "application/json");
    }

    [HttpPost("set/curated-members", Name = "PostSetCuratedMember")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> PostSetCuratedMember(IFormFile file)
    {
        var response = await _httpRequestHelper.UploadFile(
            _setHttpClient,
            "/market-data-management/curated-members",
            file
        );

        var wrapper = new ResponseWrapper
        {
            Set = HttpResponseHelper.ParseResponseContent(await response.Content.ReadAsStringAsync())
        };

        return Content(JsonConvert.SerializeObject(wrapper), "application/json");
    }

    [HttpPost("ge/curated-members", Name = "PostGeCuratedMember")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> PostGeCuratedMember(IFormFile file)
    {
        var response = await _httpRequestHelper.UploadFile(
            _geHttpClient,
            "/market-data-management/curated-members",
            file
        );

        var wrapper = new ResponseWrapper
        {
            Ge = HttpResponseHelper.ParseResponseContent(await response.Content.ReadAsStringAsync())
        };

        return Content(JsonConvert.SerializeObject(wrapper), "application/json");
    }

    private async Task<string> GetResponseContent(HttpClient client, string? query)
    {
        var response = await _httpRequestHelper.RequestByUrl(
            client,
            $"/market-data-management/curated-members{query}",
            null,
            HttpMethod.Get
        );
        return await response.Content.ReadAsStringAsync();
    }
}