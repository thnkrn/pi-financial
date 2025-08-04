using MassTransit;
using Moq;
using Pi.Common.SeedWork;
using Pi.User.Application.Commands;
using Pi.User.Domain.AggregatesModel.UserAccountAggregate;

namespace Pi.User.Application.Tests.Commands;

public class CreateUserAccountTests
{
    private readonly CreateUserAccountConsumer _createUserAccountConsumer;
    private readonly Mock<IUserAccountRepository> _userAccountRepository = new();

    public CreateUserAccountTests()
    {
        _userAccountRepository.Setup(r => r.UnitOfWork).Returns(new Mock<IUnitOfWork>().Object);
        _createUserAccountConsumer =
            new CreateUserAccountConsumer(_userAccountRepository.Object);
    }

    [Fact]
    public async void Should_Add_UserAccount_Successfully()
    {
        var userAccount = new UserAccount("WALLET001");
        _userAccountRepository
            .Setup(r => r.AddAsync(It.IsAny<UserAccount>()))
            .ReturnsAsync(userAccount);

        var createUserAccount = new CreateUserAccount(Guid.NewGuid(), userAccount.Id, UserAccountType.CashWallet);

        var context = Mock.Of<ConsumeContext<CreateUserAccount>>(c => c.Message == createUserAccount);
        await _createUserAccountConsumer.Consume(context);

        _userAccountRepository.Verify(c => c.AddAsync(It.IsAny<UserAccount>()), Times.Once);
        _userAccountRepository.Verify(c => c.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}