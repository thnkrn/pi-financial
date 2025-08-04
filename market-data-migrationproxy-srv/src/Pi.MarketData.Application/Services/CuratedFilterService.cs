using Newtonsoft.Json;
using Pi.MarketData.Domain.Models;
using Pi.MarketData.Application.Interfaces;
using Pi.MarketData.Domain.Models.Request;

namespace Pi.MarketData.Application.Services;

public class CuratedFilterService : ICuratedFilterService
{
    private readonly Dictionary<string, Dictionary<string, string>> _curatedFilterSetting;

    public CuratedFilterService(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The provided file path does not exist.", filePath);
        }

        var jsonString = File.ReadAllText(filePath);

        _curatedFilterSetting = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonString)
            ?? throw new InvalidOperationException("Failed to deserialize the JSON file.");
    }

    public string? GetDomain(FiltersRequestPayload requestPayload)
    {
        ArgumentNullException.ThrowIfNull(requestPayload);
        ArgumentNullException.ThrowIfNull(requestPayload.GroupName);
        ArgumentNullException.ThrowIfNull(requestPayload.SubGroupName);

        if (requestPayload.GroupName == "Funds")
        {
            return "Fund";
        }

        if (_curatedFilterSetting.TryGetValue(requestPayload.GroupName, out var subGroupDict) &&
            subGroupDict.TryGetValue(requestPayload.SubGroupName, out var domain))
        {
            return domain;
        }

        return null;
    }

    public string? GetDomain(HomeInstrumentPayload requestPayload)
    {
        ArgumentNullException.ThrowIfNull(requestPayload);
        ArgumentNullException.ThrowIfNull(requestPayload.ListName);
        ArgumentNullException.ThrowIfNull(requestPayload.RelevantTo);

        if (_curatedFilterSetting.TryGetValue(requestPayload.RelevantTo, out var subGroupDict))
        {
            return
                subGroupDict.TryGetValue(requestPayload.ListName, out var domain) ? domain :
                subGroupDict.TryGetValue("Default", out domain) ? domain :
                "Both";
        }

        return "Both";
    }
    public string GetDomain(MarketFiltersRequest requestPayload)
    {
        ArgumentNullException.ThrowIfNull(requestPayload);
        ArgumentNullException.ThrowIfNull(requestPayload.GroupName);
        ArgumentNullException.ThrowIfNull(requestPayload.SubGroupName);

        if(string.Equals(requestPayload.GroupName, "Fund"))
        {
            return "Fund";
        }
        if (_curatedFilterSetting.TryGetValue(requestPayload.GroupName, out var subGroupDict) &&
            subGroupDict.TryGetValue(requestPayload.SubGroupName, out var domain))
        {
            return domain;
        }
        return string.Empty;
    }
}