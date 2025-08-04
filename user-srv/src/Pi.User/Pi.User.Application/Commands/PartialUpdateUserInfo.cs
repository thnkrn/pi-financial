using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Common.Cryptography;
using Pi.User.Application.Options;
using Pi.User.Application.Queries;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;

namespace Pi.User.Application.Commands;

public record PartialUpdateUserInfo(
    Guid UserId,
    string? CitizenId,
    string? Email,
    string? FirstnameTh,
    string? LastnameTh,
    string? FirstnameEn,
    string? LastnameEn,
    string? PhoneNumber,
    string? PlaceOfBirthCity,
    string? PlaceOfBirthCountry,
    DateOnly? DateOfBirth,
    string? WealthType
);

public record PartialUpdateUserInfoResponse(bool IsSuccess);

public class PartialUpdateUserInfoConsumer : IConsumer<PartialUpdateUserInfo>
{
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly DbConfig _dbConfig;
    private readonly IEncryption _encryption;
    private readonly ILogger<PartialUpdateUserInfoConsumer> _logger;

    public PartialUpdateUserInfoConsumer(
        IUserInfoRepository userInfoRepository,
        ILogger<PartialUpdateUserInfoConsumer> logger,
        IEncryption encryption,
        IOptions<DbConfig> dbConfig)
    {
        _userInfoRepository = userInfoRepository;
        _logger = logger;
        _encryption = encryption;
        _dbConfig = dbConfig.Value;
    }

    public async Task Consume(ConsumeContext<PartialUpdateUserInfo> context)
    {
        _logger.LogInformation("Receive PartialUpdateUserInfo request");
        var userInfo = await _userInfoRepository.Get(context.Message.UserId, true) ?? throw new UserNotFoundException($"UserInfo not found. UserId: {context.Message.UserId}");
        if (!string.IsNullOrEmpty(context.Message.CitizenId))
        {
            userInfo.UpdateCitizenId(context.Message.CitizenId);
            userInfo.UpdateCitizenIdHash(_encryption.Hashed(context.Message.CitizenId, _dbConfig.Salt));
        }
        if (!string.IsNullOrEmpty(context.Message.PhoneNumber))
        {
            userInfo.UpdatePhoneNumber(context.Message.PhoneNumber);
            userInfo.UpdatePhoneNumberHash(_encryption.Hashed(context.Message.PhoneNumber, _dbConfig.Salt));
        }
        if (!string.IsNullOrEmpty(context.Message.Email))
        {
            userInfo.UpdateEmail(context.Message.Email);
            userInfo.UpdateEmailHash(_encryption.Hashed(context.Message.Email, _dbConfig.Salt));
        }

        userInfo.UpdateNameTh(context.Message.FirstnameTh ?? userInfo.FirstnameTh ?? string.Empty, context.Message.LastnameTh ?? userInfo.LastnameTh ?? string.Empty);
        userInfo.UpdateNameEn(context.Message.FirstnameEn ?? userInfo.FirstnameEn ?? string.Empty, context.Message.LastnameEn ?? userInfo.LastnameEn ?? string.Empty);
        userInfo.UpdatePlaceOfBirth(context.Message.PlaceOfBirthCountry ?? userInfo.PlaceOfBirthCountry ?? string.Empty,
            context.Message.PlaceOfBirthCity ?? userInfo.PlaceOfBirthCity ?? string.Empty);

        if (context.Message.DateOfBirth != null)
        {
            userInfo.UpdateDateOfBirth(context.Message.DateOfBirth?.ToString("yyyy-MM-dd")!);
        }

        userInfo.UpdateWealthType(context.Message.WealthType?.ToLower() ??
                                  userInfo.WealthType?.ToLower() ??
                                  string.Empty);

        await _userInfoRepository.UnitOfWork.SaveChangesAsync(context.CancellationToken);
        await context.RespondAsync(new PartialUpdateUserInfoResponse(true));
    }
}