namespace Pi.GlobalMarketData.Domain.Models.Request;

public class MorningStarCenterApiRequest
{
    public IdentifierType IdentifierType { get; set; } = IdentifierType.Isin;
    public string? Identifier { get; set; }
    public string? AccessCode { get; set; }
    public string? ResponseTypeJson { get; set; } = "Json";
}
