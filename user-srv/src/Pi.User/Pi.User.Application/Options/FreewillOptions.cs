using System.ComponentModel.DataAnnotations;

namespace Pi.User.Application.Options;

public class FreewillOptions
{
    public const string Options = "Freewill";

    [Required]
    public string Host { get; set; } = string.Empty;

    [Required]
    public string Requester { get; set; } = string.Empty;

    [Required]
    public string Application { get; set; } = string.Empty;

    [Required]
    public string KeyBase { get; set; } = string.Empty;

    [Required]
    public string IvCode { get; set; } = string.Empty;

    public int MaxRetry { get; set; }
    public int TimeoutMS { get; set; }
}