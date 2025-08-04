using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.TfexService.Application.Services.It;
using Pi.Client.ItBackOffice.Api;
using Pi.TfexService.Application.Models;
using Pi.TfexService.Application.Utils;
using ApiException = Pi.Client.ItBackOffice.Client.ApiException;
using Pi.TfexService.Domain.Exceptions;
using Pi.TfexService.Infrastructure.Options;

namespace Pi.TfexService.Infrastructure.Services;

public class ItService(
    IBackOfficeApi itApi,
    IOptionsSnapshot<ItOptions> options,
    ILogger<ItService> logger) : IItService
{
    public async Task<PaginatedOrderTrade> GetTradeDetail(
        GetTradeDetailRequestModel requestModel,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var dateFrom = requestModel.DateFrom.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            var dateTo = requestModel.DateTo.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

            var response = await itApi.ApiBackOfficeOrderTradesGetAsync(dateFrom, dateTo, requestModel.AccountNo, options.Value.ApiKey, cancellationToken);
            if (response.Code != 0 && response.Message != "success")
            {
                throw new ItApiException($"IT Api response error code: {response.Code}, message: {response.Message}");
            }

            response.Data.RemoveAll(order =>
            {
                if (requestModel.Side.HasValue && ItOrderUtils.MappingSide(order.RefType) != requestModel.Side)
                {
                    return true;
                }

                if (requestModel.Position.HasValue && ItOrderUtils.MappingPosition(order.Buysellsorter) != requestModel.Position)
                {
                    return true;
                }

                return false;
            });

            var (trades, hasNextPage) = WithPagination(response.Data, requestModel.Page, requestModel.PageSize);

            return new PaginatedOrderTrade(trades, hasNextPage);
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetTradeDetail", requestModel.AccountNo);
        }
    }

    public async Task<PositionTfexResponseData> GetTfexData(string custCode, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await itApi.ApiBackOfficePositionTFEXGetAsync(custCode, options.Value.ApiKey, cancellationToken);

            return new PositionTfexResponseData(
                response.Code,
                response.Message,
                response.Count,
                response.Timestemp,
                new PositionTfex(
                    response.Data.SendDate,
                    response.Data.SendTime,
                    response.Data.OutstdDate,
                    response.Data.Custcode,
                    response.Data.Custacct,
                    response.Data.Account,
                    response.Data.Xchgmkt,
                    (decimal?)response.Data.Equity,
                    (decimal?)response.Data.Excessequity,
                    (decimal?)response.Data.Mktval,
                    (decimal?)response.Data.UnrealizePl,
                    (decimal?)response.Data.RealizePl,
                    response.Data.PositionList?.Select(x => new PositionItem(
                        x.Sharecode,
                        (int)x.Longunit,
                        (int)x.Shortunit,
                        (decimal)x.Accmtm,
                        (decimal)x.AvgcostUsdLong,
                        (decimal)x.AvgcostUsdShort,
                        (decimal)x.AvgcostThbLong,
                        (decimal)x.AvgcostThbShort,
                        x.Fxcode,
                        (decimal)x.Settlementprice,
                        (decimal)x.Multiplier
                    )).ToList()
                )
            );
        }
        catch (Exception e)
        {
            throw HandleException(e, "GetTfexData", custCode);
        }
    }

    private Exception HandleException(Exception e, string methodName, string? accountNo = null)
    {
        logger.LogError(e, "ItService Exception");

        if (e is ApiException apiException)
        {
            return HandleApiException(apiException, methodName, accountNo);
        }

        return new ItApiException($"ItApiException {methodName}: Error while calling IT Api for AccountNo: {accountNo}", string.Empty, e);
    }

    private Exception HandleApiException(ApiException e, string methodName, string? accountNo = null)
    {
        var apiException = ApiExceptionHelper.DeserializeApiException(e.ErrorContent);

        logger.LogError("ItService {MethodName}: Account No: {AccountNo} Error while calling IT Api: {ErrorMessage}", methodName, accountNo, apiException.ErrorMessage);

        return e.ErrorCode switch
        {
            StatusCodes.Status404NotFound => CreateItException<ItNotFoundException>("Trade not found"),
            _ => CreateItException<ItApiException>(apiException.ErrorMessage ?? "An unknown error occurred")
        };

        Exception CreateItException<T>(string message) where T : Exception =>
            (T)Activator.CreateInstance(typeof(T), $"ItService {methodName}: {message}", apiException.ErrorCode, e)!;
    }

    private static (List<T> Items, bool HasNextPage) WithPagination<T>(List<T> source, int page, int pageSize)
    {
        var skip = (page - 1) * pageSize;
        if (skip >= source.Count)
        {
            return ([], false);
        }

        var take = Math.Min(pageSize, source.Count - skip);
        var paginatedList = source.Skip(skip).Take(take).ToList();
        var hasNextPage = skip + take < source.Count;

        return (paginatedList, hasNextPage);
    }
}