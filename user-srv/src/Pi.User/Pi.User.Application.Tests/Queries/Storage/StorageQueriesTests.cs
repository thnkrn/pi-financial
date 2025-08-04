using Amazon.S3;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Pi.User.Application.Queries.Storage;
using Moq;
using Pi.User.Application.Services.Storage;
using Pi.User.Application.Options;
using Pi.User.Application.Models.Storage;
using System.Text.RegularExpressions;
using Pi.User.Application.Models.ErrorCode;


namespace Pi.User.Application.Tests.Queries.Storage;

public class StorageQueriesTests
{
    private readonly IStorageQueries _storageQueries;
    private readonly IConfiguration _configuration;
    private readonly string _documentBucketName = "onboard-document";
    private readonly string _supportedContentType = "image/jpeg";
    private readonly string _awsRegion = S3Region.APSoutheast1.Value;

    private readonly Mock<ILogger<StorageQueries>> _mockLogger = new();
    private readonly Mock<IStorageService> _mockStorageService = new();

    public StorageQueriesTests()
    {
        // Mock appsettings configuration.
        var appsettingsConfig = @"{
            ""AWS"": {
                ""Profile"": ""default"",
                ""Region"": ""x""
            },
            ""AwsS3"": {
                ""AccessKey"": ""x"",
                ""SecretKey"": ""x"",
                ""DocumentBucketName"": ""x""
            }
        }";
        _configuration = new ConfigurationBuilder()
            .AddJsonStream(
                new MemoryStream(
                    Encoding.ASCII.GetBytes(appsettingsConfig)))
            .Build();
        _configuration["AwsS3:DocumentBucketName"] = _documentBucketName;
        _configuration["AWS:Region"] = _awsRegion;

        // Create SUT instance with proper dependency injection.
        var serviceCollection = new ServiceCollection();
        serviceCollection
            .AddOptions<AwsS3Option>()
            .Bind(_configuration.GetSection(AwsS3Option.Options))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        serviceCollection
            .AddScoped<ILogger<StorageQueries>>((_) => _mockLogger.Object)
            .AddScoped<IStorageService>((_) => _mockStorageService.Object)
            .AddScoped<StorageQueries>();
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Get SUT from service provider.
        _storageQueries = serviceProvider.GetRequiredService<StorageQueries>();
    }

    [Fact]
    public async Task UploadFile_WhenImageIsBase64StrAndS3UploadIsSuccessful_ShouldReturnCorrectFileNameAndUrl()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var documentType = "documentType";
        var randomNumberPattern = @"\d+";
        var expectedFileNamePattern = $"{userId}_{documentType}_{randomNumberPattern}\\.jpg";
        var expectedFileUrlPattern =
            $"https://{_documentBucketName}\\.s3\\.{_awsRegion}\\.amazonaws\\.com/{expectedFileNamePattern}";
        Regex expectedFileNameRegex = new(expectedFileNamePattern);
        Regex expectedFileUrlRegex = new(expectedFileUrlPattern);

        // Mock base64 string to represent image data.
        var sampleString = "abcdefghijlmnopqrstuvwxyz0123456789";
        var sampleStringBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(sampleString));

        // Mock successful S3 upload.
        var uploadResult = new S3Response(true, 201, $"... has been uploaded sucessfully");
        _mockStorageService
            .Setup(s => s.UploadFileAsync(It.IsAny<S3Object>()))
            .ReturnsAsync(uploadResult);

        // Act
        var (actualFileUrl, actualFileName) =
            await _storageQueries.UploadFile(userId, sampleStringBase64, documentType);

        // Assert
        Assert.True(expectedFileNameRegex.Match(actualFileName).Success);
        Assert.True(expectedFileUrlRegex.Match(actualFileUrl).Success);
    }

    [Fact]
    public async Task UploadFile_WhenImageIsBase64StrAndS3UploadFailed_ShouldThrowError()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var documentType = "documentType";
        var expectedErrorMessage = BankAccountErrorCode.BA001.ToString();

        // Mock base64 string to represent image data.
        var sampleString = "abcdefghijlmnopqrstuvwxyz0123456789";
        var sampleStringBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(sampleString));

        // Mock failed S3 upload.
        var uploadResult = new S3Response(false, 500, $"InternalServerError");
        _mockStorageService
            .Setup(s => s.UploadFileAsync(It.IsAny<S3Object>()))
            .ReturnsAsync(uploadResult);

        // Act and Assert exception
        var actualException = await Assert.ThrowsAsync<InvalidDataException>(() =>
            _storageQueries.UploadFile(userId, sampleStringBase64, documentType));
        Assert.Equal(actualException.Message, expectedErrorMessage);
    }
}