using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pi.Client.OnboardService.Api;
using Pi.Client.OnboardService.Model;
using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.UserInfoAggregate;
using Pi.User.Domain.Metrics;

namespace Pi.User.Infrastructure.Repositories;
public class UserInfoRepository : IUserInfoRepository
{
    private readonly ILogger<UserInfoRepository> _logger;
    private readonly OtelMetrics _otelMetrics;
    private readonly UserDbContext _userDbContext;

    public UserInfoRepository(
        UserDbContext userDbContext,
        ILogger<UserInfoRepository> logger,
        OtelMetrics otelMetrics)
    {
        _userDbContext = userDbContext;
        _logger = logger;
        _otelMetrics = otelMetrics;
    }

    public IUnitOfWork UnitOfWork => _userDbContext;


    public void UpdateBulkUserInfos(List<UserInfo> userInfos)
    {
        _userDbContext.UserInfos.UpdateRange(userInfos);
    }

    public async Task<UserInfo?> GetAsync(Guid id)
    {
        UserInfo? userInfo;
        try
        {
            userInfo = await _userDbContext.UserInfos
                .Where(u => u.Id == id)
                .Include(u => u.Devices.Where(d => d.IsActive))
                .ThenInclude(d => d.NotificationPreference)
                .Include(u => u.NotificationPreferences)
                .Include(u => u.TradingAccounts)
                .Include(u => u.CustCodes)
                .SingleOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to queries user. ID: {Id}. Exception: {E}", id,
                e.Message);
            throw;
        }

        if (userInfo == null) return null;

        await _userDbContext.Entry(userInfo)
            .Collection(c => c.Devices)
            .LoadAsync();
        await _userDbContext.Entry(userInfo)
            .Collection(c => c.NotificationPreferences)
            .LoadAsync();
        await _userDbContext.Entry(userInfo)
            .Collection(c => c.TradingAccounts)
            .LoadAsync();
        await _userDbContext.Entry(userInfo)
            .Collection(c => c.CustCodes)
            .LoadAsync();

        return userInfo;
    }

    public async Task<UserInfo?> GetAsync(string customerId)
    {
        UserInfo? userInfo;
        try
        {
            userInfo = await _userDbContext.UserInfos
                .Where(u => u.CustomerId == customerId)
                .Include(u => u.Devices.Where(d => d.IsActive))
                .ThenInclude(device => device.NotificationPreference)
                .Include(u => u.TradingAccounts)
                .Include(u => u.CustCodes)
                .SingleOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to queries user. ID: {CustomerId}. Exception: {E}", customerId,
                e.Message);
            throw;
        }

        if (userInfo == null) return null;

        await _userDbContext.Entry(userInfo)
            .Collection(c => c.Devices)
            .LoadAsync();
        await _userDbContext.Entry(userInfo)
            .Collection(c => c.TradingAccounts)
            .LoadAsync();
        await _userDbContext.Entry(userInfo)
            .Collection(c => c.CustCodes)
            .LoadAsync();

        return userInfo;
    }

    public async Task<UserInfo?> Get(Guid id, bool isTracking = false)
    {
        UserInfo? userInfo;
        try
        {
            var query = _userDbContext.UserInfos
                .Include(u => u.Devices.Where(d => d.IsActive))
                .ThenInclude(d => d.NotificationPreference)
                .Include(u => u.NotificationPreferences)
                .Include(u => u.TradingAccounts)
                .Include(u => u.CustCodes)
                .Where(u => u.Id == id);

            if (!isTracking)
            {
                query = query.AsNoTracking();
            }

            userInfo = await query.SingleOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to queries user. ID: {Id}. Exception: {E}", id,
                e.Message);
            throw;
        }

        return userInfo;
    }

    public async Task<UserInfo?> Get(string customerId)
    {
        UserInfo? userInfo;
        try
        {
            userInfo = await _userDbContext.UserInfos
                .AsNoTracking()
                .Where(u => u.CustomerId == customerId)
                .Include(u => u.Devices.Where(d => d.IsActive))
                .ThenInclude(device => device.NotificationPreference)
                .Include(u => u.TradingAccounts)
                .Include(u => u.CustCodes)
                .SingleOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to queries user. ID: {CustomerId}. Exception: {E}", customerId,
                e.Message);
            throw;
        }

        return userInfo;
    }

    public async Task<UserInfo?> GetByEmail(string email)
    {
        UserInfo? userInfo;
        try
        {
            userInfo = await _userDbContext.UserInfos
                .AsNoTracking()
                .Where(u => u.EmailHash == email)
                .Include(u => u.Devices.Where(d => d.IsActive))
                .ThenInclude(device => device.NotificationPreference)
                .Include(u => u.TradingAccounts)
                .Include(u => u.CustCodes)
                .SingleOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to queries user. Email: {Email}. Exception: {E}", email,
                e.Message);
            throw;
        }

        return userInfo;
    }

    public async Task<UserInfo?> GetByPhoneNumber(string phoneNumber)
    {
        UserInfo? userInfo;
        try
        {
            userInfo = await _userDbContext.UserInfos
                .AsNoTracking()
                .Where(u => u.PhoneNumberHash == phoneNumber)
                .Include(u => u.Devices.Where(d => d.IsActive))
                .ThenInclude(device => device.NotificationPreference)
                .Include(u => u.TradingAccounts)
                .Include(u => u.CustCodes)
                .SingleOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to queries user. Phone number: {PhoneNumber}. Exception: {E}", phoneNumber,
                e.Message);
            return null;
        }

        return userInfo;
    }

    public async Task<UserInfo?> GetByCitizenId(string citizenId)
    {
        UserInfo? userInfo;
        try
        {
            userInfo = await _userDbContext.UserInfos
                .AsNoTracking()
                .Where(u => u.CitizenIdHash == citizenId)
                .Include(u => u.Devices.Where(d => d.IsActive))
                .ThenInclude(device => device.NotificationPreference)
                .Include(u => u.TradingAccounts)
                .Include(u => u.CustCodes)
                .SingleOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to queries user. CitizenId: {CitizenId}. Exception: {E}", citizenId,
                e.Message);
            throw;
        }

        return userInfo;
    }

    public async Task<Guid?> GetUserIdByCitizenId(string citizenId)
    {
        Guid userId = await _userDbContext.UserInfos
                .AsNoTracking()
                .Where(u => u.CitizenIdHash == citizenId)
                .Select(u => u.Id)
                .SingleOrDefaultAsync();
        return userId == Guid.Empty ? null : userId;
    }

    public async Task<CustCode?> GetCustCodeAsync(string custCode)
    {
        CustCode? customerCode;
        try
        {
            customerCode = await _userDbContext.CustCodes
                .AsNoTracking()
                .Where(c => c.CustomerCode == custCode)
                .Include(u => u.UserInfo)
                .SingleOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Unable to queries user. ID: {CustCode}. Exception: {E}", custCode,
                e.Message);
            throw;
        }

        return customerCode;
    }

    public Task<CustCode?> GetCustomerCodeAsync(Guid userId)
    {
        return _userDbContext.CustCodes.SingleOrDefaultAsync(o => o.UserInfoId == userId);
    }

    public async Task<IEnumerable<UserInfo>> GetBulk(IEnumerable<Guid> ids)
    {
        IEnumerable<UserInfo> userInfos;
        try
        {
            userInfos = await _userDbContext.UserInfos
                .AsNoTracking()
                .Include(u => u.Devices.Where(d => d.IsActive))
                .ThenInclude(d => d.NotificationPreference)
                .Include(u => u.NotificationPreferences)
                .Include(u => u.TradingAccounts)
                .Include(u => u.CustCodes)
                .Where(u => ids.Contains(u.Id))
                .ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to queries user. ID: {Id}. Exception: {E}", ids,
                e.Message);
            throw;
        }

        return userInfos;
    }

    public async Task<IEnumerable<UserInfo>> GetBulk(IEnumerable<string> customerIds)
    {
        IEnumerable<UserInfo> userInfos;
        try
        {
            userInfos = await _userDbContext.UserInfos
                .AsNoTracking()
                .Where(u => customerIds.Contains(u.CustomerId))
                .Include(u => u.Devices.Where(d => d.IsActive))
                .ThenInclude(device => device.NotificationPreference)
                .Include(u => u.TradingAccounts)
                .Include(u => u.CustCodes)
                .ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to queries user. ID: {CustomerId}. Exception: {E}", customerIds,
                e.Message);
            throw;
        }

        return userInfos;
    }

    public async Task<IEnumerable<CustCode>> GetBulkCustCode(IEnumerable<string> custCodes)
    {
        IEnumerable<CustCode> customerCodes;
        try
        {
            customerCodes = await _userDbContext.CustCodes
                .AsNoTracking()
                .Include(u => u.UserInfo)
                .Where(c => custCodes.Contains(c.CustomerCode))
                .ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Unable to queries user. ID: {CustCode}. Exception: {E}", custCodes,
                e.Message);
            throw;
        }

        return customerCodes;
    }

    public async Task<Device?> GetByDevice(Guid userId, Guid deviceId)
    {
        Device? device;
        try
        {
            device = await _userDbContext.Devices
                .Where(n => n.DeviceId == deviceId && n.IsActive && n.UserInfoId == userId)
                .Include(d => d.NotificationPreference)
                .SingleOrDefaultAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Unable to queries notification reference. deviceId: {Id}. Exception: {E}",
                deviceId,
                e.Message);
            throw;
        }

        if (device == null) return null;

        await _userDbContext.Entry(device)
            .Reference(c => c.NotificationPreference)
            .LoadAsync();

        return device;
    }

    public async Task<IEnumerable<Device>?> GetByDeviceId(Guid deviceId)
    {
        List<Device>? devices;
        try
        {
            devices = await _userDbContext.Devices
                .Where(n => n.DeviceId == deviceId && n.IsActive)
                .ToListAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Unable to queriesByDeviceId. deviceId: {Id}. Exception: {E}",
                deviceId,
                e.Message);
            throw;
        }

        return !devices.Any()
            ? null
            : devices;
    }

    public async Task<IEnumerable<TradingAccountWithCustCode>?> GetTradingAccountNoListByCustomerCodeAsync(
        string customerCode, CancellationToken cancellationToken = default)
    {
        List<TradingAccountWithCustCode>? tradingAccounts;
        try
        {
            tradingAccounts = await _userDbContext.CustCodes
                .Where(r => r.CustomerCode == customerCode)
                .Join(_userDbContext.TradingAccounts, c => c.UserInfoId, t => t.UserInfoId,
                    (c, t) => new TradingAccountWithCustCode(c.CustomerCode, t.TradingAccountId))
                .ToListAsync(cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "Unable to get trading account no by customer code: {CustomerCode}",
                customerCode);
            throw;
        }

        return !tradingAccounts.Any()
            ? null
            : tradingAccounts;
    }

    public async Task<UserInfo> CreateAsync(UserInfo userInfo)
    {
        var entry = await _userDbContext.AddAsync(userInfo);
        _otelMetrics.CreateUser();
        return entry.Entity;
    }

    public int CountCustomerInfoMigration()
    {
        return _userDbContext.UserInfos.Include(x => x.CustCodes).Count(x => x.CustCodes.Count > 0);
    }

    public async Task<List<UserInfo>> GetMigrateCustomerInfo(int skip, int limit)
    {
        return await _userDbContext.UserInfos.AsNoTracking().Include(x => x.CustCodes).Where(x => x.CustCodes.Count > 0)
            .OrderBy(x => x.CreatedAt).Skip(skip).Take(limit).ToListAsync();
    }
}
