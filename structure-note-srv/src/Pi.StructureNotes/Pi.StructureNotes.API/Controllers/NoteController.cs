using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Pi.Common.Features;
using Pi.Common.Http;
using Pi.StructureNotes.API.Models;

namespace Pi.StructureNotes.API.Controllers;

public class NoteController : ControllerBase
{
    private const string _logoUrl = "https://d34vubfbkpay0j.cloudfront.net/";
    private const string SiriusMigration = "sirius-structure-note-market-data-migration";

    private readonly IAccountService _accService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger _logger;
    private readonly IMarketService _marketService;
    private readonly INoteRepository _repo;
    private readonly IMarketDataService _marketDataService;
    private readonly IFeatureService _featureService;

    public NoteController(
        IWebHostEnvironment env,
        IAccountService accService,
        IMarketService marketService,
        INoteRepository repo,
        IMarketDataService marketDataService,
        IFeatureService featureService,
        ILogger<NoteController> logger)
    {
        _env = env;
        _accService = accService;
        _marketService = marketService;
        _repo = repo;
        _marketDataService = marketDataService;
        _featureService = featureService;
        _logger = logger;
    }

    [HttpGet("secure/notes/{accountId}/{currency}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<AccountNotes>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccountNotes(
        [FromHeader(Name = "user-id")][Required] string userId,
        [FromRoute] GetAccountNotesRequest request, CancellationToken ct = default)
    {
        try
        {
            string accountId = request.AccountId;
            string currency = request.Currency;
            AccountInfo account = await _accService.GetSnAccountById(userId, accountId, ct);
            if (account == null)
            {
                return Unauthorized();
            }

            AccountNotes sNotes = await _repo.GetStructuredNotes(account, ct);
            if (sNotes == null)
            {
                return NotFound();
            }

            IEnumerable<string> currencies = sNotes.GetCurrencies().Where(c => c != currency);
            IExchangeRateLookup lookUp = await _marketService.GetExchangeLookup(currencies, currency, ct);

            await SetStockLatestPrices(sNotes.Stocks, ct);

            sNotes.SetCurrency(currency, lookUp);
            sNotes.CalculateSummaries();

            ApiResponse<AccountNotes> response = new ApiResponse<AccountNotes>(sNotes);

            return Ok(response);
        }
        catch (AggregateException ex)
        {
            ReadOnlyCollection<Exception> exs = ex.InnerExceptions;
            StringBuilder sb = new StringBuilder();
            sb.Append($"Aggregate Exception: {ex.Message}, Count: {exs.Count}, Details:");
            foreach (Exception e in exs)
            {
                sb.Append($"Exception:{e.Message}");
                if (e.InnerException != null)
                {
                    sb.Append($".Inner Exception: {e.InnerException.Message}");
                }

                sb.Append(".");
            }

            string error = sb.ToString();
            _logger.LogError(error);

            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail: error);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail: e.Message);
        }
    }

    [HttpGet("internal/notes/{currency}")]
    [HttpGet("secure/notes/{currency}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<MultiAccountNotes>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
    public async Task<IActionResult> GetAccountNotes(
        [FromHeader(Name = "user-id")][Required] string userId,
        [FromRoute] GetNotesRequest request, CancellationToken ct = default)
    {
        try
        {
            string currency = request.Currency;
            IEnumerable<AccountInfo> accounts = await _accService.GetSnAccounts(userId, ct);
            MultiAccountNotes result = new MultiAccountNotes();
            foreach (AccountInfo account in accounts)
            {
                try
                {
                    AccountNotes sNotes = await _repo.GetStructuredNotes(account, ct);
                    if (sNotes == null)
                    {
                        continue;
                    }

                    IEnumerable<string> currencies = sNotes.GetCurrencies().Where(c => c != currency);
                    IExchangeRateLookup lookUp = await _marketService.GetExchangeLookup(currencies, currency, ct);

                    await SetStockLatestPrices(sNotes.Stocks, ct);

                    sNotes.SetCurrency(currency, lookUp);
                    sNotes.CalculateSummaries();

                    result.AddAccountNotes(sNotes);
                }
                catch (AggregateException ex)
                {
                    ReadOnlyCollection<Exception> exs = ex.InnerExceptions;
                    StringBuilder sb = new StringBuilder();
                    sb.Append($"Aggregate Exception: {ex.Message}, Count: {exs.Count}, Details:");
                    foreach (Exception e in exs)
                    {
                        sb.Append($"Exception:{e.Message}");
                        if (e.InnerException != null)
                        {
                            sb.Append($".Inner Exception: {e.InnerException.Message}");
                        }

                        sb.Append(".");
                    }

                    string error = sb.ToString();
                    _logger.LogError(error);

                    return Problem(statusCode: StatusCodes.Status500InternalServerError,
                        title: ErrorCodes.InternalServerError.ToUpper(),
                        detail: error);
                }
                catch (Exception ex)
                {
                    string accId = account.AccountId;
                    string error = $"Could not get structure notes for account {accId}, Exception: {ex.Message}";
                    if (_env.IsProduction())
                    {
                        result.AddFailedAccount(account, null);
                    }
                    else
                    {
                        result.AddFailedAccount(account, error);
                    }

                    _logger.LogError(error, accId);
                }
            }

            result.CalculateSummaries();

            ApiResponse<MultiAccountNotes> response = new ApiResponse<MultiAccountNotes>(result);
            return Ok(response);
        }
        catch (AggregateException ex)
        {
            ReadOnlyCollection<Exception> exs = ex.InnerExceptions;
            StringBuilder sb = new StringBuilder();
            sb.Append($"Aggregate Exception: {ex.Message}, Count: {exs.Count}, Details:");
            foreach (Exception e in exs)
            {
                sb.Append($"Exception:{e.Message}");
                if (e.InnerException != null)
                {
                    sb.Append($".Inner Exception: {e.InnerException.Message}");
                }

                sb.Append(".");
            }

            string error = sb.ToString();
            _logger.LogError(error);

            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail: error);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
            return Problem(statusCode: StatusCodes.Status500InternalServerError,
                title: ErrorCodes.InternalServerError.ToUpper(),
                detail: e.Message);
        }
    }

    private async Task SetStockLatestPrices(IEnumerable<Stock> stocks, CancellationToken ct)
    {
        var siriusMigration = _featureService.IsOn(SiriusMigration);
        List<Task> tasks = new List<Task>();
        foreach (Stock stock in stocks)
        {
            tasks.Add(SetStockPrice(stock, siriusMigration, ct));
        }

        await Task.WhenAll(tasks);
    }

    private async Task SetStockPrice(Stock stock, bool siriusMigration, CancellationToken ct)
    {
        try
        {
            StockPrice marketPrice = null;
            try
            {
                string symbol = stock.Symbol;
                string currency = stock.Currency;
                marketPrice = await _marketService.GetStockPrice(symbol, currency, ct);
                if (marketPrice != null)
                {
                    stock.TrySetLogo(_logoUrl);
                    stock.SetStockPrice(marketPrice);
                }
                else
                {
                    _logger.LogWarning($"Could not find price for {stock.Symbol}, Provider returned null");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error with setting stock price: {stock.Symbol}, Error: {ex.Message}");
            }
            finally
            {
                if (marketPrice == null || marketPrice.Value == 0)
                {
                    decimal lastPrice;
                    if (siriusMigration)
                        lastPrice = await _marketDataService.GetLastSessionClosePrice(stock.Symbol, ct);
                    else
                        lastPrice = await _marketDataService.GetSiriusLastSessionClosePrice(stock.Symbol, ct);

                    marketPrice = new StockPrice(stock.Currency, lastPrice);
                    stock.TrySetLogo(_logoUrl);
                    stock.SetStockPrice(marketPrice);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error with setting stock price: {stock.Symbol}, Error: {ex.Message}");
        }
    }
}
