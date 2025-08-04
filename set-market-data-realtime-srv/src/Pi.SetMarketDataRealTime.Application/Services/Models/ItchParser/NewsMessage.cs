using Pi.SetMarketDataRealTime.Application.Services.Constants;
using Pi.SetMarketDataRealTime.Application.Services.Types.ItchParser;
using Pi.SetMarketDataRealTime.Application.Utils;

namespace Pi.SetMarketDataRealTime.Application.Services.Models.ItchParser;

public class NewsMessageParams
{
    public required Numeric32 Nanos { get; init; }
    public required Alpha ReportType { get; init; }
    public required Alpha MarketId { get; init; }
    public required Alpha Language { get; init; }
    public required Alpha SenderName { get; init; }
    public required Timestamp PublishDate { get; init; }
    public required Alpha NewsType { get; init; }
    public required Alpha Headline { get; init; }
    public required Alpha NewsFileName { get; init; }
    public required Alpha PDFFileName { get; init; }
    public required Alpha XMLFileName { get; init; }
    public required Alpha Period { get; init; }
    public required Date DateAsOf { get; init; }
    public required Numeric8 Quarter { get; init; }
    public required Alpha FinStmtType { get; init; }
    public required Alpha FinStmtStatus { get; init; }
    public required Alpha Status { get; init; }
    public required Alpha NewsId { get; init; }
    public required Alpha NewsGroup { get; init; }
    public required Numeric8 NumberOfTemplateCodes { get; init; }
    public required IReadOnlyList<TemplateCode> TemplateCodes { get; init; }
    public required Numeric8 NumberOfRelatedSymbols { get; init; }
    public required IReadOnlyList<TemplateCode> RelatedSymbols { get; init; }
}

public struct TemplateCode
{
    public Alpha Data { get; }

    public TemplateCode(Alpha data)
    {
        Data = data;
    }

    public override string ToString()
    {
        return Data.ToString();
    }
}

public class NewsMessage : ItchMessage
{
    public NewsMessage(NewsMessageParams newsMessageParams)
    {
        MsgType = ItchMessageType.j;
        Nanos = newsMessageParams.Nanos;
        ReportType = newsMessageParams.ReportType;
        MarketId = newsMessageParams.MarketId;
        Language = newsMessageParams.Language;
        SenderName = newsMessageParams.SenderName;
        PublishDate = newsMessageParams.PublishDate;
        NewsType = newsMessageParams.NewsType;
        Headline = newsMessageParams.Headline;
        NewsFileName = newsMessageParams.NewsFileName;
        PDFFileName = newsMessageParams.PDFFileName;
        XMLFileName = newsMessageParams.XMLFileName;
        Period = newsMessageParams.Period;
        DateAsOf = newsMessageParams.DateAsOf;
        Quarter = newsMessageParams.Quarter;
        FinStmtType = newsMessageParams.FinStmtType;
        FinStmtStatus = newsMessageParams.FinStmtStatus;
        Status = newsMessageParams.Status;
        NewsId = newsMessageParams.NewsId;
        NewsGroup = newsMessageParams.NewsGroup;
        NumberOfTemplateCodes = newsMessageParams.NumberOfTemplateCodes;
        TemplateCodes = newsMessageParams.TemplateCodes;
        NumberOfRelatedSymbols = newsMessageParams.NumberOfRelatedSymbols;
        RelatedSymbols = newsMessageParams.RelatedSymbols;
    }

    public override Numeric32 Nanos { get; }
    public Alpha ReportType { get; }
    public Alpha MarketId { get; }
    public Alpha Language { get; }
    public Alpha SenderName { get; }
    public Timestamp PublishDate { get; }
    public Alpha NewsType { get; }
    public Alpha Headline { get; }
    public Alpha NewsFileName { get; }
    public Alpha PDFFileName { get; }
    public Alpha XMLFileName { get; }
    public Alpha Period { get; }
    public Date DateAsOf { get; }
    public Numeric8 Quarter { get; }
    public Alpha FinStmtType { get; }
    public Alpha FinStmtStatus { get; }
    public Alpha Status { get; }
    public Alpha NewsId { get; }
    public Alpha NewsGroup { get; }
    public Numeric8 NumberOfTemplateCodes { get; }
    public IReadOnlyList<TemplateCode> TemplateCodes { get; }
    public Numeric8 NumberOfRelatedSymbols { get; }
    public IReadOnlyList<TemplateCode> RelatedSymbols { get; }

    public static NewsMessage Parse(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);
        if (bytes.Length < 522)
            throw new ArgumentException("Invalid data format for NewsMessage. Expected at least 522 bytes.",
                nameof(bytes));

        using var reader = new ItchMessageByteReader(bytes);
        var templateCodes = new List<TemplateCode>();
        var relatedSymbols = new List<TemplateCode>();

        var newsMessageParams = new NewsMessageParams
        {
            Nanos = reader.ReadNumeric32(),
            ReportType = reader.ReadAlpha(10),
            MarketId = reader.ReadAlpha(1),
            Language = reader.ReadAlpha(1),
            SenderName = reader.ReadAlpha(20),
            PublishDate = reader.ReadTimestamp(),
            NewsType = reader.ReadAlpha(2),
            Headline = reader.ReadAlpha(250),
            NewsFileName = reader.ReadAlpha(50),
            PDFFileName = reader.ReadAlpha(50),
            XMLFileName = reader.ReadAlpha(50),
            Period = reader.ReadAlpha(24),
            DateAsOf = reader.ReadDate(),
            Quarter = reader.ReadNumeric8(),
            FinStmtType = reader.ReadAlpha(1),
            FinStmtStatus = reader.ReadAlpha(1),
            Status = reader.ReadAlpha(1),
            NewsId = reader.ReadAlpha(20),
            NewsGroup = reader.ReadAlpha(20),
            NumberOfTemplateCodes = reader.ReadNumeric8(),
            TemplateCodes = ReadTemplateCodes(reader),
            NumberOfRelatedSymbols = reader.ReadNumeric8(),
            RelatedSymbols = ReadRelatedSymbols(reader)
        };

        return new NewsMessage(newsMessageParams);
    }

    private static IReadOnlyList<TemplateCode> ReadTemplateCodes(ItchMessageByteReader reader)
    {
        var count = reader.ReadNumeric8();
        var codes = new List<TemplateCode>(count);
        for (var i = 0; i < count; i++) codes.Add(new TemplateCode(reader.ReadAlpha(2)));
        return codes.AsReadOnly();
    }

    private static IReadOnlyList<TemplateCode> ReadRelatedSymbols(ItchMessageByteReader reader)
    {
        var count = reader.ReadNumeric8();
        var symbols = new List<TemplateCode>(count);
        for (var i = 0; i < count; i++) symbols.Add(new TemplateCode(reader.ReadAlpha(20)));
        return symbols.AsReadOnly();
    }

    public string ToStringUnitTest()
    {
        return $"""
                NewsMessage:
                Nanos: {Nanos},
                ReportType: {ReportType},
                MarketId: {MarketId},
                Language: {Language},
                SenderName: {SenderName},
                PublishDate: {PublishDate},
                NewsType: {NewsType},
                Headline: {Headline},
                NewsFileName: {NewsFileName},
                PDFFileName: {PDFFileName},
                XMLFileName: {XMLFileName},
                Period: {Period},
                DateAsOf: {DateAsOf},
                Quarter: {Quarter},
                FinStmtType: {FinStmtType},
                FinStmtStatus: {FinStmtStatus},
                Status: {Status},
                NewsId: {NewsId},
                NewsGroup: {NewsGroup},
                NumberOfTemplateCodes: {NumberOfTemplateCodes},
                TemplateCodes:
                {string.Join("\n", TemplateCodes.Select(tc => $"  TemplateCode: {tc}"))}
                NumberOfRelatedSymbols: {NumberOfRelatedSymbols},
                RelatedSymbols:
                {string.Join("\n", RelatedSymbols.Select(rs => $"  RelatedSymbol: {rs}"))}
                """;
    }
}