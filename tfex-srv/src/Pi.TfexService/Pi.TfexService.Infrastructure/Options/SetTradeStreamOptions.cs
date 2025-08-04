using System.ComponentModel.DataAnnotations;

namespace Pi.TfexService.Infrastructure.Options;

public class SetTradeStreamOptions
{
    public const string Options = "SetTradeStream";

    [Required]
    public string Path { get; set; } = string.Empty;

    [Required]
    public List<string> Topic { get; set; } = [];

    public int ConnectionDelaySeconds { get; set; } = 5;

    public int HealthCheckSeconds { get; set; } = 300;
}