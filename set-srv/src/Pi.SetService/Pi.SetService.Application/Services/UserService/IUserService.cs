namespace Pi.SetService.Application.Services.UserService;

public interface IUserService
{
    Task<IEnumerable<string>> GetCustomerCodesByUserId(Guid userId);
    Task<Guid?> GetUserIdByCustCode(string custCode, CancellationToken contextCancellationToken);
}
