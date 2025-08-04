using Pi.User.Application.Models.Storage;

namespace Pi.User.Application.Services.Storage;

public interface IStorageService
{
    Task<S3Response> UploadFileAsync(S3Object obj);

    string GetPreSignedURL(string key);
}