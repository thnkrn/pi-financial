namespace Pi.GlobalMarketData.DataMigrationJobProducer.Tests;

public class TryParseArgsTests
{
    [Fact]
    public void TryParseArgs_ValidInput_ReturnsTrue()
    {
        // Arrange
        string[] args = { "2024-03-15", "2024-03-20", "NASDAQ", "AAPL,GOOGL,MSFT" };

        // Act
        bool result = Program.TryParseArgs(args, out DateTime migrationDateFrom, out DateTime migrationDateTo, out string venue, out string[] stockSymbols);
        // Assert
        Assert.True(result);
        Assert.Equal(new DateTime(2024, 3, 15, 0, 0, 0, DateTimeKind.Utc), migrationDateFrom.ToUniversalTime());
        Assert.Equal(new DateTime(2024, 3, 20, 0, 0, 0, DateTimeKind.Utc), migrationDateTo.ToUniversalTime());
        Assert.Equal("NASDAQ", venue);
        Assert.Equal(["AAPL", "GOOGL", "MSFT"], stockSymbols);
    }

    [Theory]
    [InlineData("15-03-2024", "2024-03-20", "Equity", "AAPL,GOOGL,MSFT")]
    [InlineData("2024-03-15", "20-03-2024", "Equity", "AAPL,GOOGL,MSFT")]
    [InlineData("2024/03/15", "2024/03/20", "Derivative", "AAPL,GOOGL,MSFT")]
    [InlineData("2024.03.15", "2024.03.20", "Derivative", "AAPL,GOOGL,MSFT")]
    [InlineData("15/03/2024", "20/03/2024", "Derivative", "AAPL,GOOGL,MSFT")]
    public void TryParseArgs_InvalidDateFormatOrVenue_ReturnsFalse(string dateFrom, string dateTo, string venue, string symbols)
    {
        // Arrange
        string[] args = { dateFrom, dateTo, venue, symbols };

        // Act
        bool result = Program.TryParseArgs(args, out _, out _, out _, out _);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryParseArgs_NoStockSymbols_ReturnsFalse()
    {
        // Arrange
        string[] args = { "2024-03-15", "2024-03-20", "NASDAQ", "" };

        // Act
        bool result = Program.TryParseArgs(args, out _, out _, out _, out _);

        // Assert
        Assert.False(result);
    }
}