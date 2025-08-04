using System.ComponentModel.DataAnnotations;

namespace Pi.WalletService.Infrastructure.Options;

public class SetTradeOptions
{
    public const string Options = "SetTrade";

    [Required]
    public string Host { get; set; } = string.Empty;

    [Required]
    public string BrokerId { get; set; } = string.Empty;

    [Required]
    public string Application { get; set; } = string.Empty;

    [Required]
    public string ApiKey { get; set; } = string.Empty;

    [Required]
    public string AppSecret { get; set; } = string.Empty;

    public int TimeoutMs { get; set; }

    public int AuthMaxRetry { get; set; } = 1;
}