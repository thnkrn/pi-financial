using MassTransit;
using Pi.BackofficeService.Application.Factories;
using Pi.BackofficeService.Domain.AggregateModels.User;

namespace Pi.BackofficeService.Application.Commands.User;

public record UserUpdateOrCreateRequest(Guid IamUserId, string FirstName, string LastName, string Email);

public record UserIdResponse(Guid Id);

public class UserUpdateOrCreateConsumer : IConsumer<UserUpdateOrCreateRequest>
{
    private readonly IUserRepository _userRepository;

    public UserUpdateOrCreateConsumer(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task Consume(ConsumeContext<UserUpdateOrCreateRequest> context)
    {
        var payload = context.Message;
        var user = await _userRepository.GetIamUserId(payload.IamUserId);

        if (user == null)
        {
            user = await _userRepository.Create(EntityFactory.NewUser(payload));
        }
        else if (
            user.FirstName != payload.FirstName ||
            user.LastName != payload.LastName ||
            user.Email != payload.Email
        )
        {
            user.FirstName = payload.FirstName;
            user.LastName = payload.LastName;
            user.Email = payload.Email;

            _userRepository.Update(user);
            await _userRepository.UnitOfWork.SaveChangesAsync();
        }

        await context.RespondAsync(new UserIdResponse(user.Id));
    }
}
