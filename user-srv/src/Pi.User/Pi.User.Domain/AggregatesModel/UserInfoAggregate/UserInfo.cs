using Pi.Common.SeedWork;
using Pi.User.Domain.AggregatesModel.AddressAggregate;
using Pi.User.Domain.AggregatesModel.BankAccountAggregate;
using Pi.User.Domain.AggregatesModel.DocumentAggregate;
using Pi.User.Domain.AggregatesModel.ExamAggregate;
using Pi.User.Domain.AggregatesModel.KycAggregate;
using Pi.User.Domain.AggregatesModel.SuitabilityTestAggregate;
using Pi.User.Domain.AggregatesModel.TradeAccountAggregate;
using Pi.User.Domain.AggregatesModel.UserAccountAggregate;

namespace Pi.User.Domain.AggregatesModel.UserInfoAggregate;

public record UserPersonalInfo(
    string? CitizenId = null,
    string? FirstnameTh = null,
    string? LastnameTh = null,
    string? FirstnameEn = null,
    string? LastnameEn = null,
    string? PhoneNumber = null,
    string? Email = null,
    string? PlaceOfBirthCountry = null,
    string? PlaceOfBirthCity = null,
    DateOnly? DateOfBirth = null
);

public class UserInfo : Entity<Guid>, IAggregateRoot, IAuditableEntity
{
    private readonly List<CustCode> _custCodes;
    private readonly List<Device> _devices;
    private readonly List<NotificationPreference> _notificationPreferences;
    private readonly List<TradingAccount> _tradingAccounts;

    // For EF, only include non-nullable fields
    private UserInfo(
        Guid id,
        string customerId
    ) : base(id)
    {
        CustomerId = customerId;
        _custCodes = new List<CustCode>();
        _tradingAccounts = new List<TradingAccount>();
        _devices = new List<Device>();
        _notificationPreferences = new List<NotificationPreference>();
    }

    public UserInfo(
        Guid id,
        string customerId,
        string? globalAccount = null,
        UserPersonalInfo? userPersonalInfo = null,
        string? wealthType = null
    ) : base(id)
    {
        CustomerId = customerId;
        _custCodes = new List<CustCode>();
        _tradingAccounts = new List<TradingAccount>();
        _devices = new List<Device>();
        _notificationPreferences = new List<NotificationPreference>();
        GlobalAccount = globalAccount;
        CitizenId = userPersonalInfo?.CitizenId;
        FirstnameTh = userPersonalInfo?.FirstnameTh;
        LastnameTh = userPersonalInfo?.LastnameTh;
        FirstnameEn = userPersonalInfo?.FirstnameEn;
        LastnameEn = userPersonalInfo?.LastnameEn;
        PhoneNumber = userPersonalInfo?.PhoneNumber;
        Email = userPersonalInfo?.Email;
        PlaceOfBirthCountry = userPersonalInfo?.PlaceOfBirthCountry;
        PlaceOfBirthCity = userPersonalInfo?.PlaceOfBirthCity;
        DateOfBirth = userPersonalInfo?.DateOfBirth?.ToString("yyyy-MM-dd");
        WealthType = wealthType;
    }

    public string CustomerId { get; private set; }
    public IReadOnlyCollection<CustCode> CustCodes => _custCodes;
    public IReadOnlyCollection<TradingAccount> TradingAccounts => _tradingAccounts;
    public IReadOnlyCollection<Device> Devices => _devices;
    public IReadOnlyCollection<NotificationPreference> NotificationPreferences => _notificationPreferences;
    public string? FirstnameTh { get; private set; }
    public string? LastnameTh { get; private set; }
    public string? FirstnameEn { get; private set; }
    public string? LastnameEn { get; private set; }
    public string? CitizenId { get; private set; }
    public string? CitizenIdHash { get; set; }
    public string? PhoneNumber { get; private set; }
    public string? PhoneNumberHash { get; private set; }
    public string? Email { get; private set; }
    public string? EmailHash { get; private set; }
    public string? GlobalAccount { get; private set; }
    public string? PlaceOfBirthCountry { get; private set; }
    public string? PlaceOfBirthCity { get; private set; }
    // public string? DateOfBirth { get; private set; }
    public string? DateOfBirth { get; private set; }

    public string? WealthType { get; private set; }

    public ICollection<BankAccount> BankAccounts { get; } = new List<BankAccount>();
    public ICollection<Document> Documents { get; } = new List<Document>();
    public ICollection<Examination> Examinations { get; } = new List<Examination>();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }


    // New Structure
    public ICollection<UserAccount> UserAccounts { get; } = new List<UserAccount>();
    public Kyc? Kyc { get; set; }
    public Address? Address { get; set; }
    public ICollection<SuitabilityTest> SuitabilityTests { get; } = new List<SuitabilityTest>();
    public ICollection<BankAccountV2> BankAccountsV2 { get; } = new List<BankAccountV2>();

    public void AddCustCodes(IEnumerable<string> custCodes)
    {
        _custCodes.AddRange(custCodes.Select(c => new CustCode(c)).ToList());
    }

    public void AddTradingAccounts(IEnumerable<TradingAccount> tradingAccounts)
    {
        _tradingAccounts.AddRange(tradingAccounts);
    }

    public void AddDevice(Guid deviceId, string deviceToken, string language, string? deviceIdentifier, string platform,
        string? subscriptionIdentifier)
    {
        if (_devices.SingleOrDefault(d => d.DeviceId == deviceId && d.IsActive) != null) return;

        if (_devices.Count(d => d.IsActive) >= 5)
        {
            var removeItem = _devices.Where(d => d.IsActive).OrderBy(d => d.CreatedAt).First();
            _devices.Find(d => d.Id == removeItem.Id)?.MarkInactive();
        }

        _devices.Add(new Device(deviceId, deviceToken, language, deviceIdentifier ?? string.Empty, platform,
            subscriptionIdentifier ?? string.Empty));
    }

    public void UpdateCustCode(string newCustCode)
    {
        if (_custCodes.Any(c => c.CustomerCode == newCustCode)) return;

        _custCodes.Add(new CustCode(newCustCode));
    }

    public void UpdateTradingAccount(string newTradingAccount, string newAcctCode)
    {
        if (_tradingAccounts.Any(t => t.TradingAccountId == newTradingAccount && t.AcctCode == newAcctCode)) return;

        _tradingAccounts.Add(new TradingAccount(newTradingAccount, newAcctCode));
    }

    public void UpdateDevice(Guid newDeviceId, string newDeviceToken, string newLanguage, string platform)
    {
        var existingDevice = _devices.SingleOrDefault(d => d.DeviceId == newDeviceId && d.IsActive);

        if (existingDevice == null)
        {
            AddDevice(newDeviceId, newDeviceToken, newLanguage, string.Empty, platform, string.Empty);
            return;
        }

        if (!string.IsNullOrWhiteSpace(newDeviceToken) && existingDevice.DeviceToken != newDeviceToken)
        {
            existingDevice.UpdateDeviceToken(newDeviceToken);
            existingDevice.UpdateDeviceIdentifier(string.Empty);
        }

        if (!string.IsNullOrWhiteSpace(newLanguage) && newLanguage != existingDevice.Language)
            _devices
                .Where(d => d.DeviceId == newDeviceId)
                .ToList()
                .ForEach(d => d.UpdateLanguage(string.Empty));
    }

    public void RemoveDuplicateDevice()
    {
        var devices = _devices.Where(d => d.IsActive).DistinctBy(d => d.DeviceId);

        _devices.Clear();

        foreach (var device in devices) _devices.Add(device);
    }

    public UserInfo UpdateCitizenId(string? citizenId)
    {
        CitizenId = citizenId;
        return this;
    }

    public UserInfo UpdateCitizenIdHash(string? hashedCitizenId)
    {
        CitizenIdHash = hashedCitizenId;
        return this;
    }

    public UserInfo UpdateNameTh(string firstnameTh, string lastnameTh)
    {
        FirstnameTh = firstnameTh;
        LastnameTh = lastnameTh;

        return this;
    }

    public UserInfo UpdateNameEn(string firstnameEn, string lastnameEn)
    {
        FirstnameEn = firstnameEn;
        LastnameEn = lastnameEn;

        return this;
    }

    public UserInfo UpdatePhoneNumber(string? phoneNumber)
    {
        PhoneNumber = phoneNumber;
        return this;
    }

    public UserInfo UpdatePhoneNumberHash(string? hashPhoneNumber)
    {
        PhoneNumberHash = hashPhoneNumber;
        return this;
    }

    public UserInfo UpdateEmail(string? email)
    {
        Email = email;
        return this;
    }

    public UserInfo UpdateEmailHash(string? hashedEmail)
    {
        EmailHash = hashedEmail;
        return this;
    }

    public UserInfo UpdateGlobalAccount(string? globalAccount)
    {
        GlobalAccount = globalAccount;
        return this;
    }

    public UserInfo UpdatePlaceOfBirth(string placeOfBirthCountry, string placeOfBirthCity)
    {
        PlaceOfBirthCountry = placeOfBirthCountry;
        PlaceOfBirthCity = placeOfBirthCity;
        return this;
    }

    public UserInfo UpdateDateOfBirth(string? dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
        return this;
    }

    public UserInfo UpdateWealthType(string? wealthType)
    {
        WealthType = wealthType;
        return this;
    }
}