using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Pi.BackofficeService.Application.Commands.User;
using Pi.BackofficeService.Domain.AggregateModels.User;
using Pi.BackofficeService.Domain.SeedWork;
using UserEntity = Pi.BackofficeService.Domain.AggregateModels.User.User;

namespace Pi.BackofficeService.Application.Tests.Commands.User;

public class UserUpdateOrCreateConsumerTest : ConsumerTest
{
    private readonly Mock<IUserRepository> _userRepository;

    public UserUpdateOrCreateConsumerTest()
    {
        _userRepository = new Mock<IUserRepository>();
        _userRepository
            .Setup(r => r.UnitOfWork)
            .Returns(new Mock<IUnitOfWork>().Object);
        Provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg => { cfg.AddConsumer<UserUpdateOrCreateConsumer>(); })
            .AddScoped<IUserRepository>(_ => _userRepository.Object)
            .BuildServiceProvider(true);
    }

    [Fact]
    public async void Should_Return_Existing_User_When_UserAlreadyExist()
    {
        // Arrange
        var user = new UserEntity(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");
        var client = Harness.GetRequestClient<UserUpdateOrCreateRequest>();
        _userRepository.Setup(q => q.GetIamUserId(It.IsAny<Guid>())).ReturnsAsync(user);

        // Act
        var response = await client.GetResponse<UserIdResponse>(
            new UserUpdateOrCreateRequest(user.IamUserId, user.FirstName, user.LastName, user.Email)
        );

        // Assert
        Assert.Equal(user.Id, response.Message.Id);
    }

    [Fact]
    public async void Should_Update_Existing_User_When_UserRequestFirstName_Not_EqualExistingUser()
    {
        // Arrange
        var client = Harness.GetRequestClient<UserUpdateOrCreateRequest>();
        var updatedFirstName = "updated";
        var user = new UserEntity(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");
        _userRepository.Setup(q => q.GetIamUserId(It.IsAny<Guid>())).ReturnsAsync(user);
        _userRepository.Setup(q => q.Update(It.IsAny<UserEntity>())).Verifiable();

        // Act
        await client.GetResponse<UserIdResponse>(
            new UserUpdateOrCreateRequest(user.IamUserId, updatedFirstName, user.LastName, user.Email)
        );

        // Assert
        _userRepository.Verify();
    }

    [Fact]
    public async void Should_Update_Existing_User_When_UserRequestLastName_Not_EqualExistingUser()
    {
        // Arrange
        var client = Harness.GetRequestClient<UserUpdateOrCreateRequest>();
        var updated = "updated";
        var user = new UserEntity(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");
        _userRepository.Setup(q => q.GetIamUserId(It.IsAny<Guid>())).ReturnsAsync(user);
        _userRepository.Setup(q => q.Update(It.IsAny<UserEntity>())).Verifiable();

        // Act
        await client.GetResponse<UserIdResponse>(
            new UserUpdateOrCreateRequest(user.IamUserId, user.FirstName, updated, user.Email)
        );

        // Assert
        _userRepository.Verify();
    }

    [Fact]
    public async void Should_Update_Existing_User_When_UserRequestEmail_Not_EqualExistingUser()
    {
        // Arrange
        var client = Harness.GetRequestClient<UserUpdateOrCreateRequest>();
        var updated = "updated";
        var user = new UserEntity(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");
        _userRepository.Setup(q => q.GetIamUserId(It.IsAny<Guid>())).ReturnsAsync(user);
        _userRepository.Setup(q => q.Update(It.IsAny<UserEntity>())).Verifiable();

        // Act
        await client.GetResponse<UserIdResponse>(
            new UserUpdateOrCreateRequest(user.IamUserId, user.FirstName, user.LastName, updated)
        );

        // Assert
        _userRepository.Verify();
    }

    [Fact]
    public async void Should_Return_Existing_User_When_UserRequestFirstName_Not_EqualExistingUser()
    {
        // Arrange
        var client = Harness.GetRequestClient<UserUpdateOrCreateRequest>();
        var updatedFirstName = "updated";
        var user = new UserEntity(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");
        _userRepository.Setup(q => q.GetIamUserId(It.IsAny<Guid>())).ReturnsAsync(user);
        _userRepository.Setup(q => q.Update(It.IsAny<UserEntity>())).Verifiable();

        // Act
        var response = await client.GetResponse<UserIdResponse>(
            new UserUpdateOrCreateRequest(user.IamUserId, updatedFirstName, user.LastName, user.Email)
        );

        // Assert
        Assert.Equal(user.Id, response.Message.Id);
    }

    [Fact]
    public async void Should_Create_New_User_When_UserNotExist()
    {
        // Arrange
        var client = Harness.GetRequestClient<UserUpdateOrCreateRequest>();
        var user = new UserEntity(Guid.NewGuid(), Guid.NewGuid(), "FirstName", "LastName", "some@mail.com");
        _userRepository.Setup(q => q.GetIamUserId(It.IsAny<Guid>()));
        _userRepository.Setup(q => q.Create(It.IsAny<UserEntity>())).ReturnsAsync(user).Verifiable();

        // Act
        await client.GetResponse<UserIdResponse>(
            new UserUpdateOrCreateRequest(Guid.NewGuid(), user.FirstName, user.LastName, user.Email)
        );

        // Assert
        _userRepository.Verify();
    }
}
