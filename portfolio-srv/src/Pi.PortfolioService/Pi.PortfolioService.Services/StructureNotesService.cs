using Microsoft.Extensions.Logging;
using Pi.Client.StructureNotes.Api;
using Pi.Common.ExtensionMethods;
using Pi.PortfolioService.Application.Services.Models.StructureNote;

namespace Pi.PortfolioService.Services;

public class StructureNoteService : IStructureNoteService
{
    private readonly INoteApi _noteApi;
    private ILogger<StructureNoteService> _logger;

    public StructureNoteService(INoteApi noteApi, ILogger<StructureNoteService> logger)
    {
        _noteApi = noteApi;
        _logger = logger;
    }

    public async Task<IEnumerable<StructureNoteAccountSummary>?> GetStructureNotes(
        string userId,
        string currency,
        CancellationToken ct = default
    )
    {
        try
        {
            var apiResponse = await _noteApi.InternalNotesCurrencyGetAsync(userId, currency, ct);

            var accountType = PortfolioAccountType.Offshore.ToString();
            var accountNotes = apiResponse.Data.AccountNotes;
            var failedAccounts = apiResponse.Data.FailedToFetchAccounts;
            var error =
                "Value not updated, The investment value could not updated due to a data retrieval error.";
            var result = failedAccounts
                .Select(
                    account =>
                        new StructureNoteAccountSummary(
                            accountType,
                            account.AccountId,
                            account.AccountNo,
                            account.CustCode,
                            false,
                            0,
                            0,
                            error
                        )
                )
                .ToList();

            result.AddRange(
                accountNotes.Select(account =>
                {
                    var summaries = new[]
                    {
                        account.NoteSummary,
                        account.StockSummary,
                        account.CashSummary
                    };
                    var totalMarketValue = account.OverallSummary.TotalMarketValue ?? 0;
                    var totalCost = account.OverallSummary.TotalCostValue ?? 0;

                    var upnl = totalMarketValue - totalCost;

                    var accountType = PortfolioAccountType.Offshore.ToString();

                    var result = new StructureNoteAccountSummary(
                        accountType,
                        account.AccountId,
                        account.AccountNo,
                        account.CustCode,
                        false,
                        totalMarketValue,
                        upnl,
                        string.Empty
                    );

                    return result;
                })
            );

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Structure Notes data, userId: {userId}", userId);
            return null;
        }
    }

    public async Task<IEnumerable<PortfolioAccount>> GetStructureNotesPortfolioAccount(string userId, string currency,
        CancellationToken ct = default)
    {
        try
        {
            var apiResponse = await _noteApi.InternalNotesCurrencyGetAsync(userId, currency, ct);
            var accountType = PortfolioAccountType.Offshore.ToString();
            const string error =
                "Value not updated, The investment value could not updated due to a data retrieval error.";

            return apiResponse.Data.AccountNotes
                .Select(a =>
                {
                    var totalMarketValue = a.OverallSummary.TotalMarketValue ?? 0;
                    var totalCost = a.OverallSummary.TotalCostValue ?? 0;

                    var upnl = totalMarketValue - totalCost;

                    var result = new PortfolioAccount(
                        accountType,
                        a.AccountId,
                        a.AccountNo,
                        a.AccountNo,
                        a.CustCode,
                        false,
                        totalMarketValue,
                        0,
                        upnl,
                        string.Empty
                    );
                    return result;
                })
                .Concat(apiResponse.Data.FailedToFetchAccounts
                    .Select(a => new PortfolioAccount(
                        accountType,
                        a.AccountId,
                        a.AccountNo,
                        a.AccountNo,
                        a.CustCode,
                        false,
                        0,
                        0,
                        0,
                        error)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get Structure Notes data, userId: {userId}", userId);
            return new List<PortfolioAccount>();
        }
    }
}