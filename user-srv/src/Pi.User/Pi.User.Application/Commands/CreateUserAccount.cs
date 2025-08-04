using MassTransit;
using Pi.User.Domain.AggregatesModel.UserAccountAggregate;

namespace Pi.User.Application.Commands;

public record CreateUserAccount(Guid UserId, string UserAccountId, UserAccountType UserAccountType);

public class CreateUserAccountConsumer(IUserAccountRepository userAccountRepository) : IConsumer<CreateUserAccount>
{
    public async Task Consume(ConsumeContext<CreateUserAccount> context)
    {
        var userAccount = new UserAccount(context.Message.UserAccountId)
        {
            UserId = context.Message.UserId,
            UserAccountType = context.Message.UserAccountType
        };

        await userAccountRepository.AddAsync(userAccount);
        await userAccountRepository.UnitOfWork.SaveChangesAsync();
    }
}