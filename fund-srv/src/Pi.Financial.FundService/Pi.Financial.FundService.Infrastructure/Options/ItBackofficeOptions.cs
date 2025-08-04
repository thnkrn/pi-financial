using Elastic.Apm.Api.Constraints;

namespace Pi.Financial.FundService.Infrastructure.Options;

public class ItBackofficeOptions
{
    public const string Options = "ItBackoffice";

    [Required]
    public string Host { get; set; } = string.Empty;
    public int TimeoutMS { get; set; }
    public short MaxRetry { get; set; }
    public Guid? ApiKey { get; set; }
    public int CacheExpiration { get; set; } = 60;
}
