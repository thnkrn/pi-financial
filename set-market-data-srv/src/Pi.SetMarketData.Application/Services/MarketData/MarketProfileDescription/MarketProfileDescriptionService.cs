using System.Text.RegularExpressions;
using Pi.SetMarketData.Domain.Entities;
using Pi.SetMarketData.Domain.Models.Response;

namespace Pi.SetMarketData.Application.Services.MarketData.MarketProfileDescription;

public static class MarketProfileDescriptionService
{
    public static MarketProfileDescriptionResponse GetResult(MorningStarStocks? morningStarStocks, string amcLogo)
    {
        var morningStarStock = morningStarStocks ?? new MorningStarStocks();
        var websiteLink = morningStarStock.Website;
        var websiteLinkName = Regex.Replace(websiteLink, @"^https?://", "", default, TimeSpan.FromSeconds(1));

        return new MarketProfileDescriptionResponse
        {
            Code = "0",
            Message = string.Empty,
            Response = new ProfileDescriptionResponse
            {
                Description = morningStarStock.Description,
                WebsiteLink = websiteLink,
                WebsiteLinkName = websiteLinkName,
                AmcCode = string.Empty,
                AmcLogo = amcLogo,
                AmcFriendlyName = string.Empty
            }
        };
    }
}