namespace Pi.WalletService.Application.Models;

public record PaginateRequest(int PageNo, int PageSize, string? OrderBy, string? OrderDirection);