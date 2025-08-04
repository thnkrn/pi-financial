using System.Xml.Serialization;

namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;

[XmlRoot("response")]
public class XmlResponse<TData>
{
    [XmlElement("data")]
    public List<TData> DataList { get; init; }
}
