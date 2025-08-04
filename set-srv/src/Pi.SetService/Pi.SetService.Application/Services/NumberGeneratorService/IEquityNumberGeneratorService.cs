namespace Pi.SetService.Application.Services.NumberGeneratorService;

public interface IEquityNumberGeneratorService
{
    Task<string> GenerateAndUpdateOnlineOrderNoAsync(Guid correlationId, CancellationToken cancellationToken = default);
    Task<ulong> GenerateAndUpdateOfflineOrderNoAsync(Guid correlationId, CancellationToken cancellationToken = default);
    Task<ulong> GenerateSblOrderOrderIdAsync(CancellationToken cancellationToken = default);
}
