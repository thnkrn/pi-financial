using System.Runtime.CompilerServices;
using Amazon.S3;
using Amazon.S3.Model;
using Pi.SetService.Application.Models;
using Pi.SetService.Application.Services.SblService;

namespace Pi.SetService.Infrastructure.Services;

public class SblService(IAmazonS3 client) : ISblService
{
    private const string Separator = ",";

    public async IAsyncEnumerable<SblInstrumentSyncInfo> GetSblInstrumentInfoFromStorage(string bucketName, string fileKey, [EnumeratorCancellation] CancellationToken ct = default)
    {
        var dict = new Dictionary<string, int>();
        var updateHeader = false;

        await foreach (var (rawHeader, content) in ReadLineCsvFromS3(bucketName, fileKey, ct))
        {
            if (!updateHeader && rawHeader != null)
            {
                var headers = rawHeader.Split(Separator);
                for (var i = headers.Length - 1; i >= 0; i--)
                {
                    dict[headers[i].ToLower()] = i;
                }
                updateHeader = true;
            }
            var columns = content.Split(Separator);

            yield return new SblInstrumentSyncInfo
            {
                Symbol = columns[dict["symbol"]],
                InterestRate = Convert.ToDecimal(columns[dict["rate"]]),
                RetailLender = Convert.ToDecimal(columns[dict["retail lender"]])
            };
        }
    }

    private async IAsyncEnumerable<(string?, string)> ReadLineCsvFromS3(string bucketName, string fileKey,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await using var stream = await GetObjectAsync(bucketName, fileKey);
        using var reader = new StreamReader(stream);
        var headers = await reader.ReadLineAsync(ct);

        while (!reader.EndOfStream)
        {
            if (ct.IsCancellationRequested)
                yield break;

            var line = await reader.ReadLineAsync(ct);
            if (line != null)
            {
                yield return (headers, line);
            }
        }
    }

    private async Task<Stream> GetObjectAsync(string bucketName, string fileKey)
    {
        var request = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = fileKey,
        };

        var response = await client.GetObjectAsync(request);

        return response.ResponseStream;
    }
}
