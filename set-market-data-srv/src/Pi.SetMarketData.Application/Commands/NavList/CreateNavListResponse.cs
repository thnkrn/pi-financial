namespace Pi.SetMarketData.Application.Commands.NavList;

public record CreateNavListResponse(bool Success, Domain.Entities.NavList? CreatedNavList = null);