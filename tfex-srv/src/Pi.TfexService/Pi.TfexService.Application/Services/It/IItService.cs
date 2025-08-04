using Pi.TfexService.Application.Models;

namespace Pi.TfexService.Application.Services.It;

public interface IItService
{
    Task<PaginatedOrderTrade> GetTradeDetail(
        GetTradeDetailRequestModel requestModel,
        CancellationToken cancellationToken = default);
    Task<PositionTfexResponseData> GetTfexData(string custCode, CancellationToken cancellationToken = default);
}