using Newtonsoft.Json;
using Pi.MarketData.Application.Services;
using Pi.MarketData.Domain.Models;

namespace Pi.MarketData.Application.Tests.Services;

#pragma warning disable CS8625
public class CuratedFilterServiceTests
{
    private readonly string _testFilePath = Path.GetTempFileName();

    public CuratedFilterServiceTests()
    {
        // Set up test data
        var testData = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "Thai Equities", new Dictionary<string, string>
                {
                    { "Stocks", "SET" },
                    { "Derivative Warrants", "SET" },
                    { "PF & REIT", "SET" },
                    { "Depositary Receipts", "SET" },
                    { "ETFs", "SET" },
                    { "Warrants", "SET" }
                }
            },
            {
                "Global Equities", new Dictionary<string, string>
                {
                    { "Stocks", "GE" },
                    { "ETFs", "GE" },
                    { "S&P 500", "GE" },
                    { "NASDAQ", "GE" },
                    { "Hong Kong", "GE" }
                }
            },
            {
                "Derivatives", new Dictionary<string, string>
                {
                    { "S50 Futures", "SET" },
                    { "S50 Options", "SET" },
                    { "Sectors", "SET" },
                    { "Stocks", "SET" },
                    { "Metals", "SET" },
                    { "Currency", "SET" }
                }
            }
        };

        File.WriteAllText(_testFilePath, JsonConvert.SerializeObject(testData));
    }

    [Fact]
    public void Constructor_ValidFilePath_CreatesInstance()
    {
        var service = new CuratedFilterService(_testFilePath);
        Assert.NotNull(service);
    }

    [Fact]
    public void Constructor_NullFilePath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CuratedFilterService(null));
    }

    [Fact]
    public void Constructor_EmptyFilePath_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new CuratedFilterService(string.Empty));
    }

    [Fact]
    public void Constructor_NonExistentFilePath_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() => new CuratedFilterService("nonexistent.json"));
    }

    [Theory]
    [InlineData("Thai Equities", "Stocks", "SET")]
    [InlineData("Thai Equities", "ETFs", "SET")]
    [InlineData("Global Equities", "Stocks", "GE")]
    [InlineData("Global Equities", "NASDAQ", "GE")]
    [InlineData("Derivatives", "S50 Futures", "SET")]
    [InlineData("Derivatives", "Currency", "SET")]
    public void GetDomain_ValidInput_ReturnsDomain(string groupName, string subGroupName, string expectedDomain)
    {
        var service = new CuratedFilterService(_testFilePath);
        var payload = new FiltersRequestPayload { GroupName = groupName, SubGroupName = subGroupName };

        var result = service.GetDomain(payload);

        Assert.Equal(expectedDomain, result);
    }

    [Fact]
    public void GetDomain_InvalidGroup_ReturnsNull()
    {
        var service = new CuratedFilterService(_testFilePath);
        var payload = new FiltersRequestPayload { GroupName = "InvalidGroup", SubGroupName = "SubGroup1" };

        var result = service.GetDomain(payload);

        Assert.Null(result);
    }

    [Fact]
    public void GetDomain_InvalidSubGroup_ReturnsNull()
    {
        var service = new CuratedFilterService(_testFilePath);
        var payload = new FiltersRequestPayload { GroupName = "Thai Equities", SubGroupName = "InvalidSubGroup" };

        var result = service.GetDomain(payload);

        Assert.Null(result);
    }

    [Fact]
    public void GetDomain_NullPayload_ThrowsArgumentNullException()
    {
        var service = new CuratedFilterService(_testFilePath);
        Assert.Throws<ArgumentNullException>(() => service.GetDomain(new FiltersRequestPayload()));
    }

    [Fact]
    public void GetDomain_NullGroupName_ThrowsArgumentNullException()
    {
        var service = new CuratedFilterService(_testFilePath);
        var payload = new FiltersRequestPayload { GroupName = null, SubGroupName = "Stock" };

        Assert.Throws<ArgumentNullException>(() => service.GetDomain(payload));
    }

    [Fact]
    public void GetDomain_NullSubGroupName_ThrowsArgumentNullException()
    {
        var service = new CuratedFilterService(_testFilePath);
        var payload = new FiltersRequestPayload { GroupName = "Thai Equities", SubGroupName = null };

        Assert.Throws<ArgumentNullException>(() => service.GetDomain(payload));
    }

    internal void Dispose()
    {
        File.Delete(_testFilePath);
    }
}