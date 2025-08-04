using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SecurityToken;
using Amazon.SecurityToken.Model;
using Microsoft.Extensions.Options;
using Pi.StructureNotes.Infrastructure.Models;

namespace Pi.StructureNotes.Infrastructure.Services;

public class S3NoteReader : INoteFileReader
{
    private readonly S3NoteReaderConfig _config;
    private readonly Regex _regex;

    public S3NoteReader(IOptions<S3NoteReaderConfig> options)
    {
        _config = options.Value;
        string prefix = _config.Prefix;
        if (!prefix.EndsWith("/"))
        {
            prefix = $"{prefix}/";
        }

        _regex = new Regex(
            @$"^{prefix}(?<type>{FileType.Note}|{FileType.Stock}|{FileType.Cash}).*(?<date>\d{{4}}-\d{{2}}-\d{{2}}).*\.csv$");
    }

    public async IAsyncEnumerable<(FileType Type, string File)> GetFilesToRead(DateTime utc,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        using AmazonS3Client client = await CreateClient(ct);
        string bucket = _config.Bucket;
        string prefix = _config.Prefix;
        if (!prefix.EndsWith("/"))
        {
            prefix = $"{prefix}/";
        }

        ListObjectsV2Response resp =
            await client.ListObjectsV2Async(new ListObjectsV2Request { BucketName = bucket, Prefix = prefix }, ct);

        List<S3Object> objs = resp.S3Objects;
        List<(FileType Type, DateOnly Date, DateTime LastUpdated, string File)> all =
            new List<(FileType Type, DateOnly Date, DateTime LastUpdated, string File)>();
        foreach (S3Object obj in objs)
        {
            string key = obj.Key;
            if (key.EndsWith("/"))
            {
                continue;
            }

            DateTime objDate = obj.LastModified;
            if (objDate < utc)
            {
                continue;
            }

            Match match = _regex.Match(key);
            if (!match.Success)
            {
                continue;
            }

            string typeStr = match.Groups["type"].Value;
            if (!Enum.TryParse<FileType>(typeStr, out FileType type))
            {
                continue;
            }

            DateOnly fileDate = DateOnly.Parse(match.Groups["date"].Value);

            all.Add((type, fileDate, objDate, key));
        }

        IEnumerable<(FileType Type, DateOnly Date, DateTime LastUpdated, string File)> result = all
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.LastUpdated)
            .GroupBy(x => new { x.Type, x.Date })
            .Select(x => x.First())
            .GroupBy(x => x.Type)
            .Select(x => x.First());

        foreach ((FileType Type, DateOnly Date, DateTime LastUpdated, string File) item in result)
        {
            yield return (item.Type, item.File);
        }
    }

    public async Task<Stream> ReadFileAsync(string file, CancellationToken ct = default)
    {
        string bucket = _config.Bucket;
        using AmazonS3Client client = await CreateClient(ct);
        GetObjectResponse objResp =
            await client.GetObjectAsync(new GetObjectRequest { BucketName = bucket, Key = file }, ct);
        Stream stream = objResp.ResponseStream;
        return stream;
    }

    private async Task<AmazonS3Client> CreateClient(CancellationToken ct)
    {
        string role = _config.RoleArn;
        string key = _config.Key;
        string secret = _config.Secret;

        AssumeRoleRequest roleReq = new AssumeRoleRequest { RoleArn = role, RoleSessionName = "SnWorkerSession" };

        BasicAWSCredentials cred = new BasicAWSCredentials(key, secret);

        using AmazonSecurityTokenServiceClient tokenClient = new AmazonSecurityTokenServiceClient(cred);

        AssumeRoleResponse resp = await tokenClient.AssumeRoleAsync(roleReq, ct);

        AmazonS3Client client = new AmazonS3Client(
            resp.Credentials.AccessKeyId,
            resp.Credentials.SecretAccessKey,
            resp.Credentials.SessionToken, new AmazonS3Config { RegionEndpoint = RegionEndpoint.APSoutheast1 }
        );

        return client;
    }
}
