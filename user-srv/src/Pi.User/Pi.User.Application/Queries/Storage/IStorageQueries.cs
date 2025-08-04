using Microsoft.AspNetCore.Http;

namespace Pi.User.Application.Queries.Storage;

public interface IStorageQueries
{
    Task<(string, string)> UploadFile(string userId, IFormFile file, string filePostfix);
    Task<(string, string)> UploadFile(string userId, string base64, string filePostfix);
    string GetPreSignedURL(string key);
}