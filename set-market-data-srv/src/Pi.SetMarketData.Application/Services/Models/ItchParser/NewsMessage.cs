using Pi.SetMarketData.Application.Constants;
using Pi.SetMarketData.Application.Services.Types.ItchParser;
using Pi.SetMarketData.Application.Utils;

namespace Pi.SetMarketData.Application.Services.Models.ItchParser
{
    public class NewsMessageParams
    {
        public Numeric32 Nanos { get; set; }
        public required Alpha ReportType { get; set; }
        public required Alpha MarketId { get; set; }
        public required Alpha Language { get; set; }
        public required Alpha SenderName { get; set; }
        public Timestamp PublishDate { get; set; }
        public required Alpha NewsType { get; set; }
        public required Alpha Headline { get; set; }
        public required Alpha NewsFileName { get; set; }
        public required Alpha PDFFileName { get; set; }
        public required Alpha XMLFileName { get; set; }
        public required Alpha Period { get; set; }
        public Date DateAsOf { get; set; }
        public Numeric8 Quarter { get; set; }
        public required Alpha FinStmtType { get; set; }
        public required Alpha FinStmtStatus { get; set; }
        public required Alpha Status { get; set; }
        public required Alpha NewsId { get; set; }
        public required Alpha NewsGroup { get; set; }
        public Numeric8 NumberOfTemplateCodes { get; set; }
        public required List<TemplateCode> TemplateCodes { get; set; }
        public Numeric8 NumberOfRelatedSymbols { get; set; }
        public required List<TemplateCode> RelatedSymbols { get; set; }
    }

    public class TemplateCode(Alpha data)
    {
        public Alpha Data { get; private set; } = data;
    }

    public class NewsMessage : ItchMessage
    {
        public Numeric32 Nanos { get; private set; }
        public Alpha ReportType { get; private set; }
        public Alpha MarketId { get; private set; }
        public Alpha Language { get; private set; }
        public Alpha SenderName { get; private set; }
        public Timestamp PublishDate { get; private set; }
        public Alpha NewsType { get; private set; }
        public Alpha Headline { get; private set; }
        public Alpha NewsFileName { get; private set; }
        public Alpha PDFFileName { get; private set; }
        public Alpha XMLFileName { get; private set; }
        public Alpha Period { get; private set; }
        public Date DateAsOf { get; private set; }
        public Numeric8 Quarter { get; private set; }
        public Alpha FinStmtType { get; private set; }
        public Alpha FinStmtStatus { get; private set; }
        public Alpha Status { get; private set; }
        public Alpha NewsId { get; private set; }
        public Alpha NewsGroup { get; private set; }
        public Numeric8 NumberOfTemplateCodes { get; private set; }
        public List<TemplateCode> TemplateCodes { get; private set; }
        public Numeric8 NumberOfRelatedSymbols { get; private set; }
        public List<TemplateCode> RelatedSymbols { get; private set; }

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
            TemplateCodes = [.. newsMessageParams.TemplateCodes];

            NumberOfRelatedSymbols = newsMessageParams.NumberOfRelatedSymbols;
            RelatedSymbols = [.. newsMessageParams.RelatedSymbols];
        }

        public static NewsMessage Parse(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 522) // Expecting at least 522 bytes for the NewsMessage
            {
                throw new ArgumentException("Invalid data format for NewsMessage.");
            }

            var reader = new ItchMessageByteReader(new Memory<byte>(bytes));
            var nanos = reader.ReadNumeric32();
            var reportType = reader.ReadAlpha(10);
            var marketId = reader.ReadAlpha(1);
            var language = reader.ReadAlpha(1);
            var senderName = reader.ReadAlpha(20);
            var publishDate = reader.ReadTimestamp();
            var newsType = reader.ReadAlpha(2);
            var headline = reader.ReadAlpha(250);
            var newsFileName = reader.ReadAlpha(50);
            var pdfFileName = reader.ReadAlpha(50);
            var xmlFileName = reader.ReadAlpha(50);
            var period = reader.ReadAlpha(24);
            var dateAsOf = reader.ReadDate();
            var quarter = reader.ReadNumeric8();
            var finStmtType = reader.ReadAlpha(1);
            var finStmtStatus = reader.ReadAlpha(1);
            var status = reader.ReadAlpha(1);
            var newsId = reader.ReadAlpha(20);
            var newsGroup = reader.ReadAlpha(20);
            var numberOfTemplateCodes = reader.ReadNumeric8();
            List<TemplateCode> templateCodes = [];

            for (int i = 0; i < Convert.ToInt16(numberOfTemplateCodes); i++)
            {
                templateCodes.Add(new TemplateCode(reader.ReadAlpha(2)));
            }

            var numberOfRelatedSymbols = reader.ReadNumeric8();
            List<TemplateCode> relatedSymbols = [];

            for (int i = 0; i < Convert.ToInt16(numberOfRelatedSymbols); i++)
            {
                relatedSymbols.Add(new TemplateCode(reader.ReadAlpha(20)));
            }

            var newsMessageParams = new NewsMessageParams
            {
                Nanos = nanos,
                ReportType = reportType,
                MarketId = marketId,
                Language = language,
                SenderName = senderName,
                PublishDate = publishDate,
                NewsType = newsType,
                Headline = headline,
                NewsFileName = newsFileName,
                PDFFileName = pdfFileName,
                XMLFileName = xmlFileName,
                Period = period,
                DateAsOf = dateAsOf,
                Quarter = quarter,
                FinStmtType = finStmtType,
                FinStmtStatus = finStmtStatus,
                Status = status,
                NewsId = newsId,
                NewsGroup = newsGroup,
                NumberOfTemplateCodes = numberOfTemplateCodes,
                TemplateCodes = templateCodes,
                NumberOfRelatedSymbols = numberOfRelatedSymbols,
                RelatedSymbols = relatedSymbols
            };

            return new NewsMessage(newsMessageParams);
        }

        public override string ToString()
        {
            var messageStr =
                $"NewsMessage:\n"
                + $"Nanos: {Nanos},\n"
                + $"ReportType: {ReportType},\n"
                + $"MarketId: {MarketId},\n"
                + $"Language: {Language},\n"
                + $"SenderName: {SenderName},\n"
                + $"PublishDate: {PublishDate},\n"
                + $"NewsType: {NewsType},\n"
                + $"Headline: {Headline},\n"
                + $"NewsFileName: {NewsFileName},\n"
                + $"PDFFileName: {PDFFileName},\n"
                + $"XMLFileName: {XMLFileName},\n"
                + $"Period: {Period},\n"
                + $"DateAsOf: {DateAsOf},\n"
                + $"Quarter: {Quarter},\n"
                + $"FinStmtType: {FinStmtType},\n"
                + $"FinStmtStatus: {FinStmtStatus},\n"
                + $"Status: {Status},\n"
                + $"NewsId: {NewsId},\n"
                + $"NewsGroup: {NewsGroup},\n"
                + $"NumberOfTemplateCodes: {NumberOfTemplateCodes},\n"
                + $"TemplateCodes:";

            foreach (var TemplateCode in TemplateCodes)
            {
                messageStr += $"\n  TemplateCode: {TemplateCode}";
            }

            messageStr +=
                $"\nNumberOfRelatedSymbols: {NumberOfRelatedSymbols}, \n" + $"RelatedSymbols:";

            foreach (var RelatedSymbol in RelatedSymbols)
            {
                messageStr += $"\n  RelatedSymbol: {RelatedSymbol}";
            }

            return messageStr;
        }
    }
}
