using System.Xml.Serialization;

namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;

public class HistoricalDistribution
{
    [XmlElement(ElementName = "Dividends")]
    public HistoricalDividend Dividend { get; set; }

    [XmlRoot(ElementName = "DividendDetail")]
    public class DividendDetail
    {
        [XmlElement(ElementName = "PayDate")]
        public DateTime PayDate { get; set; }

        [XmlElement(ElementName = "TotalDividend")]
        public double TotalDividend { get; set; }
    }

    [XmlRoot(ElementName = "Dividends")]
    public class HistoricalDividend
    {
        [XmlElement(ElementName = "DividendDetail")]
        public List<DividendDetail> DividendDetails { get; set; }
    }

}
