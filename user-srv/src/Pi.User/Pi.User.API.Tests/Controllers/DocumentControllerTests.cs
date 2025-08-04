using System.Security.Cryptography;
using Pi.User.Application.Commands;
using Pi.User.Application.Queries.Storage;
using Pi.User.Application.Queries.Document;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MassTransit;
using Moq;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;
using Pi.Common.SeedWork;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Pi.Common.Http;
using Pi.User.API.Controllers;
using Pi.User.Application.Models.Document;
using Pi.User.Application.Services.Storage;

namespace Pi.User.API.Tests.Controllers;

public class DocumentControllerTests : ConsumerTest
{
    private readonly Mock<ILogger<IStorageQueries>> _mockStorageQueriesLogger = new();
    private readonly Mock<ILogger<IStorageService>> _mockStorageServiceLogger = new();
    private readonly Mock<ILogger<IDocumentRepository>> _mockDocumentRepositoryLogger = new();
    private readonly Mock<IStorageService> _mockStorageService = new();
    private readonly Mock<IStorageQueries> _mockStorageQueries = new();
    private readonly Mock<IDocumentQueries> _mockDocumentQueries = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IDocumentRepository> _mockDocumentRepository = new();
    private readonly DocumentController _documentController;

    public DocumentControllerTests()
    {
        _mockDocumentRepository.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);

        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { })
            // .AddScoped<ILogger<IDocumentRepository>>(_ => _mockDocumentRepositoryLogger.Object)
            // .AddScoped<ILogger<IStorageService>>(_ => _mockStorageServiceLogger.Object)
            // .AddScoped<ILogger<IStorageQueries>>(_ => _mockStorageQueriesLogger.Object)
            // .AddScoped<IStorageService>(_ => _mockStorageService.Object)
            // .AddScoped<IDocumentRepository>(_ => _mockDocumentRepository.Object)
            .AddScoped<IStorageQueries>(_ => _mockStorageQueries.Object)
            .AddScoped<IDocumentQueries>(_ => _mockDocumentQueries.Object)
            .AddScoped<DocumentController>()
            .BuildServiceProvider();

        _documentController = Provider.GetRequiredService<DocumentController>();
    }

    [Fact]
    public async Task UploadDocument_WhenGivenValidDocumentTypeAndImageFileSizeIsLessThanTheLimitAndUploadToStorageSuccessfully_ShouldReturnAcceptedAndPublishEventToSubmitDocument()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userIdString = userId.ToString();

        var imageFileSizeBytes = 1;
        var documentType = DocumentType.Signature;
        var fileName = $"{userIdString}_{documentType}_123456.jpg";
        var fileUrl = $"https://onboard-document.s3.ap-southeast-1.amazonaws.com/{fileName}";

        // Mock base64 string to represent image data.
        var imageDataBase64String = GenerateBase64StringOfRandomBytesData(imageFileSizeBytes);

        SubmitDocumentRequest expectedSubmitDocumentRequest = new(userId,
            new List<SubmitDocument>() { new SubmitDocument(documentType, fileUrl, fileName) });
        List<Type> expectedPublishedEvents = new() { typeof(SubmitDocumentRequest) };
        List<SubmitDocumentRequest> expectedPublishedMessages = new() { expectedSubmitDocumentRequest };

        UploadDocumentRequest uploadDocumentRequest = new(imageDataBase64String, documentType);

        _mockStorageQueries
            .Setup(s => s.UploadFile(
                userIdString, imageDataBase64String, documentType.ToString()))
            .ReturnsAsync((fileUrl, fileName));

        // Act
        var result = await _documentController.UploadDocument(userIdString, uploadDocumentRequest);

        // Assert
        Assert.IsType<AcceptedResult>(result);

        var actualPublishedEvents = Harness.Published
            .Select(q => true)
            .Select(x => x.MessageType).ToList();
        var actualPublishedMessages = Harness.Published
            .Select(q => true)
            .Select(x => x.MessageObject).ToList();

        actualPublishedEvents.Should().BeEquivalentTo(expectedPublishedEvents);
        actualPublishedMessages.Should().BeEquivalentTo(expectedPublishedMessages);
    }

    [Fact]
    public async Task UploadDocument_WhenGivenValidDocumentTypeAndImageFileSizeIsExactlyTheLimitAndUploadToStorageSuccessfully_ShouldReturnAcceptedAndPublishEventToSubmitDocument()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userIdString = userId.ToString();

        var imageFileSizeBytes = 10 * 1000 * 1000; // 10MB
        var documentType = DocumentType.Signature;
        var fileName = $"{userIdString}_{documentType}_123456.jpg";
        var fileUrl = $"https://onboard-document.s3.ap-southeast-1.amazonaws.com/{fileName}";

        // Mock base64 string to represent image data.
        var imageDataBase64String = GenerateBase64StringOfRandomBytesData(imageFileSizeBytes);

        SubmitDocumentRequest expectedSubmitDocumentRequest = new(userId,
            new List<SubmitDocument>() { new SubmitDocument(documentType, fileUrl, fileName) });
        List<Type> expectedPublishedEvents = new() { typeof(SubmitDocumentRequest) };
        List<SubmitDocumentRequest> expectedPublishedMessages = new() { expectedSubmitDocumentRequest };

        UploadDocumentRequest uploadDocumentRequest = new(imageDataBase64String, documentType);

        _mockStorageQueries
            .Setup(s => s.UploadFile(
                userIdString, imageDataBase64String, documentType.ToString()))
            .ReturnsAsync((fileUrl, fileName));

        // Act
        var result = await _documentController.UploadDocument(userIdString, uploadDocumentRequest);

        // Assert
        Assert.IsType<AcceptedResult>(result);

        var actualPublishedEvents = Harness.Published
            .Select(q => true)
            .Select(x => x.MessageType).ToList();
        var actualPublishedMessages = Harness.Published
            .Select(q => true)
            .Select(x => x.MessageObject).ToList();

        actualPublishedEvents.Should().BeEquivalentTo(expectedPublishedEvents);
        actualPublishedMessages.Should().BeEquivalentTo(expectedPublishedMessages);
    }

    [Fact]
    public async Task UploadDocument_WhenGivenImageFileLargerThanMaxLimit_ShouldReturnErrorAndPublishNoEvents()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userIdString = userId.ToString();

        var imageFileSizeBytes = 11 * 1000 * 1000; // 11MB
        var documentType = DocumentType.Signature;
        var fileName = $"{userIdString}_{documentType}_123456.jpg";
        var fileUrl = $"https://onboard-document.s3.ap-southeast-1.amazonaws.com/{fileName}";

        var expectedProblemDetails = $"{documentType} document exceeds maximum size";

        // Mock base64 string to represent image data.
        var imageDataBase64String = GenerateBase64StringOfRandomBytesData(imageFileSizeBytes);

        UploadDocumentRequest uploadDocumentRequest = new(imageDataBase64String, documentType);

        _mockStorageQueries
            .Setup(s => s.UploadFile(
                userIdString, imageDataBase64String, documentType.ToString()))
            .ReturnsAsync((fileUrl, fileName));

        // Act
        var result = await _documentController.UploadDocument(userIdString, uploadDocumentRequest);

        // Assert
        var objResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objResult.Value);
        Assert.Equal(400, problemDetails.Status);
        Assert.Equal(expectedProblemDetails, problemDetails.Detail);

        var actualPublishedEvents = Harness.Published
            .Select(q => true)
            .Select(x => x.MessageType).ToList();
        var actualPublishedMessages = Harness.Published
            .Select(q => true)
            .Select(x => x.MessageObject).ToList();

        Assert.Empty(actualPublishedEvents);
        Assert.Empty(actualPublishedMessages);
    }

    [Fact]
    public async Task UploadDocument_WhenGivenValidDocumentAndUploadToStorageFailed_ShouldReturnErrorAndPublishNoEvents()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userIdString = userId.ToString();

        var imageFileSizeBytes = 1;
        var documentType = DocumentType.Signature;

        // Mock base64 string to represent image data.
        var imageDataBase64String = GenerateBase64StringOfRandomBytesData(imageFileSizeBytes);

        UploadDocumentRequest uploadDocumentRequest = new(imageDataBase64String, documentType);

        _mockStorageQueries
            .Setup(s => s.UploadFile(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Throws(new Exception("some error"));

        // Act
        var result = await _documentController.UploadDocument(userIdString, uploadDocumentRequest);

        // Assert
        var objResult = Assert.IsType<ObjectResult>(result);
        var problemDetails = Assert.IsType<ProblemDetails>(objResult.Value);
        Assert.Equal(500, problemDetails.Status);
        Assert.Equal("some error", problemDetails.Detail);

        var actualPublishedEvents = Harness.Published
            .Select(q => true)
            .Select(x => x.MessageType).ToList();
        var actualPublishedMessages = Harness.Published
            .Select(q => true)
            .Select(x => x.MessageObject).ToList();

        Assert.Empty(actualPublishedEvents);
        Assert.Empty(actualPublishedMessages);
    }

    [Fact]
    public async Task GetDocumentsByUserId_ShouldReturnCorrectly()
    {
        //Arrange
        var id = Guid.NewGuid();
        _mockDocumentQueries
            .Setup(x => x.GetDocumentsWithPreSignedUrlByUserId(It.IsAny<Guid>()))
            .ReturnsAsync([new DocumentDto("fileUrl", "fileName", DocumentType.Selfie, DateTime.Now)]);

        //Act
        var result = await _documentController.GetDocumentsByUserId(id.ToString());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var apiResponse = Assert.IsType<ApiResponse<List<DocumentDto>>>(okResult.Value);
        Assert.NotEmpty(apiResponse.Data);
    }

    private string GenerateBase64StringOfRandomBytesData(int fileSizeBytes)
    {
        var fileDataBytes = new byte[fileSizeBytes];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(fileDataBytes);
        }
        var imageDataBase64String = System.Convert.ToBase64String(fileDataBytes);
        return imageDataBase64String;
    }


}