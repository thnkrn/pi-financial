using Pi.SetService.Application.Models;

namespace Pi.SetService.Application.Services.SblService;

public interface ISblService
{
    IAsyncEnumerable<SblInstrumentSyncInfo> GetSblInstrumentInfoFromStorage(string bucketName, string fileKey, CancellationToken ct = default);
}
