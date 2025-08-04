using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pi.MarketData.Domain.Entities;
using Pi.MarketData.Domain.Models.Response;
using Pi.MarketData.MigrationProxy.API.Helpers;
using Pi.MarketData.MigrationProxy.API.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Pi.MarketData.MigrationProxy.API.Controllers;

[ApiController]
[Route("market-data-management")]
public class CuratedListController : ControllerBase
{
    private readonly ILogger<CuratedListController> _logger;
    private readonly IHttpRequestHelper _httpRequestHelper;
    private readonly HttpClient _setHttpClient;
    private readonly HttpClient _geHttpClient;
    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="httpRequestHelper"></param>
    public CuratedListController
    (
        ILogger<CuratedListController> logger,
        IHttpClientFactory httpClientFactory,
        IHttpRequestHelper httpRequestHelper
    )
    {
        _logger = logger;
        _setHttpClient = httpClientFactory.CreateClient("SETClient");
        _geHttpClient = httpClientFactory.CreateClient("GEClient");
        _httpRequestHelper = httpRequestHelper;
    }

    [HttpGet("curated-lists", Name = "GetAllCuratedList")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [SwaggerIgnore]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> GetCuratedList()
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

    [HttpPost("set/curated-lists", Name = "PostSetCuratedList")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> PostSetCuratedList(IFormFile file)
    {
        var response = await _httpRequestHelper.UploadFile(
            _setHttpClient,
            "/market-data-management/curated-lists",
            file
        );

        var wrapper = new ResponseWrapper
        {
            Set = HttpResponseHelper.ParseResponseContent(await response.Content.ReadAsStringAsync())
        };

        return Content(JsonConvert.SerializeObject(wrapper), "application/json");
    }

    [HttpPost("ge/curated-lists", Name = "PostGeCuratedList")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> PostGeCuratedList(IFormFile file)
    {
        var response = await _httpRequestHelper.UploadFile(
            _geHttpClient,
            "/market-data-management/curated-lists",
            file
        );

        var wrapper = new ResponseWrapper
        {
            Ge = HttpResponseHelper.ParseResponseContent(await response.Content.ReadAsStringAsync())
        };

        return Content(JsonConvert.SerializeObject(wrapper), "application/json");
    }

    [HttpPatch("set/curated-lists/{id}", Name = "UpdateSetCuratedList")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [SwaggerIgnore]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> UpdateSetCuratedList([FromRoute] string id)
    {
        string requestBody = await new StreamReader(Request.Body, encoding: Encoding.UTF8).ReadToEndAsync();
        var response = await _httpRequestHelper.RequestByUrl(
            _setHttpClient,
            $"/market-data-management/curated-lists/{id}",
            requestBody,
            HttpMethod.Patch
        );

        var wrapper = new ResponseWrapper
        {
            Set = HttpResponseHelper.ParseResponseContent(await response.Content.ReadAsStringAsync())
        };

        return Content(JsonConvert.SerializeObject(wrapper), "application/json");
    }

    [HttpPatch("ge/curated-lists/{id}", Name = "UpdateGeCuratedList")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [SwaggerIgnore]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> UpdateGeCuratedList([FromRoute] string id)
    {
        string requestBody = await new StreamReader(Request.Body, encoding: Encoding.UTF8).ReadToEndAsync();
        var response = await _httpRequestHelper.RequestByUrl(
            _geHttpClient,
            $"/market-data-management/curated-lists/{id}",
            requestBody,
            HttpMethod.Patch
        );

        var wrapper = new ResponseWrapper
        {
            Ge = HttpResponseHelper.ParseResponseContent(await response.Content.ReadAsStringAsync())
        };

        return Content(JsonConvert.SerializeObject(wrapper), "application/json");
    }

    [HttpDelete("set/curated-lists/{id}", Name = "DeleteSetCuratedList")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [SwaggerIgnore]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> DeleteSetCuratedList([FromRoute] string id)
    {
        var response = await _httpRequestHelper.RequestByUrl(
            _setHttpClient,
            $"/market-data-management/curated-lists/{id}",
            null,
            HttpMethod.Delete
        );

        var wrapper = new ResponseWrapper
        {
            Set = HttpResponseHelper.ParseResponseContent(await response.Content.ReadAsStringAsync())
        };

        return Content(JsonConvert.SerializeObject(wrapper), "application/json");
    }

    [HttpDelete("ge/curated-lists/{id}", Name = "DeleteGeCuratedList")]
    [SwaggerOperation(Tags = ["MarketDataManagement"])]
    [SwaggerIgnore]
    [ResponseCache(CacheProfileName = "Default5Mins")]
    public async Task<IActionResult> DeleteGeCuratedList([FromRoute] string id)
    {
        var response = await _httpRequestHelper.RequestByUrl(
            _geHttpClient,
            $"/market-data-management/curated-lists/{id}",
            null,
            HttpMethod.Delete
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
            $"/market-data-management/curated-lists{query}",
            null,
            HttpMethod.Get
        );
        return await response.Content.ReadAsStringAsync();
    }
}