using Pi.Financial.Client.Freewill.Model;
using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Application.Models.Enums;

namespace Pi.Financial.FundService.Application.Services.CustomerService
{
    public interface ICustomerService
    {
        /// <summary>
        /// Get a customer information
        /// </summary>
        /// <exception cref="KeyNotFoundException">Customer id is not found.</exception>
        /// <param name="customerId"></param>
        /// <returns></returns>
        Task<CustomerInfo> GetCustomerInfo(string customerId);
        Task<CustomerInfoForSyncCustomerFundAccount> GetCustomerInfoForSyncFundCustomerAccount(string customerCode);

        [Obsolete("Use OnboardService.GetFundTradingAccount instead")]
        Task<CustomerAccount> GetCustomerAccount(string customerCode);

        Task<FatcaInfo> GetFatcaInfo(string customerId);

        Task<bool> IsOpenFundAccount(string customerId);

        string GetBuilding(GetAddressInfoResponseItem address);

        (Occupation, string?, BusinessType?, string?) GetOccupationAndBusinessType(GetKYCInfoResponseItem? kycInfo);
    }

    public class UnableToGetCustomerInfoException : Exception
    {
    }

    public class MissingRequiredFieldException : Exception
    {
        public string FieldName => $"Missing Required Field: {base.Message}";

        public MissingRequiredFieldException(string fieldName) : base(fieldName)
        {
        }
    }
}
