using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.User.Application.Models.ErrorCode;
using Pi.User.Application.Models.Storage;
using Pi.User.Application.Options;
using Pi.User.Application.Services.Storage;
using Pi.User.Application.Utils;

namespace Pi.User.Application.Queries.Storage;

public class StorageQueries : IStorageQueries
{
    private readonly ILogger<StorageQueries> _logger;
    private readonly IOptionsSnapshot<AwsS3Option> _options;
    private readonly IStorageService _storageService;
    private readonly string DEFAULT_CONTENT_TYPE = "image/jpeg";

    public StorageQueries(
        IStorageService storageService,
        ILogger<StorageQueries> logger,
        IOptionsSnapshot<AwsS3Option> options)
    {
        _storageService = storageService;
        _logger = logger;
        _options = options;
    }

    public async Task<(string, string)> UploadFile(string userId, IFormFile file, string filePostfix)
    {
        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var fileExt = Path.GetExtension(file.FileName).ToLower();
        string? contentType = CommonUtil.SUPPORT_FILE_TYPES.GetValueOrDefault(fileExt);

        var docName = $"{userId}_{filePostfix}_{CommonUtil.RandomNumberCode()}{fileExt}";

        var resp = await DoUploadFile(
            new S3Object(
                docName,
                _options.Value.DocumentBucketName,
                memoryStream,
                contentType
            )
        );

        return (resp, docName);
    }

    public async Task<(string, string)> UploadFile(string userId, string base64, string filePostfix)
    {
        byte[] bytes = Convert.FromBase64String(base64);
        await using var memoryStream = new MemoryStream(bytes);

        var docName = $"{userId}_{filePostfix}_{CommonUtil.RandomNumberCode()}.jpg";

        var resp = await DoUploadFile(
            new S3Object(
                docName,
                _options.Value.DocumentBucketName,
                memoryStream,
                DEFAULT_CONTENT_TYPE
            )
        );

        return (resp, docName);
    }

    public string GetPreSignedURL(string key)
    {
        return _storageService.GetPreSignedURL(key);
    }

    private async Task<string> DoUploadFile(S3Object obj)
    {
        var resp = await _storageService.UploadFileAsync(obj);

        if (!resp.IsSuccess)
        {
            _logger.LogError(resp.Message, "Upload base64 file failed: ");
            throw new InvalidDataException(BankAccountErrorCode.BA001.ToString());
        }

        return
            $"https://{_options.Value.DocumentBucketName}.s3.{S3Region.APSoutheast1.Value}.amazonaws.com/{obj.FileName}";
    }
}