using System.Net;
using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Pi.BackofficeService.Application.Exceptions;
using Pi.BackofficeService.Application.Models.Commons;
using Pi.BackofficeService.Application.Models.Sbl;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Application.Services.SblService;
using Pi.BackofficeService.Infrastructure.Factories;
using Pi.BackofficeService.Infrastructure.Options;
using Pi.Client.SetService.Api;
using Pi.Client.SetService.Client;
using Pi.Client.SetService.Model;

namespace Pi.BackofficeService.Infrastructure.Services;

public class SblService(
    ISblApi sblApi,
    ISyncApi syncApi,
    IAmazonS3 s3Client,
    IOptions<SblServiceOptions> options) : ISblService
{
    private const int MaxSize = 10 * 1024 * 1024; // 10MB

    public async Task<PaginateResult<SblOrder>> GetSblOrderPaginateAsync(SblOrderFilters filters, int pageNum,
        int pageSize, string? orderBy, string? orderDir, CancellationToken ct = default)
    {
        List<PiSetServiceDomainAggregatesModelFinancialAssetAggregateSblOrderStatus>? filterStatuses = null;
        if (filters.Statues != null)
        {
            filterStatuses = [];
            foreach (var sblOrderStatus in filters.Statues)
            {
                if (SetFactory.TryNewSourceSblOrderSide(sblOrderStatus, out var filter) && filter != null)
                {
                    filterStatuses.Add(filter.Value);
                }
            }
        }

        var response = await sblApi.InternalSblOrdersGetAsync(
            pageNum,
            pageSize,
            orderBy,
            orderDir?.ToLower() switch
            {
                "desc" => PiSetServiceDomainAggregatesModelCommonAggregateOrderDirection.Desc,
                "asc" => PiSetServiceDomainAggregatesModelCommonAggregateOrderDirection.Asc,
                _ => null
            },
            filters.TradingAccountNo,
            filters.Open,
            filters.Symbol,
            filterStatuses,
            SetFactory.TryNewSourceSblOrderType(filters.Type, out var filterType) ? filterType : null,
            filters.CreatedDateFrom?.ToDateTime(TimeOnly.MinValue),
            filters.CreatedDateTo?.ToDateTime(TimeOnly.MaxValue),
            ct
        );

        return new PaginateResult<SblOrder>(response.Data.Select(SetFactory.NewSblOrder).ToList(), response.Page,
            response.PageSize, response.Total, response.OrderBy, response.OrderDir);
    }

    public async Task<PaginateResult<SblInstrument>> GetSblInstrumentsPaginateAsync(SblInstrumentFilters filters, int pageNum, int pageSize, string? orderBy,
        string? orderDir, CancellationToken ct = default)
    {
        var response = await sblApi.InternalSblInstrumentsGetAsync(
            pageNum,
            pageSize,
            orderBy,
            orderDir?.ToLower() switch
            {
                "desc" => PiSetServiceDomainAggregatesModelCommonAggregateOrderDirection.Desc,
                "asc" => PiSetServiceDomainAggregatesModelCommonAggregateOrderDirection.Asc,
                _ => null
            },
            filters.Symbol,
            ct
        );

        return new PaginateResult<SblInstrument>(response.Data.Select(SetFactory.NewSblInstrument).ToList(), response.Page,
            response.PageSize, response.Total, response.OrderBy, response.OrderDir);
    }

    public async Task<int> UploadSblInstruments(string fileName, Stream content, CancellationToken ct = default)
    {
        if (content.Length > MaxSize)
        {
            throw new FileSizeTooLargeException(MaxSize);
        }

        await UploadFileAsync(options.Value.InstrumentBucket, fileName, content, ct);
        var response = await syncApi.InternalSyncSblInstrumentsPostAsync(
            new PiSetServiceApplicationCommandsSyncSblInstrument(options.Value.InstrumentBucket, fileName), ct);

        return response.Data.Create;
    }

    public async Task<SblOrder> SubmitReviewSblOrderAsync(SubmitReview submitReview, CancellationToken ct = default)
    {
        try
        {
            var response = await sblApi.InternalSblOrdersOrderIdPatchAsync(
                submitReview.Id,
                new PiSetServiceAPIModelsSblOrderSubmitReviewRequest(
                    SetFactory.NewSubmitReviewStatus(submitReview.SblOrderStatus),
                    submitReview.ReviewerId,
                    submitReview.RejectedReason!
                ), ct);

            return SetFactory.NewSblOrder(response.Data);
        }
        catch (ApiException e)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var externalApiException =
                JsonSerializer.Deserialize<ExternalApiException>(
                    e.ErrorContent.ToString() ?? throw new InvalidOperationException(), options);
            if (externalApiException == null)
            {
                throw;
            }

            throw externalApiException;
        }
    }

    private async Task UploadFileAsync(string bucket, string fileName, Stream content, CancellationToken ct = default)
    {
        var request = new PutObjectRequest()
        {
            Key = fileName,
            BucketName = bucket,
            InputStream = content,
        };
        var response = await s3Client.PutObjectAsync(request, ct);

        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            throw new UploadFileFailedException(fileName);
        }
    }
}
