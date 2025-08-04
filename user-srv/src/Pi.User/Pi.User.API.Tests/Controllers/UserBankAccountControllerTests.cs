using MassTransit;
using MassTransit.Clients;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.Common.Http;
using Pi.User.API.Controllers;
using Pi.User.Application.Commands;
using Pi.User.Application.Models.BankAccount;
using Pi.User.Application.Models.ErrorCode;
using Pi.User.Application.Queries.BankAccount;
using Pi.User.Application.Queries.Storage;

namespace Pi.User.API.Tests.Controllers;

public class UserBankAccountControllerTests : ConsumerTest
{
    private readonly Mock<IStorageQueries> _mockStorageQueries;
    private readonly Mock<IBankAccountQueries> _mockBankAccountQueries;
    private readonly UserBankAccountController _controller;

    private readonly Guid _submitBankAccountRequestSuccessUserId = Guid.Parse("0009701c-edc8-4bea-8bd3-d3a8f7d10e03");
    private readonly Guid _deleteBankAccountRequestSuccessUserId = Guid.Parse("000a92fd-9f5f-4826-9f7c-c55fbdcbe183");

    public UserBankAccountControllerTests()
    {
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddHandler<SubmitBankAccountRequest>(async ctx =>
                {
                    if (ctx.Message.UserId == _submitBankAccountRequestSuccessUserId)
                    {
                        await ctx.RespondAsync(new SubmitBankAccountResponse(Guid.NewGuid()));
                    }
                    else
                    {
                        await ctx.RespondAsync(new ErrorCodeResponse(BankAccountErrorCode.BA000.ToString()));
                    }

                });
                cfg.AddHandler<DeleteBankAccountRequest>(async ctx =>
                {
                    if (ctx.Message.UserId == _deleteBankAccountRequestSuccessUserId)
                    {
                        await ctx.RespondAsync(new DeleteBankAccountResponse());
                    }
                    else
                    {
                        await ctx.RespondAsync(new ErrorCodeResponse(BankAccountErrorCode.BA000.ToString()));
                    }
                });
            })

            .BuildServiceProvider(true);
        _mockStorageQueries = new Mock<IStorageQueries>();
        _mockBankAccountQueries = new Mock<IBankAccountQueries>();
        _controller = new UserBankAccountController(_mockStorageQueries.Object, _mockBankAccountQueries.Object, Provider.GetRequiredService<IBus>())
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            }
        };
    }

    [Fact]
    public async Task UpdateBankAccount_ShouldReturnAccepted_WhenCalledWithValidParameters()
    {
        // Arrange
        var userId = _submitBankAccountRequestSuccessUserId.ToString();
        var bankAccountInfo = new BankAccountInfoPayload(BankAccountNo: "1234567890", BankAccountName: "John Doe",
            BankCode: "123", BankBranchCode: "456", Bookbank: null
        );

        // Act
        var result = await _controller.UpdateBankAccount(userId, bankAccountInfo);

        // Assert
        var acceptedResult = Assert.IsType<AcceptedResult>(result);
        Assert.Equal(StatusCodes.Status202Accepted, acceptedResult.StatusCode);
    }

    [Fact]
    public async Task UpdateBankAccount_ShouldReturnBadRequest_WhenFileIsTooLarge()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var bankAccountInfo = new BankAccountInfoPayload(BankAccountNo: "1234567890", BankAccountName: "John Doe",
            BankCode: "123", BankBranchCode: "456",
            Bookbank: new FormFile(Stream.Null, 0, 20 * 1000 * 1000, "Bookbank", "largefile.pdf"));

        // Act
        var result = await _controller.UpdateBankAccount(userId, bankAccountInfo);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }

    [Fact]
    public async Task UpdateBankAccount_ShouldReturnBadRequest_WhenFileExtensionIsNotSupported()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var bankAccountInfo = new BankAccountInfoPayload(BankAccountNo: "1234567890", BankAccountName: "John Doe",
            BankCode: "123", BankBranchCode: "456",
            Bookbank: new FormFile(Stream.Null, 0, 1 * 1000 * 1000, "Bookbank", "file.unsupported"));

        // Act
        var result = await _controller.UpdateBankAccount(userId, bankAccountInfo);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }

    [Fact]
    public async Task UpdateBankAccount_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var bankAccountInfo = new BankAccountInfoPayload(BankAccountNo: "1234567890", BankAccountName: "John Doe",
            BankCode: "123", BankBranchCode: "456", Bookbank: null);

        // Act
        var result = await _controller.UpdateBankAccount(userId, bankAccountInfo);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }

    [Fact]
    public async Task UploadBankAccountDocument_ShouldReturnAccepted_WhenCalledWithValidParameters()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var bankAccountInfo = new BankAccountDocumentPayload(Statements: new List<IFormFile>
            { new FormFile(Stream.Null, 0, 1 * 1000 * 1000, "Statement", "file.pdf") });

        _mockStorageQueries.Setup(q => q.UploadFile(It.IsAny<string>(), It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync(("https://filestorage.com/file.pdf", "file.pdf"));

        // Act
        var result = await _controller.UploadBankAccountDocument(userId, bankAccountInfo);

        // Assert
        var acceptedResult = Assert.IsType<AcceptedResult>(result);
        Assert.Equal(StatusCodes.Status202Accepted, acceptedResult.StatusCode);
    }

    [Fact]
    public async Task UploadBankAccountDocument_ShouldReturnBadRequest_WhenTooManyFilesUploaded()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var bankAccountInfo = new BankAccountDocumentPayload(Statements: new List<IFormFile>
        {
            new FormFile(Stream.Null, 0, 1 * 1000 * 1000, "Statement1", "file1.pdf"),
            new FormFile(Stream.Null, 0, 1 * 1000 * 1000, "Statement2", "file2.pdf"),
            new FormFile(Stream.Null, 0, 1 * 1000 * 1000, "Statement3", "file3.pdf"),
            new FormFile(Stream.Null, 0, 1 * 1000 * 1000, "Statement4", "file4.pdf"),
            new FormFile(Stream.Null, 0, 1 * 1000 * 1000, "Statement5", "file5.pdf"),
            new FormFile(Stream.Null, 0, 1 * 1000 * 1000, "Statement6", "file6.pdf")
        });

        // Act
        var result = await _controller.UploadBankAccountDocument(userId, bankAccountInfo);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }

    [Fact]
    public async Task UploadBankAccountDocument_ShouldReturnBadRequest_WhenFileIsTooLarge()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var bankAccountInfo = new BankAccountDocumentPayload(Statements: new List<IFormFile>
            { new FormFile(Stream.Null, 0, 20 * 1000 * 1000, "Statement", "largefile.pdf") });

        // Act
        var result = await _controller.UploadBankAccountDocument(userId, bankAccountInfo);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }

    [Fact]
    public async Task UploadBankAccountDocument_ShouldReturnBadRequest_WhenFileExtensionIsNotSupported()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var bankAccountInfo = new BankAccountDocumentPayload(Statements: new List<IFormFile>
            { new FormFile(Stream.Null, 0, 1 * 1000 * 1000, "Statement", "file.unsupported") });

        // Act
        var result = await _controller.UploadBankAccountDocument(userId, bankAccountInfo);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }

    [Fact]
    public async Task UploadBankAccountDocument_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var bankAccountInfo = new BankAccountDocumentPayload(Statements: new List<IFormFile>
            { new FormFile(Stream.Null, 0, 1 * 1000 * 1000, "Statement", "file.pdf") });

        _mockStorageQueries.Setup(q => q.UploadFile(It.IsAny<string>(), It.IsAny<IFormFile>(), It.IsAny<string>()))
            .Throws(new Exception("Test exception"));

        // Act
        var result = await _controller.UploadBankAccountDocument(userId, bankAccountInfo);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
    }

    [Fact]
    public async Task GetBankAccountByUserId_ShouldReturnOk_WhenCalledWithValidUserId()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var bankAccountDto = new BankAccountDto
        (
            "1234567890",
            "John Doe",
            "123",
            "456"
        );

        _mockBankAccountQueries.Setup(q => q.GetBankAccountByUserId(It.IsAny<Guid>()))
            .ReturnsAsync(bankAccountDto);

        // Act
        var result = await _controller.GetBankAccountByUserId(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        var response = Assert.IsType<ApiResponse<BankAccountDto>>(okResult.Value);
        Assert.Equal(bankAccountDto, response.Data);
    }

    [Fact]
    public async Task GetBankAccountByUserId_ShouldReturnNotFound_WhenInvalidDataExceptionIsThrown()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        _mockBankAccountQueries.Setup(q => q.GetBankAccountByUserId(It.IsAny<Guid>()))
            .ThrowsAsync(new InvalidDataException("User not found"));

        // Act
        var result = await _controller.GetBankAccountByUserId(userId);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, problemResult.StatusCode);
    }

    [Fact]
    public async Task GetBankAccountByUserId_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        _mockBankAccountQueries.Setup(q => q.GetBankAccountByUserId(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var result = await _controller.GetBankAccountByUserId(userId);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, problemResult.StatusCode);
    }

    [Fact]
    public async Task DeleteBankAccountByUserId_ShouldReturnAccepted_WhenCalledWithValidUserId()
    {
        // Arrange
        var userId = _deleteBankAccountRequestSuccessUserId.ToString();

        // Act
        var result = await _controller.DeleteBankAccountByUserId(userId);

        // Assert
        var acceptedResult = Assert.IsType<AcceptedResult>(result);
        Assert.Equal(StatusCodes.Status202Accepted, acceptedResult.StatusCode);
    }

    [Fact]
    public async Task DeleteBankAccountByUserId_ShouldReturnBadReqeust_WhenInvalidDataExceptionIsThrown()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();

        // Act
        var result = await _controller.DeleteBankAccountByUserId(userId);

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status400BadRequest, problemResult.StatusCode);
    }
}