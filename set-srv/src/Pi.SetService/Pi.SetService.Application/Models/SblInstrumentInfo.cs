using Pi.SetService.Domain.AggregatesModel.InstrumentAggregate;

namespace Pi.SetService.Application.Models;

public class SblInstrumentInfo
{
    private static readonly string[] AvoidCaTypes = { "XD", "XR", "XA", "XW", "XB" };

    public required SblInstrument SblInstrument { get; init; }
    public required IEnumerable<CorporateAction> CorporateActions { get; init; }
    public required EquityMarginInfo MarginInfo { get; init; }

    public bool CanBorrow()
    {
        return !CorporateActions.Any(q => AvoidCaTypes.Contains(q.CaType.ToUpper()));
    }
}
