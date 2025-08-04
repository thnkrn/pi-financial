using System.Collections.Concurrent;
using System.Globalization;
using MassTransit;
using Microsoft.Extensions.Options;
using Pi.Common.Cryptography;
using Pi.User.Application.Commands;
using Pi.User.Application.Models;
using Pi.User.Application.Options;
using Pi.User.Application.Services.Customer;
using Pi.User.Application.Services.Onboard;
using Pi.User.Domain.AggregatesModel.TradingAccountAggregate;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Device = Pi.User.Application.Models.Device;
using NotificationPreference = Pi.User.Application.Models.NotificationPreference;
using TradingAccount = Pi.User.Domain.AggregatesModel.UserInfoAggregate.TradingAccount;

namespace Pi.User.Application.Queries;

public class UserQueries : IUserQueries
{
    private readonly IBus _bus;
    private readonly ICustomerService _customerService;
    private readonly DbConfig _dbConfig;
    private readonly IEncryption _encryption;
    private readonly ITradingAccountRepository _tradingAccountRepository;
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly IOnboardTradingAccountService _onboardTradingAccountService;

    public UserQueries(
        IUserInfoRepository userInfoRepository,
        ITradingAccountRepository tradingAccountRepository,
        IBus bus,
        IEncryption encryption,
        IOptions<DbConfig> dbConfig,
        ICustomerService customerService,
        IOnboardTradingAccountService onboardTradingAccountService)
    {
        _userInfoRepository = userInfoRepository;
        _tradingAccountRepository = tradingAccountRepository;
        _bus = bus;
        _encryption = encryption;
        _customerService = customerService;
        _dbConfig = dbConfig.Value;
        _onboardTradingAccountService = onboardTradingAccountService;
    }

    public async Task<Models.User> GetUser(Guid id)
    {
        var res = await _userInfoRepository.Get(id);

        if (res == null) throw new UserNotFoundException($"User not found. UserId: {id}");
        return await MapFromOnboard(res);
    }

    public async Task<Models.User> GetUserByCustomerId(string customerId)
    {
        var res = await _userInfoRepository.Get(customerId);

        if (res == null) throw new UserNotFoundException($"User not found. CustomerId: {customerId}");
        var allTradingAccounts = await GetAllTradingAccounts(res);
        return MapUserFromUserInfo(res, allTradingAccounts);
    }

    public async Task<Models.User> GetUserByEmail(string email)
    {
        var res = await _userInfoRepository.GetByEmail(_encryption.Hashed(email, _dbConfig.Salt));

        if (res == null) throw new UserNotFoundException($"User not found. Email: {email}");
        var allTradingAccounts = await GetAllTradingAccounts(res);
        return MapUserFromUserInfo(res, allTradingAccounts);
    }

    public async Task<Models.User> GetUserByPhoneNumber(string phoneNumber)
    {
        var res = await _userInfoRepository.GetByPhoneNumber(_encryption.Hashed(phoneNumber, _dbConfig.Salt));

        if (res == null) throw new UserNotFoundException($"User not found. Phone number: {phoneNumber}");
        var allTradingAccounts = await GetAllTradingAccounts(res);
        return MapUserFromUserInfo(res, allTradingAccounts);
    }

    public async Task<Models.User> GetUserByCitizenId(string citizenId)
    {
        var res = await _userInfoRepository.GetByCitizenId(_encryption.Hashed(citizenId, _dbConfig.Salt));

        if (res == null) throw new UserNotFoundException($"User not found. CitizenId: {citizenId}");

        var allTradingAccounts = await GetAllTradingAccounts(res);
        return MapUserFromUserInfo(res, allTradingAccounts);
    }

    public async Task<Models.User> GetUserByCustCode(string custCode)
    {
        var id = await GetUserIdByCustomerCode(custCode);

        var res = await _userInfoRepository.Get(id);
        if (res == null) throw new UserNotFoundException($"User not found. UserId: {id}");

        return await MapFromOnboard(res);
    }

    public async Task<Models.User> CreateUserIfNotExist(string customerId)
    {
        try
        {
            var user = await GetUserByCustomerId(customerId);
            return user;
        }
        catch (UserNotFoundException)
        {
            var id = Guid.NewGuid();
            var client = _bus.CreateRequestClient<CreateUserInfo>();
            var response =
                await client.GetResponse<Models.User>(
                    new CreateUserInfo(
                        id,
                        customerId
                    ));
            return response.Message;
        }
    }

    public async Task<IEnumerable<Models.User>> GetBulkUser(IEnumerable<Guid> ids)
    {
        var res = await _userInfoRepository.GetBulk(ids);
        var users = new ConcurrentBag<Models.User>();
        var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };
        await Parallel.ForEachAsync(res, options, async (x, token) =>
        {
            var allTradingAccounts = await GetAllTradingAccounts(x, token);
            users.Add(MapUserFromUserInfo(x, allTradingAccounts));
        });
        return users;
    }

    public async Task<IEnumerable<Models.User>> GetBulkUserByCustomerId(IEnumerable<string> customerIds)
    {
        var res = await _userInfoRepository.GetBulk(customerIds);

        var users = new ConcurrentBag<Models.User>();
        var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };
        await Parallel.ForEachAsync(res, options, async (x, token) =>
        {
            var allTradingAccounts = await GetAllTradingAccounts(x, token);
            users.Add(MapUserFromUserInfo(x, allTradingAccounts));
        });
        return users;
    }


    public async Task<IEnumerable<Models.User>> GetBulkUserByCustCode(IEnumerable<string> custCodes)
    {
        var res = await _userInfoRepository.GetBulkCustCode(custCodes);
        var userInfos = res.Where(x => x.UserInfo != null).Select(x => x.UserInfo!);
        var users = new ConcurrentBag<Models.User>();
        var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };
        await Parallel.ForEachAsync(userInfos, options, async (x, token) =>
        {
            var allTradingAccounts = await GetAllTradingAccounts(x, token);
            users.Add(MapUserFromUserInfo(x, allTradingAccounts));
        });
        return users;
    }

    public async Task<UserCitizenId> GetUserWithCitizenId(Guid id)
    {
        var res = await _userInfoRepository.Get(id);

        if (res == null) throw new InvalidDataException($"User not found. UserId: {id}");

        return MapUserWithCitizenIdFromUserInfo(res);
    }

    public async Task<Guid> GetUserIdByCustomerCode(string customerCode)
    {
        var customerInfo = await _customerService.GetCustomerInfoByCustomerCode(customerCode);
        if (customerInfo == null) throw new UserNotFoundException($"Customer not found. CustomerCode: {customerCode}");

        var userId =
            await _userInfoRepository.GetUserIdByCitizenId(_encryption.Hashed(customerInfo.IdentificationCard.Number,
                _dbConfig.Salt));
        if (!userId.HasValue) throw new UserNotFoundException($"User not found. CustomerCode: {customerCode}");

        return userId.Value;
    }

    public static Models.User MapUserFromUserInfo(UserInfo userInfo, List<TradingAccount>? tradingAccounts = null, List<CustomerCode>? custCodes = null)
    {

        return new Models.User(
            userInfo.Id,
            userInfo.Devices
                .Where(d => d.IsActive)
                .OrderBy(d => d.UpdatedAt)
                .Select(d =>
                    new Device(
                        d.DeviceId,
                        d.DeviceToken,
                        d.DeviceIdentifier,
                        d.Language,
                        d.Platform,
                        new NotificationPreference(
                            d.NotificationPreference.Important,
                            d.NotificationPreference.Order,
                            d.NotificationPreference.Portfolio,
                            d.NotificationPreference.Wallet,
                            d.NotificationPreference.Market
                        )
                    )
                )
                .ToList(),
            custCodes ?? userInfo.CustCodes.Select(
                    c =>
                        new CustomerCode(
                            c.CustomerCode,
                            userInfo
                                .TradingAccounts
                                .Select(t => t.TradingAccountId)
                                .Where(tId =>
                                {
                                    var customerCode = tId.EndsWith("-M")
                                        ? tId.Remove(tId.Length - 2, 2)
                                        : tId.Remove(tId.Length - 1, 1);
                                    return customerCode == c.CustomerCode;
                                })
                                .ToList()
                        )
                )
                .ToList(),
            tradingAccounts ?? userInfo
                .TradingAccounts
                .Select(t => new TradingAccount(t.TradingAccountId, t.AcctCode)
                {
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                })
                .ToList(),
            userInfo.FirstnameTh,
            userInfo.LastnameTh,
            userInfo.FirstnameEn,
            userInfo.LastnameEn,
            userInfo.PhoneNumber,
            userInfo.GlobalAccount,
            userInfo.Email,
            userInfo.CustomerId,
            userInfo.PlaceOfBirthCountry,
            userInfo.PlaceOfBirthCity,
            userInfo.CitizenId,
            MapDate(userInfo.DateOfBirth),
            userInfo.WealthType?.ToLower()
        );
    }

    private UserCitizenId MapUserWithCitizenIdFromUserInfo(UserInfo res)
    {
        return new UserCitizenId(
            res.Id,
            res.CitizenId ?? string.Empty
        );
    }

    private async Task<List<TradingAccount>> GetAllTradingAccounts(UserInfo res,
        CancellationToken cancellationToken = default)
    {
        // todo: this is temporary solution, we need to redesign this when we do the new login migration.
        var allTradingAccounts =
            await _tradingAccountRepository.GetTradingAccountsAsync(res.CustCodes.Select(x => x.CustomerCode),
                cancellationToken);
        return allTradingAccounts
            .Select(x => new TradingAccount(x.TradingAccountNo.Replace("-", string.Empty), x.AccountTypeCode)
            {
                CreatedAt = DateTime.Now
            }).ToList();
    }

    private async Task<Models.User> MapFromOnboard(UserInfo res)
    {
        if (string.IsNullOrWhiteSpace(res.CitizenId)) return MapUserFromUserInfo(res);

        var tradingAccountsGroupedByCustCode =
            await _onboardTradingAccountService
                .GetTradingAccountListGroupedByCustomerCodeByIdentificationNumberAsync(
                    res.CitizenId);

        List<TradingAccount> allTradingAccounts = [];
        List<CustomerCode> customerCodes = [];
        tradingAccountsGroupedByCustCode.ForEach(x =>
        {
            allTradingAccounts.AddRange(x.TradingAccounts
                .Select(d => new TradingAccount(
                    d.TradingAccountNo.Replace("-", string.Empty),
                    d.AccountTypeCode)
                {
                    CreatedAt = DateTime.Now
                }).ToList());
            customerCodes.Add(new CustomerCode(x.CustCode, x.TradingAccounts.Select(
                d => d.AccountTypeCode == "UT" ? d.TradingAccountNo.Replace("-1", "-M") : d.TradingAccountNo.Replace("-", string.Empty)).ToList()));
        });

        return MapUserFromUserInfo(res, allTradingAccounts, customerCodes);
    }

    private static DateOnly? MapDate(string? date)
    {
        if (string.IsNullOrWhiteSpace(date)) return null;

        if (DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var result))
            return result;
        return DateOnly.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
    }

    public async Task<MigrateCustomerInfo> GetMigrateCustomerInfo(int skip, int limit)
    {
        var count = _userInfoRepository.CountCustomerInfoMigration();
        var data = await _userInfoRepository.GetMigrateCustomerInfo(skip, limit);
        data.ForEach(d => d.CustCodes.ToList().ForEach(x => x.UserInfo = null));

        return new MigrateCustomerInfo(data, count);
    }
}