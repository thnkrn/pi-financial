namespace Pi.SetService.Infrastructure.Options;

public class DatabaseOptions
{
    public const string Options = "Database";
    public required string AesKey { get; set; }
    public required string AesIV { get; set; }
}
