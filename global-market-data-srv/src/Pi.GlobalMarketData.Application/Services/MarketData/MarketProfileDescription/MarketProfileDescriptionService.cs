using System.Text.RegularExpressions;
using Pi.GlobalMarketData.Domain.Entities;
using Pi.GlobalMarketData.Domain.Models.Response;

namespace Pi.GlobalMarketData.Application.Services.MarketData.MarketProfileDescription;

public static class MarketProfileDescriptionService
{
    public static MarketProfileDescriptionResponse GetResult(
        MorningStarStocks? morningStarStocks,
        MorningStarEtfs? morningStarEtfs,
        string amcLogo
    )
    {
        // ETFs Case
        if (morningStarEtfs != null)
            return new MarketProfileDescriptionResponse
            {
                Code = "0",
                Message = string.Empty,
                Response = new ProfileDescriptionResponse
                {
                    Description = morningStarEtfs.Description,
                    WebsiteLink = morningStarEtfs.Website,
                    WebsiteLinkName = morningStarEtfs.Website,
                    AmcCode = morningStarEtfs.Manager,
                    AmcLogo = amcLogo,
                    AmcFriendlyName = morningStarEtfs.Manager
                }
            };

        // Stocks Case
        var morningStarStock = morningStarStocks ?? new MorningStarStocks();
        var websiteLink = morningStarStock.Website;
        var websiteLinkName = Regex.Replace(
            websiteLink,
            @"^https?://",
            string.Empty,
            RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(3)
        );

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
                AmcLogo = string.Empty,
                AmcFriendlyName = string.Empty
            }
        };
    }
}