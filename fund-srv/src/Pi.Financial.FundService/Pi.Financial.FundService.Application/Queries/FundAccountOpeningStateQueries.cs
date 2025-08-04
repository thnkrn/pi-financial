using System.Globalization;
using Pi.Financial.FundService.Domain.AggregatesModel.AccountOpeningAggregate;

namespace Pi.Financial.FundService.Application.Queries;

public class FundAccountOpeningStateQueries : IFundAccountOpeningStateQueries
{
    private readonly IFundAccountOpeningStateRepository _fundAccountOpeningStateRepository;

    public FundAccountOpeningStateQueries(IFundAccountOpeningStateRepository fundAccountOpeningStateRepository)
    {
        _fundAccountOpeningStateRepository = fundAccountOpeningStateRepository;
    }

    public async Task<IEnumerable<FundAccountOpeningStatus>> GetFundAccountOpeningStatesByRequestDate(
        DateOnly requestReceivedDate,
        bool? ndid
    )
    {
        var result =
            await _fundAccountOpeningStateRepository
                .GetFundAccountOpeningStatesByRequestDate(requestReceivedDate, ndid);

        var results = result.Select(r => new FundAccountOpeningStatus(
            CustCode: r.CustomerCode ?? "",
            RequestReceivedTime: r.RequestReceivedTime.ToString("o", CultureInfo.InvariantCulture),
            Status: r.CurrentState,
            Ndid: null,
            DocumentGenerationTicketId: null,
            NdidRequestId: null,
            NdidDateTime: null,
            FailedReason: null,
            IsOpenSegregateAccount: null,
            CustomerId: null,
            UserId: null,
            OpenAccountRegisterUid: null
        ));

        return results;
    }

    public async Task<IEnumerable<FundAccountOpeningStatus>> GetMultipleFundAccountOpeningStatesByCustCode(string custCode)
    {
        var response = await _fundAccountOpeningStateRepository.GetMultipleFundAccountOpeningStatesByCustCode(custCode);
        var result = response.Select(r => new FundAccountOpeningStatus(
                CustCode: r.CustomerCode ?? "",
                RequestReceivedTime: r.RequestReceivedTime.ToString("o", CultureInfo.InvariantCulture),
                Status: r.CurrentState,
                Ndid: r.Ndid,
                DocumentGenerationTicketId: r.DocumentsGenerationTicketId,
                NdidRequestId: r.NdidRequestId,
                NdidDateTime: r.NdidDateTime?.ToString("o", CultureInfo.InvariantCulture),
                FailedReason: r.FailedReason,
                IsOpenSegregateAccount: r.IsOpenSegregateAccount,
                CustomerId: r.CustomerId,
                UserId: r.UserId,
                OpenAccountRegisterUid: r.OpenAccountRegisterUid))
            ;
        return result;
    }
}
