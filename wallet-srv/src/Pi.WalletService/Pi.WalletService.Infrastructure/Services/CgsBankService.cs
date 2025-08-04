using System.Globalization;
using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Client.CgsBank.Api;
using Pi.Client.CgsBank.Client;
using Pi.Client.CgsBank.Model;
using Pi.WalletService.Application.Options;
using Pi.WalletService.Application.Services.Bank;
using Pi.WalletService.Infrastructure.Models;
using Polly.Timeout;
using DDPaymentResponse = Pi.WalletService.Application.Services.Bank.DDPaymentResponse;
using QRPaymentResponse = Pi.WalletService.Application.Services.Bank.QRPaymentResponse;
namespace Pi.WalletService.Infrastructure.Services;

public class CgsBankService : IBankService
{
    private readonly ICgsBankApi _cgsBankApi;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CgsBankService> _logger;
    private readonly IOptions<OnlineDirectDebitOptions> _oddOptions;
    private readonly IDistributedCache _cache;
    private readonly string _apiKey;
    private readonly bool _useMockValues;

    public CgsBankService(
        ICgsBankApi cgsBankApi,
        IConfiguration configuration,
        IMemoryCache memoryCache,
        ILogger<CgsBankService> logger,
        IOptions<OnlineDirectDebitOptions> oddOptions,
        IDistributedCache cache)
    {
        _cgsBankApi = cgsBankApi;
        _memoryCache = memoryCache;
        _logger = logger;
        _oddOptions = oddOptions;
        _cache = cache;
        _apiKey = configuration["CgsBank:ApiKey"] ?? string.Empty;
        _useMockValues = configuration.GetValue<bool>("CgsBank:UseMockValues");
    }

    public async Task<QRPaymentResponse> GenerateQR(
        string transactionNo,
        decimal amount,
        string transactionRefCode,
        string customerCode,
        string product,
        int expiredTimeInMinute)
    {
        if (_useMockValues)
        {
            // randomly selected 5 THB qr from db
            return new QRPaymentResponse(
                null,
                null,
                string.Empty,
                string.Empty,
                true,
                "00020101021230760016A00000067701011201150107537000572010220QR2023071801341909130309231990934530376454045.005802TH62240720RF2023071801341909136304F2D0"
            );
        }

        var authToken = await GetAuthorizationHeader();

        try
        {
            var request = new QRPaymentRequest(
                transactionNo,
                amount,
                transactionRefCode,
                expiredTimeInMinute,
                new QRPaymentRequestInternalRef(customerCode, product));

            var response = await _cgsBankApi.KKPPaymentGenerateQRPostWithHttpInfoAsync(request, authToken);

            CheckIfForbiddenRemoveCachedToken(response.StatusCode);

            return new QRPaymentResponse(
                response.Data.Status.ExternalStatusCode,
                response.Data.Status.ExternalStatusDescription,
                response.Data.Status.InternalStatusCode,
                response.Data.Status.InternalStatusDescription,
                response.Data.Status.Status,
                response.Data.Data.QRValue);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CGSBankService: Unable to Get KKPPaymentGenerateQR. {Exception}", e.Message);
            throw new UnableToQrPaymentException(e.Message);
        }
    }

    public async Task<OnlineDirectDebitRegistrationResult> RegisterOnlineDirectDebit(string citizenId, string refCode, string oddBank, CancellationToken cancellationToken)
    {
        var request = new ODDRegistrationRequest(refCode, citizenId);
        try
        {
            var authToken = await GetAuthorizationHeader();
            ApiResponse<ODDRegistrationResponse> response;
            switch (oddBank.ToUpper(CultureInfo.InvariantCulture))
            {
                case "SCB":
                    request.RedirectUrl = _oddOptions.Value.RegistrationCallbackUrl["SCB"];
                    response = await _cgsBankApi.SCBPaymentRegistrationPostWithHttpInfoAsync(request, authToken, cancellationToken);
                    break;
                case "KBANK":
                    request.RedirectUrl = _oddOptions.Value.RegistrationCallbackUrl["KBANK"];
                    response = await _cgsBankApi.KBANKPaymentRegistrationPostWithHttpInfoAsync(request, authToken, cancellationToken);
                    break;
                default:
                    throw new InvalidOperationException("ODD Bank is invalid");
            }

            CheckIfForbiddenRemoveCachedToken(response.StatusCode);

            return new OnlineDirectDebitRegistrationResult(response.Data.Data.WebUrl);
        }
        catch (ApiException ex)
        {
            var errorContent = ex.ErrorContent.ToString() ?? "no content";
            var errorMessage = $"Failed to send register ODD request ({oddBank}): {errorContent}";
            _logger.LogError(ex, "Failed to send register ODD request ({OddBank}): {ErrorContent}", oddBank, errorContent);
            throw new BankOperationException(errorMessage, ex);
        }
    }

    public async Task<DDPaymentResponse> WithdrawViaAts(
        string transactionNo,
        string transactionRefCode,
        string accountNo,
        string destinationBankCode,
        decimal amount,
        string customerCode,
        string product)
    {
        if (_useMockValues)
        {
            return new DDPaymentResponse(
                null,
                null,
                string.Empty,
                string.Empty,
                true,
                transactionNo,
                transactionRefCode,
                amount,
                DateTime.Now.ToString(CultureInfo.InvariantCulture),
                string.Empty
            );
        }

        var authToken = await GetAuthorizationHeader();
        try
        {
            var request = new DDPaymentRequest(
                transactionNo,
                transactionRefCode,
                accountNo,
                destinationBankCode,
                amount,
                new QRPaymentRequestInternalRef(customerCode, product));

            var cacheEntryOptions = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1));

            await _cache.SetStringAsync(transactionNo, transactionRefCode, cacheEntryOptions);

            var response = await _cgsBankApi.KKPPaymentPaymentPostWithHttpInfoAsync(request, authToken);

            CheckIfForbiddenRemoveCachedToken(response.StatusCode);

            return new DDPaymentResponse(
                response.Data.Status.ExternalStatusCode,
                response.Data.Status.ExternalStatusDescription,
                response.Data.Status.InternalStatusCode,
                response.Data.Status.InternalStatusDescription,
                response.Data.Status.Status,
                response.Data.Data.TransactionNo,
                response.Data.Data.TransactionRefCode,
                response.Data.Data.Amount,
                response.Data.Data.ExternalRefTime,
                response.Data.Data.ExternalRefCode
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CGSBankService: Unable to Call KKPPaymentPayment. {Exception}", e.Message);
            if (e is TimeoutRejectedException)
            {
                throw;
            }
            throw new UnableToWithdrawViaAtsException(e.Message);
        }
    }

    private async Task<string> GetAuthorizationHeader()
    {
        if (_memoryCache.TryGetValue(CacheKeys.CgsBankAuthToken, out string? cacheValue) && !string.IsNullOrWhiteSpace(cacheValue))
        {
            return cacheValue;
        }

        GetTokenResponse? authResponse;
        try
        {
            authResponse = await _cgsBankApi.GetTokenPostAsync(_apiKey);
        }
        catch (Exception e)
        {
            throw new InvalidDataException("CGSBankService: Error Call GetTokenAsync", e);
        }

        if (authResponse == null || string.IsNullOrWhiteSpace(authResponse.Body.Token))
        {
            throw new UnauthorizedAccessException("CGSBankService: Authentication Response is Empty");
        }

        cacheValue = $"Bearer {authResponse.Body.Token}";

        if (string.IsNullOrEmpty(cacheValue))
        {
            throw new UnauthorizedAccessException("CGSBankService: Unable to Get API Response Token");
        }

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(authResponse.Body.ExpiresIn - 300));

        _memoryCache.Set(CacheKeys.CgsBankAuthToken, cacheValue, cacheEntryOptions);

        return cacheValue;
    }

    private void CheckIfForbiddenRemoveCachedToken(HttpStatusCode statusCode)
    {
        if (statusCode == HttpStatusCode.Forbidden)
        {
            _memoryCache.Remove(CacheKeys.CgsBankAuthToken);
        }
    }
}
