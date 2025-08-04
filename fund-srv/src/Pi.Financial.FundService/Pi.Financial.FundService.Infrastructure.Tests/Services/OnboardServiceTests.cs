// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Logging;
using Moq;
using Pi.Client.OnboardService.Api;
using Pi.Client.OnboardService.Client;
using Pi.Client.OnboardService.Model;


namespace Pi.Financial.FundService.Infrastructure.Tests.Services;

public class OnboardServiceTests
{
    private readonly Mock<IOpenAccountApi> _openAccountApi = new();
    private readonly Mock<ICrsApi> _crsApi = new();
    private readonly Mock<ITradingAccountApi> _tradingAccountApi = new();
    private readonly Mock<IBankAccountApi> _bankAccountApi = new();
    private readonly Mock<IDopaApi> _dopaApi = new();
    private readonly Mock<IUserDocumentApi> _userDocumentApi = new();
    private readonly Infrastructure.Services.OnboardService _onboardService;
    private readonly Mock<ILogger<Infrastructure.Services.OnboardService>> _logger = new();

    public OnboardServiceTests()
    {
        _onboardService = new Infrastructure.Services.OnboardService(
            _openAccountApi.Object,
            _crsApi.Object,
            _tradingAccountApi.Object,
            _bankAccountApi.Object,
            _dopaApi.Object,
            _userDocumentApi.Object,
            _logger.Object
            );
    }

    [Fact]
    public async Task GetDopaSuccessInfoByUserId_ThenReturnCorrectFormatOfDateAndTime()
    {
        //Arrange
        var now = DateTime.Now;
        var mockData =
            new PiOnboardServiceAPIModelsDopaDopaRequestDtoApiResponse(
                new PiOnboardServiceAPIModelsDopaDopaRequestDto(
                    now,
                    "SUCCESS"
                    ));

        var userId = Guid.NewGuid();
        _dopaApi
            .Setup(d => d.InternalGetDopaSuccessInfoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockData);

        //Act
        var actual = await _onboardService.GetDopaSuccessInfoByUserId(userId);

        //Assert
        Assert.Equivalent(now, actual);
    }

    [Fact]
    public async Task WhenGetDopaSuccessInfoByUserId_NoRecordFound_WillNotThrowErrorAndReturnNull()
    {
        var userId = Guid.NewGuid();
        _dopaApi
            .Setup(d => d.InternalGetDopaSuccessInfoAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Throws<ApiException>();

        //Act
        var actual = await _onboardService.GetDopaSuccessInfoByUserId(userId);

        //Assert
        Assert.Null(actual);
    }
}
