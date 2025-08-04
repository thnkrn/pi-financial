using System.ComponentModel.DataAnnotations;

namespace Pi.Financial.FundService.API.Models
{
    public class OpenFundAccountDto
    {
        public OpenFundAccountDto(string customerCode, bool ndid, NdidInfo ndidInfo, string? openAccountRegisterUid, bool? isOpenSegregateAccount = false)
        {
            CustomerCode = customerCode;
            Ndid = ndid;
            NdidInfo = ndidInfo;
            OpenAccountRegisterUid = openAccountRegisterUid;
            IsOpenSegregateAccount = isOpenSegregateAccount;
        }

        [Required, MinLength(7), MaxLength(7)]
        public string CustomerCode { get; }
        public bool Ndid { get; }
        public NdidInfo? NdidInfo { get; }

        public bool? IsOpenSegregateAccount { get; }

        public string? OpenAccountRegisterUid { get; }

        public long? CustomerId { get; }
    }

    public record NdidInfo(DateTime ApprovedDateTime, string RequestId);

    public record FundAccountOpeningTicket(string CustomerCode, Guid? TicketId);

    public record GeneratedDocument(Guid? TicketId, IEnumerable<Document> Documents);

    public record Document(string DocumentType, string PreSignedUrl);

    public record NdidResponse(string NdidReferenceId, string RequestTime);
}
