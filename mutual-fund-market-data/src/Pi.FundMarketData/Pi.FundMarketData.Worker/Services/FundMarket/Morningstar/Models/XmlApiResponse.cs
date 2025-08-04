using System.Xml.Serialization;

namespace Pi.FundMarketData.Worker.Services.FundMarket.Morningstar.Models;

[XmlRoot("response")]
public class XmlApiResponse<TApiData>
{
    [XmlElement("data")]
    public List<Data> DataList { get; set; }

    public class Data
    {
        [XmlElement("api")]
        public TApiData ApiData { get; set; }
    }
}

