using Microsoft.Extensions.Logging;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Services.It;
using Pi.TfexService.Domain.Exceptions;

namespace Pi.TfexService.Application.Queries.Order;

public class ItOrderTradeQueries(IItService itService, ILogger<ItOrderTradeQueries> logger) : IItOrderTradeQueries
{
    public async Task<PaginatedOrderTrade?> GetOrderTrade(
        GetTradeDetailRequestModel requestModel,
        CancellationToken cancellationToken = default)
    {
        if (requestModel.DateTo < requestModel.DateFrom)
        {
            throw new ArgumentException("Date from must less than date to");
        }

        var range = (requestModel.DateTo.ToDateTime(TimeOnly.MinValue) - requestModel.DateFrom.ToDateTime(TimeOnly.MinValue)).Days;
        if (Math.Abs(range) > 90)
        {
            throw new ArgumentException("The date range must not exceed 90 days");
        }

        requestModel.AccountNo = ConvertAccountCodeFormat(requestModel.AccountNo);

        try
        {
            return await itService.GetTradeDetail(requestModel, cancellationToken);
        }
        catch (ItNotFoundException)
        {
            return null;
        }
    }

    private string ConvertAccountCodeFormat(string accountCode)
    {
        switch (accountCode.Length)
        {
            case 8 when !accountCode.Contains('-'):
                return accountCode.Insert(7, "-");
            case 9 when accountCode.Contains('-'):
                return accountCode;
            default:
                logger.LogError("The account code must contain format xxxxxxx0 or xxxxxxx-0, account code: {AccountCode}", accountCode);
                throw new ArgumentException("The account code must contain format xxxxxxx0 or xxxxxxx-0");
        }
    }
}