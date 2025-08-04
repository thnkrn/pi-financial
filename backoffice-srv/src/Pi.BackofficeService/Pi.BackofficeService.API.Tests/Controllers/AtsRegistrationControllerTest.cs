using Microsoft.AspNetCore.Http;
using Pi.BackofficeService.Application.Models.Ats;

namespace Pi.BackofficeService.API.Tests.Controllers;

using Xunit;
using Moq;
using Pi.BackofficeService.API.Controllers;
using Pi.BackofficeService.Application.Services.OnboardService;
using Microsoft.AspNetCore.Mvc;
using Pi.Client.OnboardService.Model;
using Pi.BackofficeService.API.Models;
using OfficeOpenXml;
using System.IO;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AtsRegistrationControllerTests
{
    private readonly AtsRegistrationController _controller;
    private readonly Mock<IOnboardService> _onboardServiceMock;

    public AtsRegistrationControllerTests()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        _onboardServiceMock = new Mock<IOnboardService>();
        _controller = new AtsRegistrationController(_onboardServiceMock.Object);
        _controller.ControllerContext.HttpContext = new DefaultHttpContext();
    }

    [Fact]
    public async Task GetAtsUpdateRequests_ShouldReturnOk_WhenServiceReturnsResult()
    {
        // Arrange
        var query = new AtsRequestQuery();
        query.Page = 1;
        query.PageSize = 10;
        var atsRequestsPaginated = new PiOnboardServiceAPIModelsAtsAtsRequestsPaginated();
        _onboardServiceMock.Setup(s => s.GetAtsUpdateRequests(It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(atsRequestsPaginated);

        // Act
        var result = await _controller.GetAtsUpdateRequests(query);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(atsRequestsPaginated, okResult.Value);
        _onboardServiceMock.Verify(s => s.GetAtsUpdateRequests(It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DownloadAtsUpdateResult_ShouldReturnFile_WhenServiceReturnsResult()
    {
        // Arrange
        var atsRequestId = Guid.NewGuid();
        var atsResults = new PiOnboardServiceAPIModelsAtsAtsUploadResultDtoApiResponse
        {
            Data = new PiOnboardServiceAPIModelsAtsAtsUploadResultDto
            {
                FileName = "ats_upload.xlsx",
                Data =
                [
                    new PiOnboardServiceAPIModelsAtsAtsUploadResultDataDto
                    {
                        CustomerCode = "12345",
                        BankAccount = "67890",
                        BankCode = "ABC",
                        BranchCode = "XYZ",
                        Status = "Completed",
                        FailedReason = "-"
                    }
                ]
            }
        };

        _onboardServiceMock.Setup(s => s.GetAtsUpdateResults(atsRequestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(atsResults);

        // Act
        var result = await _controller.DownloadAtsUpdateResult(atsRequestId);

        // Assert
        var fileResult = Assert.IsType<FileStreamResult>(result);
        Assert.Equal("application/octet-stream", fileResult.ContentType);
        _onboardServiceMock.Verify(s => s.GetAtsUpdateResults(atsRequestId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DownloadAtsUpdateResult_ShouldReturnNotFound_WhenServiceThrowsInvalidDataException()
    {
        // Arrange
        var atsRequestId = Guid.NewGuid();
        _onboardServiceMock.Setup(s => s.GetAtsUpdateResults(atsRequestId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("Not found"));

        // Act
        var result = await _controller.DownloadAtsUpdateResult(atsRequestId);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(404, problemResult.StatusCode);
        var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
        Assert.Equal("Not found", problemDetails.Detail);
    }

    [Fact]
    public async Task InternalUploadAtsBankAccount_ShouldReturnOk_WhenValidPayloadIsProvided()
    {
        // Arrange
        var mockFile = CreateFormFile();
        var payload = new UploadAtsPayload(UploadFile: mockFile,
            UploadType: AtsUploadType.UpdateEffectiveDate, UserName: "test_user");

        _onboardServiceMock.Setup(s => s.AddAtsBankAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRow>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.InternalUploadAtsBankAccount(payload);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _onboardServiceMock.Verify(s => s.AddAtsBankAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRow>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InternalCsvUploadAtsBankAccount_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var payload = new UploadAtsPayload(UploadFile: CreateCsvTestFile(), UploadType: AtsUploadType.OverrideBankInfo,
            UserName: "test_user");

        _onboardServiceMock.Setup(s => s.AddAtsBankAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRow>>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.InternalUploadAtsBankAccount(payload);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, problemResult.StatusCode);
    }

    [Fact]
    public async Task InternalUploadAtsBankAccount_ShouldReturnBadRequest_WhenPayloadIsInvalid()
    {
        // Arrange
        var payload = new UploadAtsPayload(UploadFile: new FormFile(new MemoryStream(), 0, 10, "name", "filename"),
            UploadType: AtsUploadType.OverrideBankInfo, UserName: "test_user");

        _onboardServiceMock.Setup(s => s.AddAtsBankAccount(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<List<PiOnboardServiceApplicationCommandsUpdateAtsBankEffectiveDateRow>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidDataException("Missing some values"));

        // Act
        var result = await _controller.InternalUploadAtsBankAccount(payload);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, problemResult.StatusCode);
    }

    [Fact]
    public async Task InternalUploadAtsBankAccount_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var payload = new UploadAtsPayload(UploadFile: new FormFile(new MemoryStream(), 0, 10, "name", "filename"),
            UploadType: AtsUploadType.OverrideBankInfo, UserName: "test_user");

        // Act
        var result = await _controller.InternalUploadAtsBankAccount(payload);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, problemResult.StatusCode);
        var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
        Assert.Equal("Worksheet position out of range.", problemDetails.Detail);
    }

    private static FormFile CreateFormFile(bool isCsv = false)
    {
        using var package = new ExcelPackage();

        package.Workbook.Worksheets.Add("Sheet1");
        package.Workbook.Worksheets[0].Cells[1, 1].Value = "customerCode";
        package.Workbook.Worksheets[0].Cells[1, 2].Value = "bankAccountNo";
        package.Workbook.Worksheets[0].Cells[1, 3].Value = "bankCode";
        package.Workbook.Worksheets[0].Cells[1, 4].Value = "bankBranchCode";
        package.Workbook.Worksheets[0].Cells[2, 1].Value = "0802800";
        package.Workbook.Worksheets[0].Cells[2, 2].Value = "1234567890";
        package.Workbook.Worksheets[0].Cells[2, 3].Value = "014";
        package.Workbook.Worksheets[0].Cells[2, 4].Value = "1234";

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;

        FormFile? file = new FormFile(stream, 0, stream.Length, "file", isCsv ? "test.csv" : "test.xlsx");
        file.Headers = new HeaderDictionary();
        file.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        return file;
    }

    private static FormFile CreateCsvTestFile()
    {
        var content = "CustomerCode,BankAccountNo,BankCode,BankBranchCode,EffectiveDate\n12345,67890,ABC,XYZ,20230101";
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "test.csv");
        file.Headers = new HeaderDictionary();
        file.ContentType = "text/csv";
        return file;
    }
}
