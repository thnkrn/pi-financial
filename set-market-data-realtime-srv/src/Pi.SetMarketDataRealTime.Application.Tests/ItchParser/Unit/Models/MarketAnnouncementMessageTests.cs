using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public class MarketAnnouncementMessageTests
{
    private static readonly Mock<IMemoryCacheHelper> _memoryCache = new();
    private static readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger = new();

    private readonly ItchParserService itchParserService =
        new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

    private static byte[] CreateMarketAnnouncementMessage(
        uint nanos,
        uint orderBookId,
        byte marketCode,
        byte exchangeCode,
        byte messageInformationType,
        string messageSource,
        byte messagePriority,
        string messageHeader,
        byte itemsURL,
        string documentURL,
        byte numberOfMessageItems,
        byte numberOfMessageItemsInArray,
        List<TestMessageItem> messageItems
    )
    {
        var messageBuilder = new List<byte>
        {
            (byte)ItchMessageType.N // Message type 'N'
        };

        MockMessageCreator.AddNumeric(ref messageBuilder, nanos, 4); // Nanos
        MockMessageCreator.AddNumeric(ref messageBuilder, orderBookId, 4); // Order Book ID
        MockMessageCreator.AddNumeric(ref messageBuilder, marketCode, 1); // Market Code
        MockMessageCreator.AddNumeric(ref messageBuilder, exchangeCode, 1); // Exchange Code
        MockMessageCreator.AddNumeric(ref messageBuilder, messageInformationType, 1); // Message Information Type
        MockMessageCreator.AddAlpha(ref messageBuilder, messageSource, 80); // Message Source
        MockMessageCreator.AddNumeric(ref messageBuilder, messagePriority, 1); // Message Priority
        MockMessageCreator.AddAlpha(ref messageBuilder, messageHeader, 80); // Message Header
        MockMessageCreator.AddNumeric(ref messageBuilder, itemsURL, 1); // Items URL
        MockMessageCreator.AddAlpha(ref messageBuilder, documentURL, 255); // Document URL
        MockMessageCreator.AddNumeric(ref messageBuilder, numberOfMessageItems, 1); // Number of Message Items
        MockMessageCreator.AddNumeric(ref messageBuilder, numberOfMessageItemsInArray,
            1); // Number of Message Items in Array

        for (var i = 0; i < messageItems.Count; i++)
        {
            // Add message item
            MockMessageCreator.AddAlpha(
                ref messageBuilder,
                messageItems[i].AdvertisementId,
                10
            ); // Advertisement ID
            MockMessageCreator.AddAlpha(ref messageBuilder, messageItems[i].Action, 1); // Action
            MockMessageCreator.AddAlpha(ref messageBuilder, messageItems[i].Side, 1); // Side
            MockMessageCreator.AddAlpha(ref messageBuilder, messageItems[i].Quantity, 20); // Quantity
            MockMessageCreator.AddAlpha(ref messageBuilder, messageItems[i].Price, 10); // Price
            MockMessageCreator.AddAlpha(ref messageBuilder, messageItems[i].ContactName, 10); // Contact Name
            MockMessageCreator.AddAlpha(ref messageBuilder, messageItems[i].ContactInfo, 28); // Contact Info
        }

        return [.. messageBuilder];
    }

    [Fact]
    public async Task MarketAnnouncementMessage_Constructor_SetsInputCorrectly()
    {
        // Arrange
        var input = CreateMarketAnnouncementMessage(
            123456789,
            12,
            0,
            0,
            8,
            "the market control.",
            2,
            "a message header",
            24,
            "https://www.google.co.th",
            1,
            1,
            [
                new TestMessageItem
                {
                    AdvertisementId = "159",
                    Action = "I",
                    Side = "B",
                    Quantity = "20",
                    Price = "500",
                    ContactName = "John Doe",
                    ContactInfo = "1234567890"
                }
            ]
        );

        var expectedMessageSource = "the market control.".PadRight(80); // 80 characters
        var expectedMessageHeader = "a message header".PadRight(80); // 80 characters
        var expectedDocumentURL = "https://www.google.co.th".PadRight(255); // 255 characters
        var expectedAdvertisementId = "159".PadRight(10); // 10 characters
        var expectedAction = "I"; // 1 character
        var expectedSide = "B"; // 1 character
        var expectedQuantity = "20".PadRight(20); // 20 characters
        var expectedPrice = "500".PadRight(10); // 10 characters
        var expectedContactName = "John Doe".PadRight(10); // 10 characters
        var expectedContactInfo = "1234567890".PadRight(28); // 28 characters

        // Act
        var output =
            await itchParserService.Parse(input) as MarketAnnouncementMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(expectedMessageSource, output.MessageSource);
        Assert.Equal(expectedMessageHeader, output.MessageHeader);
        Assert.Equal(expectedDocumentURL, output.DocumentURL);
        Assert.Equal(expectedAdvertisementId, output.ArrayOfMessageItems[0].AdvertisementId);
        Assert.Equal(expectedAction, output.ArrayOfMessageItems[0].Action);
        Assert.Equal(expectedSide, output.ArrayOfMessageItems[0].Side);
        Assert.Equal(expectedQuantity, output.ArrayOfMessageItems[0].Quantity);
        Assert.Equal(expectedPrice, output.ArrayOfMessageItems[0].Price);
        Assert.Equal(expectedContactName, output.ArrayOfMessageItems[0].ContactName);
        Assert.Equal(expectedContactInfo, output.ArrayOfMessageItems[0].ContactInfo);
    }

    [Fact]
    public async Task MarketAnnouncementMessage_Constructor_SetsInputCorrectlyWithArrayMessageMoreThanOne()
    {
        // Arrange
        var input = CreateMarketAnnouncementMessage(
            123456789,
            12,
            0,
            0,
            8,
            "the market control.",
            2,
            "a message header",
            24,
            "https://www.google.co.th",
            1,
            1,
            [
                new TestMessageItem
                {
                    AdvertisementId = "159",
                    Action = "I",
                    Side = "B",
                    Quantity = "20",
                    Price = "500",
                    ContactName = "John Doe",
                    ContactInfo = "1234567890"
                },
                new TestMessageItem
                {
                    AdvertisementId = "200",
                    Action = "I",
                    Side = "S",
                    Quantity = "20",
                    Price = "500",
                    ContactName = "John Doe",
                    ContactInfo = "1234567890"
                }
            ]
        );

        var expectedMessageSource = "the market control.".PadRight(80); // 80 characters
        var expectedMessageHeader = "a message header".PadRight(80); // 80 characters
        var expectedDocumentURL = "https://www.google.co.th".PadRight(255); // 255 characters
        var expectedAdvertisementId = "159".PadRight(10); // 10 characters

        var expectedAction = "I"; // 1 character
        var expectedSide = "B"; // 1 character
        var expectedQuantity = "20".PadRight(20); // 20 characters
        var expectedPrice = "500".PadRight(10); // 10 characters
        var expectedContactName = "John Doe".PadRight(10); // 10 characters
        var expectedContactInfo = "1234567890".PadRight(28); // 28 characters

        var expectedAdvertisementId2 = "200".PadRight(10); // 10 characters
        var expectedAction2 = "I"; // 1 character
        var expectedSide2 = "S"; // 1 character
        var expectedQuantity2 = "20".PadRight(20); // 20 characters
        var expectedPrice2 = "500".PadRight(10); // 10 characters
        var expectedContactName2 = "John Doe".PadRight(10); // 10 characters
        var expectedContactInfo2 = "1234567890".PadRight(28); // 28 characters

        // Act
        var output =
            await itchParserService.Parse(input) as MarketAnnouncementMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(expectedMessageSource, output.MessageSource);
        Assert.Equal(expectedMessageHeader, output.MessageHeader);
        Assert.Equal(expectedDocumentURL, output.DocumentURL);
        // First message item
        Assert.Equal(expectedAdvertisementId, output.ArrayOfMessageItems[0].AdvertisementId);
        Assert.Equal(expectedAction, output.ArrayOfMessageItems[0].Action);
        Assert.Equal(expectedSide, output.ArrayOfMessageItems[0].Side);
        Assert.Equal(expectedQuantity, output.ArrayOfMessageItems[0].Quantity);
        Assert.Equal(expectedPrice, output.ArrayOfMessageItems[0].Price);
        Assert.Equal(expectedContactName, output.ArrayOfMessageItems[0].ContactName);
        Assert.Equal(expectedContactInfo, output.ArrayOfMessageItems[0].ContactInfo);
        // Second message item
        Assert.Equal(expectedAdvertisementId2, output.ArrayOfMessageItems[1].AdvertisementId);
        Assert.Equal(expectedAction2, output.ArrayOfMessageItems[1].Action);
        Assert.Equal(expectedSide2, output.ArrayOfMessageItems[1].Side);
        Assert.Equal(expectedQuantity2, output.ArrayOfMessageItems[1].Quantity);
        Assert.Equal(expectedPrice2, output.ArrayOfMessageItems[1].Price);
        Assert.Equal(expectedContactName2, output.ArrayOfMessageItems[1].ContactName);
        Assert.Equal(expectedContactInfo2, output.ArrayOfMessageItems[1].ContactInfo);
    }

    [Fact]
    public async Task MarketAnnouncementMessage_Constructor_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.N, 0, 0, 0, 1];

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
    }

    private class TestMessageItem
    {
        public required string AdvertisementId { get; set; }
        public required string Action { get; set; }
        public required string Side { get; set; }
        public required string Quantity { get; set; }
        public required string Price { get; set; }
        public required string ContactName { get; set; }
        public required string ContactInfo { get; set; }
    }
}