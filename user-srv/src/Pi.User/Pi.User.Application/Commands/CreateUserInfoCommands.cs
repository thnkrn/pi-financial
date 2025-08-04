using MassTransit;
using Microsoft.Extensions.Options;
using Pi.Common.Cryptography;
using Pi.User.Application.Options;
using Pi.User.Application.Queries;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Commands;

public record CreateUserInfo(
    Guid Id,
    string CustomerId
)
{
    public List<DeviceInfo>? Devices { get; init; } = null;
    public List<string>? CustCodes { get; init; } = null;
    public List<string>? TradingAccounts { get; init; } = null;
    public string? CitizenId { get; init; } = null;
    public string? PhoneNumber { get; init; } = null;
    public string? GlobalAccount { get; init; } = null;
    public string? FirstnameTh { get; init; } = null;
    public string? LastnameTh { get; init; } = null;
    public string? FirstnameEn { get; init; } = null;
    public string? LastnameEn { get; init; } = null;
    public string? Email { get; set; } = null;
    public string? WealthType { get; set; } = null;
}

public record DeviceInfo(
    Guid DeviceId,
    string DeviceToken,
    string Language,
    string Platform
);

public class UserAlreadyExistException : Exception
{
    public UserAlreadyExistException(string message) : base(message)
    {
    }
}

public class CreateUserInfoConsumer : IConsumer<CreateUserInfo>
{
    private readonly IBus _bus;
    private readonly DbConfig _dbConfig;
    private readonly IEncryption _encryption;
    private readonly IUserInfoRepository _userInfoRepository;

    public CreateUserInfoConsumer(
        IBus bus,
        IUserInfoRepository userInfoRepository,
        IOptions<DbConfig> dbConfig,
        IEncryption encryption)
    {
        _bus = bus;
        _userInfoRepository = userInfoRepository;
        _dbConfig = dbConfig.Value;
        _encryption = encryption;
    }

    public async Task Consume(ConsumeContext<CreateUserInfo> context)
    {
        var user = await _userInfoRepository.GetAsync(context.Message.CustomerId);

        if (user != null)
            throw new UserAlreadyExistException(
                $"User with CustomerId {context.Message.CustomerId} is already existed");

        var userInfo = await _userInfoRepository.CreateAsync(
            new UserInfo(
                context.Message.Id,
                context.Message.CustomerId,
                context.Message.GlobalAccount,
                new UserPersonalInfo(
                    context.Message.CitizenId,
                    PhoneNumber: context.Message.PhoneNumber,
                    Email: context.Message.Email,
                    FirstnameEn: context.Message.FirstnameEn,
                    LastnameEn: context.Message.LastnameEn,
                    FirstnameTh: context.Message.FirstnameTh,
                    LastnameTh: context.Message.LastnameTh
                ),
                context.Message.WealthType?.ToLower()
            )
        );
        if (context.Message.CitizenId is not null)
            userInfo.UpdateCitizenIdHash(_encryption.Hashed(context.Message.CitizenId, _dbConfig.Salt));

        if (context.Message.Email is not null)
            userInfo.UpdateEmailHash(_encryption.Hashed(context.Message.Email, _dbConfig.Salt));

        if (context.Message.PhoneNumber is not null)
            userInfo.UpdatePhoneNumberHash(_encryption.Hashed(context.Message.PhoneNumber, _dbConfig.Salt));

        if (context.Message.CustCodes != null && context.Message.CustCodes.Any())
            userInfo.AddCustCodes(context.Message.CustCodes);

        if (context.Message.TradingAccounts != null && context.Message.TradingAccounts.Any())
            userInfo.AddTradingAccounts(context.Message.TradingAccounts.Select(x => new TradingAccount(x, ""))
                .ToList());

        await _userInfoRepository.UnitOfWork.SaveChangesAsync();

        if (context.Message.Devices != null && context.Message.Devices.Any())
        {
            var devicesReq = context.Message.Devices
                .Select(d => new CreateOrUpdateDevice(userInfo.Id, d.DeviceId, d.DeviceToken, d.Language, d.Platform))
                .ToList();

            foreach (var deviceReq in devicesReq)
                await _bus.Publish(
                    deviceReq,
                    ctx =>
                    {
                        ctx.SetDeduplicationId(deviceReq.GetDeduplicationId());
                        ctx.SetGroupId(nameof(CreateOrUpdateDevice));
                    }
                );
        }

        await context.RespondAsync(UserQueries.MapUserFromUserInfo(userInfo));
    }
}