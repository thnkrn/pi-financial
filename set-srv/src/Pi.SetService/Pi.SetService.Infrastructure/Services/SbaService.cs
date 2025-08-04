using System.Runtime.CompilerServices;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Pi.SetService.Application.Models.InitialMargin;
using Pi.SetService.Application.Services.SbaService;

namespace Pi.SetService.Infrastructure.Services;

public class SbaService(IAmazonS3 client) : ISbaService
{
    public async IAsyncEnumerable<MarginRateInfo> GetMarginRatesFromStorage(string bucketName, string fileKey, [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var line in ReadLinesFromS3(bucketName, fileKey, ct))
        {
            var lineBytes = Encoding.UTF8.GetBytes(line);
            if (lineBytes.Length < 6)
            {
                continue;
            }

            yield return new MarginRateInfo
            {
                MarginCode = Encoding.UTF8.GetString(lineBytes[..3]).Trim(),
                MarginRate = Convert.ToDecimal(Encoding.UTF8.GetString(lineBytes[3..]).Trim())
            };
        }
    }

    public async IAsyncEnumerable<MarginInstrumentInfo> GetMarginInstrumentInfoFromStorage(string bucketName, string fileKey, [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var line in ReadLinesFromS3(bucketName, fileKey, ct))
        {
            var lineBytes = Encoding.UTF8.GetBytes(line);
            if (lineBytes.Length < 50)
            {
                continue;
            }

            yield return new MarginInstrumentInfo
            {
                Symbol = Encoding.UTF8.GetString(lineBytes[..20]).Trim(),
                MarginCode = Encoding.UTF8.GetString(lineBytes[20..23]).Trim(),
                IsTurnoverList = !Encoding.UTF8.GetString(lineBytes[49..50]).Trim().Equals("N"
, StringComparison.CurrentCultureIgnoreCase)
            };
        }
    }

    private async IAsyncEnumerable<string> ReadLinesFromS3(string bucketName, string fileKey, [EnumeratorCancellation] CancellationToken ct = default)
    {
        await using var stream = await GetObjectAsync(bucketName, fileKey);
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            if (ct.IsCancellationRequested)
                yield break;

            var line = await reader.ReadLineAsync(ct);
            if (line != null)
            {
                yield return line;
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
