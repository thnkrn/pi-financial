using Pi.Client.EmployeeService.Api;
using Pi.WalletService.Application.Services.EmployeeService;

namespace Pi.WalletService.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeApi _employeeApi;

    public EmployeeService(IEmployeeApi employeeApi)
    {
        _employeeApi = employeeApi;
    }

    public async Task<Employee> GetEmployee(string marketingId, CancellationToken cancellationToken = default)
    {
        var response = await _employeeApi.InternalGetEmployeeInfoByIdAsync(marketingId, cancellationToken);

        return new Employee(
            response.Data.Id,
            response.Data.NameTh,
            response.Data.Email);
    }
}