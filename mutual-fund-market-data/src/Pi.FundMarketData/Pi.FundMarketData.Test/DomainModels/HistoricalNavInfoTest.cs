using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Test.DomainModels;

public class HistoricalNavInfoTest
{
    [Fact]
    public void Should_ReturnExpected_When_Had_Past_5Years_Nav()
    {
        // Arrange
        List<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2020, 3, 14), 2.1063M), // 5 Year
            FakeNav("KF-OIL", new DateTime(2022, 3, 14), 4.6489M), // 3 Year
            FakeNav("KF-OIL", new DateTime(2024, 3, 14), 4.3798M), // 1 Year
            FakeNav("KF-OIL", new DateTime(2024, 9, 14), 3.9335M), // 6 Months
            FakeNav("KF-OIL", new DateTime(2024, 12, 14), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new HistoricalNavInfo(navs, Interval.Over5Years);

        // Assert
        Assert.Equal(1.8951m, actual.NavChange);
        Assert.Equal(89.9729, double.Round(actual.NavChangePercentage!.Value, 4));
        Assert.Equal(navs, actual.NavList);
    }

    [Fact]
    public void Should_ReturnExpected_When_Had_Past_3Years_Nav()
    {
        // Arrange
        List<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2022, 3, 14), 4.6489M), // 3 Year
            FakeNav("KF-OIL", new DateTime(2024, 3, 14), 4.3798M), // 1 Year
            FakeNav("KF-OIL", new DateTime(2024, 9, 14), 3.9335M), // 6 Months
            FakeNav("KF-OIL", new DateTime(2024, 12, 14), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new HistoricalNavInfo(navs, Interval.Over3Years);

        // Assert
        Assert.Equal(-0.6475m, actual.NavChange);
        Assert.Equal(-13.9280, double.Round(actual.NavChangePercentage!.Value, 4));
        Assert.Equal(navs, actual.NavList);
    }

    [Fact]
    public void Should_ReturnExpected_When_Had_Past_1Years_Nav()
    {
        // Arrange
        List<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2024, 3, 14), 2.1063M), // 1 Year
            FakeNav("KF-OIL", new DateTime(2024, 9, 14), 3.9335M), // 6 Months
            FakeNav("KF-OIL", new DateTime(2024, 12, 14), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new HistoricalNavInfo(navs, Interval.Over1Year);

        // Assert
        Assert.Equal(1.8951m, actual.NavChange);
        Assert.Equal(89.9729, double.Round(actual.NavChangePercentage!.Value, 4));
        Assert.Equal(navs, actual.NavList);
    }

    [Fact]
    public void Should_ReturnExpected_When_Had_Past_6Months_Nav()
    {
        // Arrange
        List<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2024, 9, 14), 3.9335M), // 6 Months
            FakeNav("KF-OIL", new DateTime(2024, 12, 14), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new HistoricalNavInfo(navs, Interval.Over6Months);

        // Assert
        Assert.Equal(0.0679m, actual.NavChange);
        Assert.Equal(1.7262, double.Round(actual.NavChangePercentage!.Value, 4));
        Assert.Equal(navs, actual.NavList);
    }

    [Fact]
    public void Should_ReturnExpected_When_Had_Past_3Months_Nav()
    {
        // Arrange
        List<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2024, 12, 14), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new HistoricalNavInfo(navs, Interval.Over3Months);

        // Assert
        Assert.Equal(-0.1484m, actual.NavChange);
        Assert.Equal(-3.5761, double.Round(actual.NavChangePercentage!.Value, 4));
        Assert.Equal(navs, actual.NavList);
    }

    [Fact]
    public void Should_ReturnExpected_When_Had_Only_Latest_Nav()
    {
        // Arrange
        List<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new HistoricalNavInfo(navs, Interval.Over5Years);

        // Assert
        Assert.Null(actual.NavChange);
        Assert.Null(actual.NavChangePercentage);
        Assert.Empty(actual.NavList);
    }

    [Fact]
    public void Should_ReturnExpected_When_Had_SinceInception_Nav()
    {
        // Arrange
        List<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2015, 3, 14), 1M), // 10 Year
            FakeNav("KF-OIL", new DateTime(2020, 3, 14), 2.1063M), // 5 Year
            FakeNav("KF-OIL", new DateTime(2022, 3, 14), 4.6489M), // 3 Year
            FakeNav("KF-OIL", new DateTime(2024, 3, 14), 4.3798M), // 1 Year
            FakeNav("KF-OIL", new DateTime(2024, 9, 14), 3.9335M), // 6 Months
            FakeNav("KF-OIL", new DateTime(2024, 12, 14), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new HistoricalNavInfo(navs, Interval.SinceInception);

        // Assert
        Assert.Equal(3.0014m, actual.NavChange);
        Assert.Equal(300.14, double.Round(actual.NavChangePercentage!.Value, 4));
        Assert.Equal(navs, actual.NavList);
    }

    [Fact]
    public void Should_ReturnExpected_When_Had_Ytd_Nav()
    {
        // Arrange
        List<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2025, 1, 1), 4.3798M), // begining of the year
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new HistoricalNavInfo(navs, Interval.YearToDate);

        // Assert
        Assert.Equal(-0.3784m, actual.NavChange);
        Assert.Equal(-8.6397, double.Round(actual.NavChangePercentage!.Value, 4));
        Assert.Equal(navs, actual.NavList);
    }

    [Fact]
    public void Should_ReturnExpected_When_Missing_Exact_Nav()
    {
        // Arrange
        List<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2024, 9, 22), 3.9335M), // 6 Months
            FakeNav("KF-OIL", new DateTime(2024, 12, 18), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new HistoricalNavInfo(navs, Interval.Over1Year);

        // Assert
        Assert.Null(actual.NavChange);
        Assert.Null(actual.NavChangePercentage);
        Assert.Empty(actual.NavList);
    }

    [Fact]
    public void Should_ThrowException_When_Nav_Is_Null()
    {
        // Arrange
        List<HistoricalNav>? navs = null;

        // Act
        var act = () => new HistoricalNavInfo(navs, Interval.Over5Years);

        // Assert
        Assert.Throws<ArgumentNullException>(act);
    }

    [Fact]
    public void Should_ThrowException_When_Nav_Is_Empty()
    {
        // Arrange
        List<HistoricalNav> navs = [];

        // Act
        var act = () => new HistoricalNavInfo(navs, Interval.Over5Years);

        // Assert
        Assert.Throws<ArgumentException>(act);
    }

    [Fact]
    public void Should_ReturnExpected_When_Had_Past_1Years_Nav_With_FallBackPrice()
    {
        // Arrange
        List<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2024, 3, 17), 2.1061M), // 1 Year - 3 days
            FakeNav("KF-OIL", new DateTime(2024, 9, 14), 3.9335M), // 6 Months
            FakeNav("KF-OIL", new DateTime(2024, 12, 14), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new HistoricalNavInfo(navs, Interval.Over1Year);

        // Assert
        Assert.Equal(1.8953m, actual.NavChange);
        Assert.Equal(89.991, double.Round(actual.NavChangePercentage!.Value, 4));
        Assert.Equal(navs, actual.NavList);
    }

    [Fact]
    public void Should_ReturnExpected_When_Had_Past_1Years_Nav_With_Exceed_FallBackPrice()
    {
        // Arrange
        List<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2024, 3, 06), 2.1009M), // 1 Year - 8 days
            FakeNav("KF-OIL", new DateTime(2024, 9, 14), 3.9335M), // 6 Months
            FakeNav("KF-OIL", new DateTime(2024, 12, 14), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new HistoricalNavInfo(navs, Interval.Over1Year);

        // Assert
        Assert.Null(actual.NavChange);
        Assert.Null(actual.NavChangePercentage);
        Assert.Empty(actual.NavList);
    }

    [Fact]
    public void Should_ReturnExpected_When_Had_Past_1Years_Nav_With_Previous_Price_Is_Zero()
    {
        // Arrange
        List<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2024, 3, 14), 0M), // 1 Year
            FakeNav("KF-OIL", new DateTime(2024, 9, 14), 3.9335M), // 6 Months
            FakeNav("KF-OIL", new DateTime(2024, 12, 14), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new HistoricalNavInfo(navs, Interval.Over1Year);

        // Assert
        Assert.Equal(4.0014m, actual.NavChange);
        Assert.Equal(0, double.Round(actual.NavChangePercentage!.Value, 4));
        Assert.Equal(navs, actual.NavList);
    }

    private static HistoricalNav FakeNav(string symbol = "KF-OIL", DateTime date = default, decimal nav = 0)
    {
        return new HistoricalNav()
        {
            Symbol = symbol,
            Date = date,
            Nav = nav,
        };
    }
}
