namespace Pi.User.Application.Services.SSO;

public interface ISsoService
{
    public Task<bool> CheckSyncedPin(string? custCode);
}