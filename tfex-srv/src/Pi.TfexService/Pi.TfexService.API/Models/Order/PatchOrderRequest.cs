using Pi.TfexService.Application.Models;

namespace Pi.TfexService.API.Models.Order;

public record PatchOrderRequest(
    PatchOrderType PatchType,
    decimal? Price = null,
    int? Volume = null,
    bool? BypassWarning = true
)
{
    // validate the request
    public void Validate()
    {
        if (PatchType is not PatchOrderType.Update) return;

        if (Volume is null or <= 0 && Price is null or <= 0)
        {
            throw new ArgumentException("Update order required Price or Volume to change");
        }
    }
}