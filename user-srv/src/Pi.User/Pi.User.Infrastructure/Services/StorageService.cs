using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.User.Application.Models.Storage;
using Pi.User.Application.Options;
using Pi.User.Application.Services.Storage;
using S3Object = Pi.User.Application.Models.Storage.S3Object;

namespace Pi.User.Infrastructure.Services;

public class StorageService : IStorageService
{
    private readonly ILogger<StorageService> _logger;
    private readonly IOptionsSnapshot<AwsS3Option> _options;
    public StorageService(
        ILogger<StorageService> logger,
        IOptionsSnapshot<AwsS3Option> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task<S3Response> UploadFileAsync(S3Object obj)
    {

        try
        {
            var uploadRequest = new TransferUtilityUploadRequest()
            {
                InputStream = obj.InputStream,
                Key = obj.FileName,
                BucketName = obj.BucketName,
                CannedACL = S3CannedACL.NoACL,
                ContentType = obj.ContentType
            };

            // initialise client
            using var client = new AmazonS3Client(Amazon.RegionEndpoint.APSoutheast1);

            // initialise the transfer/upload tools
            var transferUtility = new TransferUtility(client);

            // initiate the file upload
            await transferUtility.UploadAsync(uploadRequest);

            return new S3Response(true, 201, $"{obj} has been uploaded sucessfully");
        }
        catch (AmazonS3Exception s3Ex)
        {
            _logger.LogError(s3Ex, "UploadFileAsync Error");
            return new S3Response(false, (int)s3Ex.StatusCode, s3Ex.Message);
        }
        catch (Exception ex)
        {
            return new S3Response(false, 500, ex.Message);
        }
    }

    public string GetPreSignedURL(string key)
    {

        try
        {
            var getPreSignedUrlRequest = new GetPreSignedUrlRequest()
            {
                Key = key,
                Expires = DateTime.UtcNow.AddDays(5),
                BucketName = _options.Value.DocumentBucketName,
            };

            // initialise client
            using var client = new AmazonS3Client(Amazon.RegionEndpoint.APSoutheast1);
            return client.GetPreSignedURL(getPreSignedUrlRequest);
        }
        catch (AmazonS3Exception s3Ex)
        {
            _logger.LogError($"GetPreSignedURL for {key} AmazonS3Exception: ", s3Ex);
            return "";
        }
        catch (Exception ex)
        {
            _logger.LogError($"GetPreSignedURL for {key} Error: ", ex);
            return "";
        }
    }
}