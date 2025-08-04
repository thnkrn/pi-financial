using System.Globalization;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Pi.Common.Cryptography;
using Pi.Common.SeedWork;
using Pi.User.Application.Commands;
using Pi.User.Application.Options;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Tests.Commands;

public class PartialUpdateUserInfoConsumerTests : ConsumerTest
{
    private readonly Mock<ILogger<PartialUpdateUserInfoConsumer>> _loggerMock = new();
    private readonly Mock<IUserInfoRepository> _userInfoRepositoryMock = new();
    private readonly Mock<IOptions<DbConfig>> _dbConfigMock = new();
    private readonly Mock<IEncryption> _encryptionMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public PartialUpdateUserInfoConsumerTests()
    {
        _dbConfigMock.SetupGet(s => s.Value)
            .Returns(new DbConfig()
            {
                Salt = "salt"
            });
        _userInfoRepositoryMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<PartialUpdateUserInfoConsumer>(); })
            .AddScoped(_ => _loggerMock.Object)
            .AddScoped(_ => _userInfoRepositoryMock.Object)
            .AddScoped(_ => _dbConfigMock.Object)
            .AddScoped(_ => _encryptionMock.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async Task Consume_ValidMessage_UpdatesUserInfo()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var partialUpdateUserInfo = new PartialUpdateUserInfo(
            userId,
            "1234567890123",
            "test@example.com",
            "FirstnameTh",
            "LastnameTh",
            "FirstnameEn",
            "LastnameEn",
            "0812345678",
            "Bangkok",
            "Thailand",
            DateOnly.FromDateTime(DateTime.Now),
            "Retail"
        );
        var userInfo = new UserInfo(Guid.NewGuid(), "customerId01", null, new UserPersonalInfo(
                partialUpdateUserInfo.CitizenId,
                partialUpdateUserInfo.FirstnameTh,
                partialUpdateUserInfo.LastnameTh,
                partialUpdateUserInfo.FirstnameEn,
                partialUpdateUserInfo.LastnameEn,
                partialUpdateUserInfo.PhoneNumber,
                partialUpdateUserInfo.Email,
                partialUpdateUserInfo.PlaceOfBirthCountry,
                partialUpdateUserInfo.PlaceOfBirthCity,
                partialUpdateUserInfo.DateOfBirth
            ),
            partialUpdateUserInfo.WealthType
        );

        _userInfoRepositoryMock.Setup(s => s.Get(partialUpdateUserInfo.UserId, false))
            .ReturnsAsync(userInfo);
        _encryptionMock.Setup(s => s.Hashed(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("something");

        // Act
        await Harness.Bus.Publish(partialUpdateUserInfo);

        // Assert
        Assert.Equal(partialUpdateUserInfo.CitizenId, userInfo.CitizenId);
        Assert.Equal(partialUpdateUserInfo.PhoneNumber, userInfo.PhoneNumber);
        Assert.Equal(partialUpdateUserInfo.Email, userInfo.Email);
        Assert.Equal(partialUpdateUserInfo.FirstnameTh, userInfo.FirstnameTh);
        Assert.Equal(partialUpdateUserInfo.LastnameTh, userInfo.LastnameTh);
        Assert.Equal(partialUpdateUserInfo.FirstnameEn, userInfo.FirstnameEn);
        Assert.Equal(partialUpdateUserInfo.LastnameEn, userInfo.LastnameEn);
        Assert.Equal(partialUpdateUserInfo.PlaceOfBirthCity, userInfo.PlaceOfBirthCity);
        Assert.Equal(partialUpdateUserInfo.PlaceOfBirthCountry, userInfo.PlaceOfBirthCountry);
    }
}