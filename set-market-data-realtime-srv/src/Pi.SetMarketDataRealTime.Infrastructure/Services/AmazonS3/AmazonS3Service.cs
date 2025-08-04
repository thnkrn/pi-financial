using System.Net;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Domain.ConstantConfigurations;
using Pi.SetMarketDataRealTime.Infrastructure.Interfaces.AmazonS3;

namespace Pi.SetMarketDataRealTime.Infrastructure.Services.AmazonS3;

public sealed class AmazonS3Service : IAmazonS3Service, IDisposable
{
    private readonly string _binLogFilePath;
    private readonly string _bucketName;
    private readonly bool _isActivated;
    private readonly ILogger<AmazonS3Service> _logger;
    private readonly AmazonS3Client _s3Client;

    public AmazonS3Service(IConfiguration configuration, ILogger<AmazonS3Service> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var regionEndpoint = RegionEndpoint.GetBySystemName(
            configuration[ConfigurationKeys.BinLogRegion] ?? "ap-southeast-1");

        _s3Client = new AmazonS3Client(regionEndpoint);
        _bucketName = configuration[ConfigurationKeys.BinLogBucketName] ??
                      throw new InvalidOperationException("BinLogBucketName is not configured.");
        _binLogFilePath = configuration[ConfigurationKeys.ServerConfigStreamDataPath] ??
                          throw new InvalidOperationException("BinLogPath is not configured.");

        var isActivated = configuration[ConfigurationKeys.BinLogIsActivated] ?? "true";
        _ = bool.TryParse(isActivated, out _isActivated);

        _logger.LogDebug("AmazonS3Service initialized with bucket: {BucketName}", _bucketName);
    }

    public async Task UploadBinLogToS3Async(string? prefix = null)
    {
        if (!_isActivated)
        {
            _logger.LogWarning("BinLog upload to the S3 module is disabled!");
            return;
        }

        _logger.LogDebug("Starting BinLog upload to S3");

        try
        {
            var fileTransferUtility = new TransferUtility(_s3Client);
            var files = ListFiles();
            _logger.LogDebug("Found {Length} files in {BinLogFilePath}", files.Length, _binLogFilePath);

            foreach (var file in files)
                try
                {
                    if (!File.Exists(file))
                    {
                        _logger.LogDebug("File not found: {File}", file);
                        continue;
                    }

                    var fileName = Path.GetFileName(file);
                    var fileKey = !string.IsNullOrEmpty(prefix) ? $"{prefix}_{fileName}" : fileName;

                    _logger.LogDebug("Uploading file: {File} to {FileKey}", file, fileKey);
                    await fileTransferUtility.UploadAsync(file, _bucketName, fileKey);
                    _logger.LogDebug("Uploaded file: {File}", file);

                    var exist = await CheckIfFileExistsAsync(fileKey);
                    if (exist)
                    {
                        File.Delete(file);
                        _logger.LogWarning("File successfully uploaded and verified: {FileKey}", fileKey);
                    }
                    else
                    {
                        _logger.LogWarning("File not found on S3 after upload attempt: {FileKey}", fileKey);
                    }

                    _logger.LogDebug("Upload completed");
                }
                catch (AmazonS3Exception ex)
                {
                    _logger.LogError(ex,
                        "AWS S3 error processing file: {File}, Error: {Message}, Error Code: {ErrorCode}, Request ID: {RequestId}",
                        file, ex.Message, ex.ErrorCode, ex.RequestId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing file: {File}, Error: {Message}", file, ex.Message);
                }

            _logger.LogDebug("Upload completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during the S3 upload process: {Message}", ex.Message);
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    public async Task ListBucketsAsync()
    {
        try
        {
            var response = await _s3Client.ListBucketsAsync();
            _logger.LogDebug("Retrieved {Count} buckets", response.Buckets.Count);

            foreach (var bucket in response.Buckets)
                _logger.LogDebug("Bucket: {BucketName}, Created: {CreationDate}",
                    bucket.BucketName, bucket.CreationDate);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex,
                "S3 specific error encountered while listing buckets: {Message}, Error Code: {ErrorCode}, Request ID: {RequestId}",
                ex.Message, ex.ErrorCode, ex.RequestId);
            throw new InvalidOperationException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error encountered while listing buckets: {Message}", ex.Message);
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    public async Task CreateBucketAsync(string bucketName)
    {
        try
        {
            // Check if the bucket already exists
            var bucketExists = await DoesBucketExistAsync(bucketName);

            if (bucketExists)
            {
                _logger.LogDebug("Bucket '{BucketName}' already exists.", bucketName);
                return;
            }

            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true
            };

            var response = await _s3Client.PutBucketAsync(putBucketRequest);
            var message = response.HttpStatusCode == HttpStatusCode.OK
                ? $"Successfully created bucket: {bucketName}"
                : $"Failed to create bucket: {bucketName}. Status code: {response.HttpStatusCode}";

            _logger.LogDebug("Message {Message}", message);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex,
                "Error creating bucket '{BucketName}': {Message}, Error Code: {ErrorCode}, Request ID: {RequestId}",
                bucketName, ex.Message, ex.ErrorCode, ex.RequestId);
            throw new InvalidOperationException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating bucket '{BucketName}': {Message}", bucketName, ex.Message);
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    public void Dispose()
    {
        _s3Client.Dispose();
        _logger.LogDebug("AmazonS3Service disposed");
    }

    private async Task<bool> DoesBucketExistAsync(string bucketName)
    {
        try
        {
            var response = await _s3Client.ListBucketsAsync();
            return response.Buckets.Exists(b => b.BucketName == bucketName);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex,
                "Error checking bucket existence: {Message}, Error Code: {ErrorCode}, Request ID: {RequestId}",
                ex.Message, ex.ErrorCode, ex.RequestId);
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    private string[] ListFiles()
    {
        try
        {
            var files = Directory.GetFiles(_binLogFilePath, "*.*", SearchOption.AllDirectories).Where(file =>
                file.EndsWith(".bin", StringComparison.OrdinalIgnoreCase) ||
                file.EndsWith(".log", StringComparison.OrdinalIgnoreCase)).ToArray();
            _logger.LogDebug("Found {Length} files in {BinLogFilePath}", files.Length, _binLogFilePath);
            return files;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing files in {BinLogFilePath}: {Message}", _binLogFilePath, ex.Message);
            throw new InvalidOperationException(ex.Message, ex);
        }
    }

    private async Task<bool> CheckIfFileExistsAsync(string fileName)
    {
        try
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = fileName,
                MaxKeys = 1
            };

            var response = await _s3Client.ListObjectsV2Async(request);
            return response.S3Objects.Exists(obj => obj.Key == fileName);
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(ex,
                "S3 error checking if file exists: {FileName}, Error: {Message}, Error Code: {ErrorCode}, Request ID: {RequestId}",
                fileName, ex.Message, ex.ErrorCode, ex.RequestId);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error checking if file exists: {FileName}, Error: {Message}", fileName,
                ex.Message);
            return false;
        }
    }
}