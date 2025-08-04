using Pi.BackofficeService.Application.Models;
using Microsoft.AspNetCore.Http;
using Pi.BackofficeService.Application.Models.CuratedManager;

namespace Pi.BackofficeService.Application.Services.CuratedManagerService
{
    public interface ICuratedManagerService
    {
        Task<CuratedListResponse> GetCuratedList(string authToken);

        Task<CuratedListResponse> UploadCuratedManualList(string authToken, IFormFile file, string dataSource);

        Task<CuratedListItem> UpdateCuratedListById(string authToken, string id, string? name, string? hashtag, string dataSource);

        Task<bool> DeleteCuratedListById(string authToken, string id, string dataSource);

        Task<List<CuratedFilterGroup>> GetCuratedFilters(string authToken, string groupName);

        Task<List<CuratedFilterGroup>> UploadCuratedFilters(string authToken, IFormFile file, string dataSource);

        Task<List<TransformedCuratedMemberItem>> GetCuratedMembersByCuratedListId(string authToken, string curatedListId);
    }
}