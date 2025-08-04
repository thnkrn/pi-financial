namespace Pi.SetMarketData.Application.Commands.NavList;

public record UpdateNavListResponse(bool Success, Domain.Entities.NavList? UpdatedNavList = null);