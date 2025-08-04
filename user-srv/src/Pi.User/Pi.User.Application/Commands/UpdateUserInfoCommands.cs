using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Common.Cryptography;
using Pi.Common.Domain.AggregatesModel.ProductAggregate;
using Pi.OnboardService.IntegrationEvents;
using Pi.User.Application.Options;
using Pi.User.Application.Queries;
using Pi.User.Application.Services.LegacyUserInfo;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Pi.User.Domain.Metrics;
using System.Linq;

namespace Pi.User.Application.Commands;

public record UpdateUserInfo(
    Guid Id
)
{
    public List<UpdateDevice>? Devices { get; init; }
    public List<string>? CustCodes { get; init; }
    public List<string>? TradingAccounts { get; init; }
    public string? CitizenId { get; init; }
    public string? PhoneNumber { get; init; }
    public string? GlobalAccount { get; init; }
    public string? Sid { get; init; }
    public string? CurrentDeviceId { get; init; }
    public string? CurrentPlatform { get; init; }
}

public record UpdateDevice(
    Guid DeviceId,
    string DeviceToken,
    string Language,
    string Platform
);

public class UpdateUserInfoConsumer : IConsumer<UpdateUserInfo>, IConsumer<OpenAccountSuccessEvent>, IConsumer<OpenAccountAutoApproved>
{
    private readonly IBus _bus;
    private readonly IUserInfoService _userInfoService;
    private readonly DbConfig _dbConfig;
    private readonly IEncryption _encryption;
    private readonly IProductRepository _productRepository;
    private readonly OtelMetrics _metrics;
    private readonly ILogger<UpdateUserInfoConsumer> _logger;
    private readonly IUserInfoRepository _userInfoRepository;

    public UpdateUserInfoConsumer(
        IBus bus,
        IUserInfoRepository userInfoRepository,
        ILogger<UpdateUserInfoConsumer> logger,
        IUserInfoService userInfoService,
        IEncryption encryption,
        IOptions<DbConfig> dbConfig,
        IProductRepository productRepository,
        OtelMetrics metrics)
    {
        _bus = bus;
        _userInfoRepository = userInfoRepository;
        _logger = logger;
        _userInfoService = userInfoService;
        _encryption = encryption;
        _productRepository = productRepository;
        _metrics = metrics;
        _dbConfig = dbConfig.Value;
    }

    public async Task Consume(ConsumeContext<UpdateUserInfo> context)
    {
        try
        {
            var userInfo = await _userInfoRepository.GetAsync(context.Message.Id);

            if (userInfo == null) throw new InvalidDataException($"UserInfo not found. UserId: {context.Message.Id}");

            if (!string.IsNullOrWhiteSpace(context.Message.Sid) &&
                !string.IsNullOrWhiteSpace(context.Message.CurrentDeviceId) &&
                !string.IsNullOrWhiteSpace(context.Message.CurrentPlatform))
            {
                var customerInfo = await _userInfoService.GetByToken(context.Message.Sid,
                    context.Message.CurrentDeviceId, context.Message.CurrentPlatform);
                var tradingAccounts = customerInfo.CustomerCodes?.Aggregate(new List<string>(),
                    (acc, curr) => acc.Union(curr.TradingAccounts).ToList());
                var customerCodes = customerInfo.CustomerCodes?.Select(c => c.CustomerCode).ToList();

                if (tradingAccounts?.Any() ?? false)
                {
                    var tradingAccountDiffList =
                        tradingAccounts
                            .Where(t => userInfo.TradingAccounts.All(a => a.TradingAccountId != t))
                            .Select(x => new Domain.AggregatesModel.UserInfoAggregate.TradingAccount(x, ""));
                    userInfo.AddTradingAccounts(tradingAccountDiffList);
                }

                if (customerCodes?.Any() ?? false)
                {
                    var custCodeDiffList = customerCodes.Where(t => userInfo.CustCodes.All(c => c.CustomerCode != t));
                    userInfo.AddCustCodes(custCodeDiffList);
                }

                if (!string.IsNullOrEmpty(customerInfo.CitizenId))
                {
                    userInfo.UpdateCitizenId(customerInfo.CitizenId);
                    userInfo.UpdateCitizenIdHash(_encryption.Hashed(customerInfo.CitizenId, _dbConfig.Salt));
                }
                if (!string.IsNullOrEmpty(customerInfo.PhoneNumber))
                {
                    userInfo.UpdatePhoneNumber(customerInfo.PhoneNumber);
                    userInfo.UpdatePhoneNumberHash(_encryption.Hashed(customerInfo.PhoneNumber, _dbConfig.Salt));
                }
                if (!string.IsNullOrEmpty(customerInfo.Email))
                {
                    userInfo.UpdateEmail(customerInfo.Email);
                    userInfo.UpdateEmailHash(_encryption.Hashed(customerInfo.Email, _dbConfig.Salt));
                }
                if (!string.IsNullOrEmpty(customerInfo.GlobalAccount))
                {
                    userInfo.UpdateGlobalAccount(customerInfo.GlobalAccount);
                }
                if (!string.IsNullOrEmpty(customerInfo.FirstnameTh) || !string.IsNullOrEmpty(customerInfo.LastnameTh))
                {
                    userInfo.UpdateNameTh(customerInfo.FirstnameTh, customerInfo.LastnameTh);
                }
                if (!string.IsNullOrEmpty(customerInfo.FirstnameEn) || !string.IsNullOrEmpty(customerInfo.LastnameEn))
                {
                    userInfo.UpdateNameEn(customerInfo.FirstnameEn, customerInfo.LastnameEn);
                }
            }

            await _userInfoRepository.UnitOfWork.SaveChangesAsync();

            await context.RespondAsync(UserQueries.MapUserFromUserInfo(userInfo));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update user info: {Message}", e.Message);
            throw;
        }
    }

    public async Task Consume(ConsumeContext<OpenAccountSuccessEvent> context)
    {
        try
        {
            var products = (await _productRepository.GetProducts()).ToDictionary(o => o.Id);
            var userInfo = await _userInfoRepository.GetAsync(context.Message.UserId) ?? throw new InvalidDataException($"UserInfo not found. UserId: {context.Message.UserId}");

            userInfo.UpdateCitizenId(context.Message.UserInfo.CitizenId);
            userInfo.UpdateCitizenIdHash(_encryption.Hashed(context.Message.UserInfo.CitizenId, _dbConfig.Salt));
            userInfo.UpdateNameTh(context.Message.UserInfo.FirstNameTh, context.Message.UserInfo.LastNameTh);
            userInfo.UpdateNameEn(context.Message.UserInfo.FirstNameEn, context.Message.UserInfo.LastNameEn);
            userInfo.AddTradingAccounts(context.Message.AccountList.Select(a => new Domain.AggregatesModel.UserInfoAggregate.TradingAccount(GetTradingAccountId(a.CustomerCode, products[a.ProductId].Suffix), products[a.ProductId].AccountTypeCode)));
            userInfo.UpdateCustCode(
                context.Message.AccountList.FirstOrDefault()?.CustomerCode ?? throw new InvalidDataException($"CustCode not found. UserId: {context.Message.UserId}")
            );

            await _userInfoRepository.UnitOfWork.SaveChangesAsync();
            this._metrics.UpdateUser();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while consume OpenAccountSuccessEvent: {Message}", ex.Message);
            throw;
        }
    }

    public async Task Consume(ConsumeContext<OpenAccountAutoApproved> context)
    {
        try
        {
            _logger.LogInformation("start consume UpdateCustomerCode: [{UserId}] [{CustCode}]", context.Message.UserId, context.Message.custCode);
            var userInfo = await _userInfoRepository.GetAsync(context.Message.UserId) ?? throw new InvalidDataException($"UserInfo not found. UserId: {context.Message.UserId}");

            userInfo.UpdateCustCode(context.Message.custCode);

            await _userInfoRepository.UnitOfWork.SaveChangesAsync();
            this._metrics.UpdateUser();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while consume UpdateCustomerCode: {Message}", ex.Message);
            throw;
        }
    }

    private string GetTradingAccountId(string custCode, string suffix)
    {
        var tradingAccountId = $"{custCode}{suffix}";
        if (suffix.Equals("UT"))
        {
            tradingAccountId = $"{custCode}-M";
        }
        return tradingAccountId;
    }
}