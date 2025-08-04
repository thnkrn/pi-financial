using Pi.Financial.FundService.Domain.AccountOpening.Events.Models;

namespace Pi.Financial.FundService.Domain.Events
{
    public record AccountOpeningDocumentsGenerated(Guid DocumentsGenerationTicketId, string CustomerCode,
        IEnumerable<Document> Documents, bool Ndid);
}
