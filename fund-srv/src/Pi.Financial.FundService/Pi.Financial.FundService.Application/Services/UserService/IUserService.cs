using Pi.Client.UserService.Model;
using Pi.Financial.FundService.Application.Models.Bank;

namespace Pi.Financial.FundService.Application.Services.UserService
{
    public interface IUserService
    {
        Task<string> GetCustomerIdByUserId(string userId);
        Task<IEnumerable<string>> GetCustomerCodesByUserId(Guid userId);
        Task<IEnumerable<string>> GetFundCustomerCodesByUserId(Guid userId, CancellationToken cancellationToken = default);
        Task<Guid?> GetUserIdByCustomerCode(string customerCode, CancellationToken cancellationToken = default);
        Task<string?> GetCitizenIdByUserId(Guid userId);
        Task<IEnumerable<PiUserApplicationModelsDocumentDocumentDto>> GetDocumentByUserId(Guid userId);
        Task<AtsBankAccounts> GetAtsBankAccountsByCustomerCodeAsync(string customerCode, CancellationToken cancellationToken = default);
    }
}
