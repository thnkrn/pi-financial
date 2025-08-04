using Pi.FundMarketData.Constants;
using Pi.FundMarketData.DomainModels;

namespace Pi.FundMarketData.Test.DomainModels;

public class PerformanceTest
{
    [Fact]
    public void Should_ReturnExpected_When_Had_Past_5Years_Nav()
    {
        // Arrange
        IEnumerable<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2015, 3, 14), 4.7586M), // 10 Year
            FakeNav("KF-OIL", new DateTime(2020, 3, 14), 2.1063M), // 5 Year
            FakeNav("KF-OIL", new DateTime(2022, 3, 14), 4.6489M), // 3 Year
            FakeNav("KF-OIL", new DateTime(2024, 3, 14), 4.3798M), // 1 Year
            FakeNav("KF-OIL", new DateTime(2024, 9, 14), 3.9335M), // 6 Months
            FakeNav("KF-OIL", new DateTime(2024, 12, 14), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new Performance(navs);

        // Assert
        Assert.Null(actual.Yield1Y);
        Assert.Null(actual.AnnualizedHistoricalReturnPercentages);
        Assert.Equal(
        [
            Interval.Over3Months,
            Interval.Over6Months,
            Interval.Over1Year,
            Interval.Over3Years,
            Interval.Over5Years,
            Interval.YearToDate,
            Interval.SinceInception
        ], actual.HistoricalReturnPercentages.Select(q => q.Interval));
        Assert.Equal(
        [
            -3.58,
            1.73,
            -8.64,
            -13.93,
            89.97,
            0,
            -15.91
        ], actual.HistoricalReturnPercentages.Select(q => q.Value != null ? double.Round((double)q.Value, 2) : (double?)null));
    }

    [Fact]
    public void Should_ReturnExpected_When_Had_Past_6Months_Nav()
    {
        // Arrange
        IEnumerable<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2024, 9, 14), 3.9335M), // 6 Months
            FakeNav("KF-OIL", new DateTime(2024, 12, 14), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new Performance(navs);

        // Assert
        Assert.Null(actual.Yield1Y);
        Assert.Null(actual.AnnualizedHistoricalReturnPercentages);
        Assert.Equal(
        [
            Interval.Over3Months,
            Interval.Over6Months,
            Interval.Over1Year,
            Interval.Over3Years,
            Interval.Over5Years,
            Interval.YearToDate,
            Interval.SinceInception
        ], actual.HistoricalReturnPercentages.Select(q => q.Interval));
        Assert.Equal(
        [
            -3.58,
            1.73,
            null,
            null,
            null,
            0,
            1.73
        ], actual.HistoricalReturnPercentages.Select(q => q.Value != null ? double.Round((double)q.Value, 2) : (double?)null));
    }

    [Fact]
    public void Should_ReturnExpected_When_Had_Only_Latest_Nav()
    {
        // Arrange
        IEnumerable<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new Performance(navs);

        // Assert
        Assert.Null(actual.Yield1Y);
        Assert.Null(actual.AnnualizedHistoricalReturnPercentages);
        Assert.Equal(
        [
            Interval.Over3Months,
            Interval.Over6Months,
            Interval.Over1Year,
            Interval.Over3Years,
            Interval.Over5Years,
            Interval.YearToDate,
            Interval.SinceInception
        ], actual.HistoricalReturnPercentages.Select(q => q.Interval));
        Assert.Equal(
        [
            null,
            null,
            null,
            null,
            null,
            0,
            0
        ], actual.HistoricalReturnPercentages.Select(q => q.Value != null ? double.Round((double)q.Value, 2) : (double?)null));
    }

    [Fact]
    public void Should_ReturnExpected_When_Missing_Exact_Nav()
    {
        // Arrange
        IEnumerable<HistoricalNav> navs = [
            FakeNav("KF-OIL", new DateTime(2024, 9, 22), 3.9335M), // 6 Months
            FakeNav("KF-OIL", new DateTime(2024, 12, 18), 4.1498M), // 3 Months
            FakeNav("KF-OIL", new DateTime(2025, 3, 14), 4.0014M)
        ];

        // Act
        var actual = new Performance(navs);

        // Assert
        Assert.Null(actual.Yield1Y);
        Assert.Null(actual.AnnualizedHistoricalReturnPercentages);
        Assert.Equal(
        [
            Interval.Over3Months,
            Interval.Over6Months,
            Interval.Over1Year,
            Interval.Over3Years,
            Interval.Over5Years,
            Interval.YearToDate,
            Interval.SinceInception
        ], actual.HistoricalReturnPercentages.Select(q => q.Interval));
        Assert.Equal(
        [
            -3.58,
            null,
            null,
            null,
            null,
            (double)0,
            1.73
        ], actual.HistoricalReturnPercentages.Select(q => q.Value != null ? double.Round((double)q.Value, 2) : (double?)null));
    }

    [Fact]
    public void Should_ReturnEmpty_When_Nav_Is_Empty()
    {
        // Arrange
        IEnumerable<HistoricalNav> navs = [];

        // Act
        var actual = new Performance(navs);

        // Assert
        Assert.Null(actual.Yield1Y);
        Assert.Null(actual.AnnualizedHistoricalReturnPercentages);
        Assert.Empty(actual.HistoricalReturnPercentages);
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
