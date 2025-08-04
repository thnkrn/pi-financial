using System.Xml.Serialization;

namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;

[XmlRoot(ElementName = "Prices")]
public class HistoricalNav
{
    [XmlElement("Prices")]
    public HistoricalPrice Historical { get; init; }

    public class HistoricalPrice
    {
        [XmlElement("p")]
        public List<Price> PriceList { get; init; }
    }

    public class Price
    {
        [XmlAttribute("v")]
        public decimal Value { get; init; }

        [XmlAttribute("d")]
        public DateTime Date { get; init; }
    }

}
