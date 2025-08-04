namespace Pi.WalletService.Application.Services.EmployeeService;

public record Employee(string Id, string Name, string Email);

public interface IEmployeeService
{
    Task<Employee> GetEmployee(string marketingId, CancellationToken cancellationToken = default);
}