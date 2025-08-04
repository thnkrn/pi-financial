namespace Pi.SetMarketDataRealTime.Infrastructure.Interfaces.AmazonS3;

public interface IAmazonS3Service
{
    Task CreateBucketAsync(string bucketName);
    Task UploadBinLogToS3Async(string? prefix = null);
    Task ListBucketsAsync();
}