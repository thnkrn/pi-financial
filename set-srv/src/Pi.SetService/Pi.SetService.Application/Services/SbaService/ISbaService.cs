using Pi.SetService.Application.Models.InitialMargin;

namespace Pi.SetService.Application.Services.SbaService;

public interface ISbaService
{
    IAsyncEnumerable<MarginRateInfo> GetMarginRatesFromStorage(string bucketName, string fileKey, CancellationToken ct = default);
    IAsyncEnumerable<MarginInstrumentInfo> GetMarginInstrumentInfoFromStorage(string bucketName, string fileKey, CancellationToken ct = default);
}
