using Microsoft.Extensions.Logging;
using Pi.SetMarketDataRealTime.Application.Interfaces.MemoryCache;
using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Tests.ItchParser.Unit.Models;

public class NewsMessageTests
{
    private const uint nanos = 148245017;
    private const string reportType = "NWS";
    private const string marketId = "A";
    private const string language = "E";
    private const string senderName = "SET News";
    private const long publishDate = unchecked((long)16311654760000000000 + long.MinValue);
    private const string publishedDate = "2024-08-01 00:00:00";
    private const string expectedPublishedDate = "2024-08-01 00:00:00.000000000 UTC";
    private const string newsType = "41";

    private const string headline =
        "Notification of Book Closed Date and Trading Suspension of DWs issued by SET News";

    private const string newsFileName = "nws/en/txt/0171NWS090920211231170629E.txt";
    private const string PDFFileName = "nws/en/pdf/0171NWS090920211231170629E.pdf";
    private const string XMLFileName = "";

    private const string period = "";

    //private const uint dateAsOf = 0;
    private const string dateAsOf = "20240801";
    private const uint quarter = 0;
    private const string finStmtType = "";
    private const string finStmtStatus = "";
    private const string status = "N";
    private const string newsId = "6840940";
    private const string newsGroup = "684094";

    private static readonly Mock<IMemoryCacheHelper> _memoryCache = new();
    private static readonly Mock<ILogger<ItchMessageMetadataHandler>> _memoryLogger = new();

    private readonly ItchParserService itchParserService =
        new(new ItchMessageMetadataHandler(_memoryCache.Object, _memoryLogger.Object));

    private static byte[] CreateNewsMessage(
        uint numberOfTemplateCodes,
        List<string> templateCodes,
        uint numberOfRelatedSymbols,
        List<string> relatedSymbols
    )
    {
        var messageBuilder = new List<byte>
        {
            (byte)ItchMessageType.j // Message type 'j'
        };

        MockMessageCreator.AddNumeric(ref messageBuilder, nanos, 4); // Nanos
        MockMessageCreator.AddAlpha(ref messageBuilder, reportType, 10); // Report Type
        MockMessageCreator.AddAlpha(ref messageBuilder, marketId, 1); // Market ID
        MockMessageCreator.AddAlpha(ref messageBuilder, language, 1); // Language
        MockMessageCreator.AddAlpha(ref messageBuilder, senderName, 20); // Sender Name
        MockMessageCreator.AddTimestamp(ref messageBuilder, publishedDate); // Publish Date
        MockMessageCreator.AddAlpha(ref messageBuilder, newsType, 2); // News Type
        MockMessageCreator.AddAlpha(ref messageBuilder, headline, 250); // Headline
        MockMessageCreator.AddAlpha(ref messageBuilder, newsFileName, 50); // News File Name
        MockMessageCreator.AddAlpha(ref messageBuilder, PDFFileName, 50); // PDF File Name
        MockMessageCreator.AddAlpha(ref messageBuilder, XMLFileName, 50); // XML File Name
        MockMessageCreator.AddAlpha(ref messageBuilder, period, 24); // Period
        MockMessageCreator.AddDate(ref messageBuilder, dateAsOf); // Date As Of
        MockMessageCreator.AddNumeric(ref messageBuilder, quarter, 1); // Quarter
        MockMessageCreator.AddAlpha(ref messageBuilder, finStmtType, 1); // Fin Stmt Type
        MockMessageCreator.AddAlpha(ref messageBuilder, finStmtStatus, 1); // Fin Stmt Status
        MockMessageCreator.AddAlpha(ref messageBuilder, status, 1); // Status
        MockMessageCreator.AddAlpha(ref messageBuilder, newsId, 20); // News ID
        MockMessageCreator.AddAlpha(ref messageBuilder, newsGroup, 20); // News Group
        MockMessageCreator.AddNumeric(ref messageBuilder, numberOfTemplateCodes, 1); // Number Of Template Codes

        MockMessageCreator.AddNumeric(ref messageBuilder, numberOfTemplateCodes, 1); // Number Of Template Codes
        foreach (var templateCode in
                 templateCodes) MockMessageCreator.AddAlpha(ref messageBuilder, templateCode, 2); // Template Code

        MockMessageCreator.AddNumeric(ref messageBuilder, numberOfRelatedSymbols, 1); // Number Of Related Symbols

        MockMessageCreator.AddNumeric(ref messageBuilder, numberOfRelatedSymbols, 1); // Number Of Related Symbols
        foreach (var relatedSymbol in relatedSymbols)
            MockMessageCreator.AddAlpha(ref messageBuilder, relatedSymbol, 20); // Related Symbol

        return [.. messageBuilder];
    }

    [Fact]
    public async Task NewsMessage_Constructor_SetsInputCorrectly()
    {
        // Arrange
        uint numberOfTemplateCodes = 1;
        uint numberOfRelatedSymbols = 1;
        List<string> templateCodes = ["90"];
        List<string> relatedSymbols = ["ACE13C2109A"];

        var input = CreateNewsMessage(
            numberOfTemplateCodes,
            templateCodes,
            numberOfRelatedSymbols,
            relatedSymbols
        );

        // Act
        var output = await itchParserService.Parse(input) as NewsMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(nanos, output.Nanos);
        Assert.Equal(reportType.PadRight(10), output.ReportType);
        Assert.Equal(marketId.PadRight(1), output.MarketId);
        Assert.Equal(language.PadRight(1), output.Language);
        Assert.Equal(senderName.PadRight(20), output.SenderName);
        //Assert.Equal(publichedDate, output.PublishDate);
        Assert.Equal(newsType.PadRight(2), output.NewsType);
        Assert.Equal(headline.PadRight(250), output.Headline);
        Assert.Equal(newsFileName.PadRight(50), output.NewsFileName);
        Assert.Equal(PDFFileName.PadRight(50), output.PDFFileName);
        Assert.Equal(XMLFileName.PadRight(50), output.XMLFileName);
        Assert.Equal(period.PadRight(24), output.Period);
        Assert.Equal(new Date(dateAsOf), output.DateAsOf);
        Assert.Equal(quarter, output.Quarter);
        Assert.Equal(finStmtType.PadRight(1), output.FinStmtType);
        Assert.Equal(finStmtStatus.PadRight(1), output.FinStmtStatus);
        Assert.Equal(status.PadRight(1), output.Status);
        Assert.Equal(newsId.PadRight(20), output.NewsId);
        Assert.Equal(newsGroup.PadRight(20), output.NewsGroup);
        Assert.Equal(numberOfTemplateCodes, output.NumberOfTemplateCodes);
        Assert.Equal(templateCodes[0], output.TemplateCodes[0].Data);
        Assert.Equal(numberOfRelatedSymbols, output.NumberOfRelatedSymbols);
        Assert.Equal(relatedSymbols[0].PadRight(20), output.RelatedSymbols[0].Data);
    }

    [Fact]
    public async Task NewsMessage_Constructor_SetsInputCorrectlyWithArrayMoreThanOne()
    {
        // Arrange
        uint numberOfTemplateCodes = 0;
        uint numberOfRelatedSymbols = 3;
        List<string> templateCodes = [];
        List<string> relatedSymbols = ["ACE13C2109A", "BANP13C2109A", "BCPG13C2109A"];

        var input = CreateNewsMessage(
            numberOfTemplateCodes,
            templateCodes,
            numberOfRelatedSymbols,
            relatedSymbols
        );

        // Act
        var output = await itchParserService.Parse(input) as NewsMessage;

        // Assert
        Assert.NotNull(output);
        Assert.Equal(nanos, output.Nanos);
        Assert.Equal(reportType.PadRight(10), output.ReportType);
        Assert.Equal(marketId.PadRight(1), output.MarketId);
        Assert.Equal(language.PadRight(1), output.Language);
        Assert.Equal(senderName.PadRight(20), output.SenderName);
        Assert.Equal(expectedPublishedDate, output.PublishDate.ToString());
        Assert.Equal(newsType.PadRight(2), output.NewsType);
        Assert.Equal(headline.PadRight(250), output.Headline);
        Assert.Equal(newsFileName.PadRight(50), output.NewsFileName);
        Assert.Equal(PDFFileName.PadRight(50), output.PDFFileName);
        Assert.Equal(XMLFileName.PadRight(50), output.XMLFileName);
        Assert.Equal(period.PadRight(24), output.Period);
        Assert.Equal(new Date(dateAsOf), output.DateAsOf);
        Assert.Equal(quarter, output.Quarter);
        Assert.Equal(finStmtType.PadRight(1), output.FinStmtType);
        Assert.Equal(finStmtStatus.PadRight(1), output.FinStmtStatus);
        Assert.Equal(status.PadRight(1), output.Status);
        Assert.Equal(newsId.PadRight(20), output.NewsId);
        Assert.Equal(newsGroup.PadRight(20), output.NewsGroup);
        Assert.Equal(numberOfTemplateCodes, output.NumberOfTemplateCodes);
        Assert.Equal(numberOfRelatedSymbols, output.NumberOfRelatedSymbols);
        Assert.Equal(relatedSymbols[0].PadRight(20), output.RelatedSymbols[0].Data);
        Assert.Equal(relatedSymbols[1].PadRight(20), output.RelatedSymbols[1].Data);
        Assert.Equal(relatedSymbols[2].PadRight(20), output.RelatedSymbols[2].Data);
    }

    [Fact]
    public async Task NewsMessage_Constructor_SetsInputWithIncorrectFormat()
    {
        // Arrange
        byte[] input = [(byte)ItchMessageType.j, 0, 0, 0, 1];

        // Act
        // Assert
        await Assert.ThrowsAsync<ArgumentException>(async () => await itchParserService.Parse(input));
    }
}