namespace Pi.Financial.FundService.Application.Queries;

public record FundAccountOpeningStatus(
    string CustCode,
    string RequestReceivedTime,
    string? Status,
    bool? Ndid,
    Guid? DocumentGenerationTicketId,
    string? NdidRequestId,
    string? NdidDateTime,
    string? FailedReason,
    bool? IsOpenSegregateAccount,
    long? CustomerId,
    Guid? UserId,
    string? OpenAccountRegisterUid
    );

public interface IFundAccountOpeningStateQueries
{
    Task<IEnumerable<FundAccountOpeningStatus>> GetFundAccountOpeningStatesByRequestDate(
        DateOnly requestReceivedDate,
        bool? ndid
    );

    Task<IEnumerable<FundAccountOpeningStatus>> GetMultipleFundAccountOpeningStatesByCustCode(
        String custCode
    );
}
