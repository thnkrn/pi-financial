namespace Pi.User.API.Models;

public record DuplicateUserPhoneNumber(
    int TotalUserPhoneNotHash,
    string PhoneNumber,
    List<Guid> DuplicateUserIdList
);