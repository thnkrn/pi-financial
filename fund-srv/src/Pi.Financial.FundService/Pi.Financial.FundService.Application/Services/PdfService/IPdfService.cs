using Pi.Financial.FundService.Application.Models.Customer;
using Pi.Financial.FundService.Domain.AccountOpening.Events.Models;

namespace Pi.Financial.FundService.Application.Services.PdfService
{
    public interface IPdfService
    {
        /// <summary>
        /// Generate FundConnext documents
        /// </summary>
        /// <param name="customerInfo"></param>
        /// <param name="customerAccount"></param>
        /// <param name="fatcaInfo"></param>
        /// <param name="crs"></param>
        /// <param name="ndidRequestId"></param>
        /// <param name="ndidDatetime"></param>
        /// <param name="dopaDateTime"></param>
        /// <returns>Request id for tracking</returns>
        Task<IEnumerable<Document>> GenerateFundConnextDocuments(
            CustomerInfo customerInfo,
            CustomerAccount customerAccount,
            FatcaInfo fatcaInfo,
            Crs crs,
            string? ndidRequestId,
            DateTime? ndidDatetime,
            DateTime? dopaDateTime);
    }
}
