namespace Pi.TfexService.Application.Services.DistributedLock;

public interface IDistributedLockService
{
    Task<bool> AddEventAsync(string eventName);
    Task<bool> RemoveEventAsync(string eventName);
}