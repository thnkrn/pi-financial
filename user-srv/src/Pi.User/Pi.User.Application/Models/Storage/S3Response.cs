namespace Pi.User.Application.Models.Storage;

public record S3Response(bool IsSuccess, int StatusCode, string Message);