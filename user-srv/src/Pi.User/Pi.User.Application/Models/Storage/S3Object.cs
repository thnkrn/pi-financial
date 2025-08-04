namespace Pi.User.Application.Models.Storage;

public record S3Object(string FileName, string BucketName, MemoryStream InputStream, string? ContentType);