using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.BackofficeService.Application.Models;
using Pi.BackofficeService.Application.Models.CuratedManager;
using Pi.BackofficeService.Application.Services.CuratedManagerService;
using Pi.BackofficeService.Infrastructure.CuratedManager;
using Pi.BackofficeService.Infrastructure.Models.CuratedManager;

namespace Pi.BackofficeService.Infrastructure.Services
{
    public class CuratedManagerService : ICuratedManagerService
    {
        private readonly ILogger<CuratedManagerService> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public CuratedManagerService(HttpClient httpClient, string baseUrl, ILogger<CuratedManagerService> logger)
        {
            _logger = logger;
            _httpClient = httpClient;
            _baseUrl = baseUrl;
        }

        private static CuratedListResponse TransformCuratedListWithIdString(CuratedListThirdPartyApiResponse response)
        {
            var combinedData = new List<TransformedCuratedListItem>();

            if (response.Set?.Data != null)
            {
                combinedData.AddRange(response.Set.Data.Select(item => new TransformedCuratedListItem(
                    id: item.IdString,
                    curatedListId: item.CuratedListId,
                    curatedListCode: item.CuratedListCode,
                    curatedType: item.CuratedType,
                    relevantTo: item.RelevantTo,
                    name: item.Name,
                    hashtag: item.Hashtag,
                    ordering: item.Ordering,
                    curatedListSource: CuratedListSource.SET,
                    createTime: item.CreateTime,
                    updateTime: item.UpdateTime,
                    updateBy: item.UpdateBy
                )));
            }

            if (response.Ge?.Data != null)
            {
                combinedData.AddRange(response.Ge.Data.Select(item => new TransformedCuratedListItem(
                    id: item.IdString,
                    curatedListId: item.CuratedListId,
                    curatedListCode: item.CuratedListCode,
                    curatedType: item.CuratedType,
                    relevantTo: item.RelevantTo,
                    name: item.Name,
                    hashtag: item.Hashtag,
                    ordering: item.Ordering,
                    curatedListSource: CuratedListSource.GE,
                    createTime: item.CreateTime,
                    updateTime: item.UpdateTime,
                    updateBy: item.UpdateBy
                )));
            }

            var logicalList = combinedData.Where(item => item.CuratedType == "Logical").ToList();
            var manualList = combinedData.Where(item => item.CuratedType == "Manual").ToList();

            return new CuratedListResponse(logicalList, manualList);
        }

        private static List<TransformedCuratedMemberItem> CombineCuratedMembers(List<CuratedMemberItem> setMembers, List<CuratedMemberItem> geMembers)
        {
            var combinedMembers = new List<TransformedCuratedMemberItem>();

            combinedMembers.AddRange(setMembers.Select(member => new TransformedCuratedMemberItem(
                symbol: member.Symbol,
                friendlyName: member.FriendlyName,
                logo: member.Logo,
                figi: member.Figi,
                units: member.Units,
                exchange: member.Exchange,
                dataVendorCode: member.DataVendorCode,
                dataVendorCode2: member.DataVendorCode2,
                source: CuratedListSource.SET,
                id: $"{member.Symbol}-{member.Exchange}-{CuratedListSource.SET}".GetHashCode().ToString("X")
            )));

            combinedMembers.AddRange(geMembers.Select(member => new TransformedCuratedMemberItem(
                symbol: member.Symbol,
                friendlyName: member.FriendlyName,
                logo: member.Logo,
                figi: member.Figi,
                units: member.Units,
                exchange: member.Exchange,
                dataVendorCode: member.DataVendorCode,
                dataVendorCode2: member.DataVendorCode2,
                source: CuratedListSource.GE,
                id: $"{member.Symbol}-{member.Exchange}-{CuratedListSource.GE}".GetHashCode().ToString("X")
            )));

            return combinedMembers;
        }

        private static List<CuratedFilterGroup> TransformCuratedFilters(CuratedFiltersThirdPartyApiResponse response)
        {
            if (response?.Data == null)
            {
                return [];
            }

            var curatedFilters = new List<CuratedFilterGroup>();

            foreach (var filterData in response.Data)
            {

                curatedFilters.Add(new CuratedFilterGroup(
                    name: filterData.SubGroupName,
                    data: [.. filterData.CuratedFilterList.OrderBy(f => f.Ordering)]
                ));
            }

            return curatedFilters;
        }

        public async Task<CuratedListResponse> GetCuratedList(string authToken)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/marketdata-migrationproxy/market-data-management/curated-lists");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var message = await response.Content.ReadAsStringAsync();
                var curatedListResponse = JsonConvert.DeserializeObject<CuratedListThirdPartyApiResponse>(message);
                if (curatedListResponse == null || curatedListResponse.Set == null || curatedListResponse.Ge == null)
                {
                    _logger.LogWarning("Curated list response is null");
                    return new CuratedListResponse([], []);
                }

                var transformedResponse = TransformCuratedListWithIdString(curatedListResponse);
                return transformedResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error - calling GetCuratedList in CuratedManagerService and mapping to DTO");
                throw;
            }
        }

        public async Task<CuratedListResponse> UploadCuratedManualList(string authToken, IFormFile file, string dataSource)
        {
            try
            {

                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

                var multipartContent = new MultipartFormDataContent
                {
                    { fileContent, "file", file.FileName },
                };

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/marketdata-migrationproxy/market-data-management/{dataSource.ToLower()}/curated-lists")
                {
                    Content = multipartContent
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var message = await response.Content.ReadAsStringAsync();
                var curatedListResponse = JsonConvert.DeserializeObject<CuratedListThirdPartyApiResponse>(message);
                if (curatedListResponse == null || curatedListResponse.Set == null || curatedListResponse.Ge == null)
                {
                    _logger.LogWarning("Curated list response is null");
                    return new CuratedListResponse([], []);
                }

                return TransformCuratedListWithIdString(curatedListResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error - calling UpLoadManualCuratedListCSV in CuratedManagerService and mapping to DTO");
                throw;
            }
        }

        public async Task<CuratedListItem> UpdateCuratedListById(string authToken, string id, string? name, string? hashtag, string dataSource)
        {
            try
            {
                var payloadDict = new Dictionary<string, string>();

                if (name != null) payloadDict["name"] = name;
                if (hashtag != null) payloadDict["hashtag"] = hashtag;

                var jsonPayload = JsonConvert.SerializeObject(payloadDict);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var endpoint = $"{_baseUrl}/marketdata-migrationproxy/market-data-management/{dataSource.ToLower()}/curated-lists/{id}";
                var request = new HttpRequestMessage(HttpMethod.Patch, endpoint)
                {
                    Content = content
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var message = await response.Content.ReadAsStringAsync();
                var updatedCuratedListResponse = JsonConvert.DeserializeObject<UpdateCuratedListByIdThirdPartyResponse>(message);

                if (updatedCuratedListResponse == null)
                {
                    _logger.LogWarning("Updated curated list response is null");
                    throw new Exception("Updated curated list response is null");
                }

                return updatedCuratedListResponse.Ge?.UpdatedCuratedList ??
                       updatedCuratedListResponse.Set?.UpdatedCuratedList ??
                       throw new Exception("Both Ge and Set updated curated list responses are null");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error - calling UpdateCuratedListById in CuratedManagerService");
                throw;
            }
        }

        public async Task<bool> DeleteCuratedListById(string authToken, string id, string dataSource)
        {
            try
            {
                var endpoint = $"{_baseUrl}/marketdata-migrationproxy/market-data-management/{dataSource.ToLower()}/curated-lists/{id}";

                var request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error - calling DeleteCuratedListById in CuratedManagerService");
                throw;
            }
        }

        public async Task<List<CuratedFilterGroup>> GetCuratedFilters(string authToken, string groupName)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/marketdata-migrationproxy/market-data-management/curated-filters?groupName={Uri.EscapeDataString(groupName)}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var message = await response.Content.ReadAsStringAsync();
                var curatedFiltersResponse = JsonConvert.DeserializeObject<CuratedFiltersThirdPartyApiResponse>(message);
                if (curatedFiltersResponse == null)
                {
                    _logger.LogWarning("Curated filters response is null");
                    return [];
                }

                return TransformCuratedFilters(curatedFiltersResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error - calling GetCuratedFilters in CuratedManagerService and mapping to DTO");
                throw;
            }
        }

        public async Task<List<CuratedFilterGroup>> UploadCuratedFilters(string authToken, IFormFile file, string dataSource)
        {
            try
            {

                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

                var multipartContent = new MultipartFormDataContent
                {
                    { fileContent, "file", file.FileName },
                };

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/marketdata-migrationproxy/market-data-management/{dataSource.ToLower()}/curated-filters")
                {
                    Content = multipartContent
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var message = await response.Content.ReadAsStringAsync();
                var curatedFiltersResponse = JsonConvert.DeserializeObject<CuratedFiltersThirdPartyApiResponse>(message);
                if (curatedFiltersResponse == null)
                {
                    _logger.LogWarning("Curated filters response is null");
                    return [];
                }

                return TransformCuratedFilters(curatedFiltersResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error - calling UploadCuratedFilters in CuratedManagerService and mapping to DTO");
                throw;
            }
        }

        public async Task<List<TransformedCuratedMemberItem>> GetCuratedMembersByCuratedListId(string authToken, string curatedListId)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/marketdata-migrationproxy/market-data-management/curated-members?curatedListId={curatedListId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var message = await response.Content.ReadAsStringAsync();
                var curatedMembersResponse = JsonConvert.DeserializeObject<CuratedMembersThirdPartyApiResponse>(message);
                if (curatedMembersResponse == null)
                {
                    _logger.LogWarning("Curated members response is null");
                    return [];
                }

                return CombineCuratedMembers(curatedMembersResponse.Set?.Data ?? [], curatedMembersResponse.Ge?.Data ?? []);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error - calling GetCuratedMembersByCuratedListId in CuratedManagerService and mapping to DTO");
                throw;
            }
        }
    }
}