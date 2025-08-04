using System.ComponentModel.DataAnnotations;

namespace Pi.User.Application.Options;

public class AwsS3Option
{
    public const string Options = "AwsS3";

    [Required]
    public string AccessKey { get; set; } = string.Empty;

    [Required]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public string DocumentBucketName { get; set; } = string.Empty;
}