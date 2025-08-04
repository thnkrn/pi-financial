using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pi.MarketData.Application.Services.MarketDataManagement;
using Pi.MarketData.Domain.Models.Response;
using Pi.MarketData.MigrationProxy.API.Helpers;
using Pi.MarketData.MigrationProxy.API.Interfaces;
using Pi.SetMarketData.Application.Queries;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi.MarketData.MigrationProxy.API.Controllers;

[ApiController]
[Route("market-data-management")]

public class CuratedFilterController : ControllerBase
{
    private readonly ILogger<CuratedFilterController> _logger;
    private readonly IHttpRequestHelper _httpRequestHelper;
    private readonly HttpClient _setHttpClient;
    private readonly HttpClient _geHttpClient;
    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="httpRequestHelper"></param>
    public CuratedFilterController
    (
        ILogger<CuratedFilterController> logger,
        IHttpClientFactory httpClientFactory,
        IHttpRequestHelper httpRequestHelper
    )
    {
        _logger = logger;
        _setHttpClient = httpClientFactory.CreateClient("SETClient");
        _geHttpClient = httpClientFactory.CreateClient("GEClient");
        _httpRequestHelper = httpRequestHelper;
    }

    [HttpGet("curated-filters", Name = "GetAllCuratedFilter")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [SwaggerIgnore]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> GetCuratedFilter()
    {
        var query = HttpContext.Request.QueryString.Value;
        var setResponse = GetResponseContent(_setHttpClient, query);
        var geResponse = GetResponseContent(_geHttpClient, query);

        await Task.WhenAll(setResponse, geResponse);
        var result = CuratedFilterResponse.GetResult([await setResponse, await geResponse]);

        return Content(JsonConvert.SerializeObject(result), "application/json");
    }

    [HttpPost("set/curated-filters", Name = "PostSetCuratedFilter")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> PostSetCuratedFilter(IFormFile file)
    {
        var response = await _httpRequestHelper.UploadFile(
            _setHttpClient,
            "/market-data-management/curated-filters",
            file
        );

        var wrapper = new ResponseWrapper
        {
            Set = HttpResponseHelper.ParseResponseContent(await response.Content.ReadAsStringAsync())
        };

        return Content(JsonConvert.SerializeObject(wrapper), "application/json");
    }

    [HttpPost("ge/curated-filters", Name = "PostGeCuratedFilter")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> PostGeCuratedFilter(IFormFile file)
    {
        var response = await _httpRequestHelper.UploadFile(
            _geHttpClient,
            "/market-data-management/curated-filters",
            file
        );

        var wrapper = new ResponseWrapper
        {
            Ge = HttpResponseHelper.ParseResponseContent(await response.Content.ReadAsStringAsync())
        };

        return Content(JsonConvert.SerializeObject(wrapper), "application/json");
    }

    private async Task<GetCuratedFilterResponse?> GetResponseContent(HttpClient client, string? query)
    {
        var response = await _httpRequestHelper.RequestByUrl(
            client,
            $"/market-data-management/curated-filters{query}",
            null,
            HttpMethod.Get
        );
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var filterResponse = JsonConvert.DeserializeObject<GetCuratedFilterResponse>(jsonResponse);
        return filterResponse;
    }
}