namespace Pi.User.API.Models;

public record CreateUserRequest(
    Guid? Id,
    string CustomerId,
    string? Email,
    string? Mobile,
    string? CitizenId,
    string? FirstnameTh,
    string? LastnameTh,
    string? FirstnameEn,
    string? LastnameEn,
    string? WealthType
);