using System.ComponentModel.DataAnnotations;
using Pi.FundMarketData.Constants;

namespace Pi.FundMarketData.API.Models;

public class FundTradableDateFilter : IValidatableObject
{
    [Required] public TradeSide TradeType { get; init; }

    private readonly string _switchSymbol;
    public string SwitchSymbol { get => _switchSymbol; init => _switchSymbol = value?.ToUpper(); }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(SwitchSymbol) && TradeType == TradeSide.Switch)
        {
            yield return new ValidationResult("TradeType Switch required SwitchSymbol.");
        }

        if (!Enum.IsDefined(typeof(TradeSide), TradeType))
        {
            yield return new ValidationResult("Invalid TradeType, TradeType must be buy, sell or switch.");
        }
    }
}
