using System.Net;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pi.Financial.Client.Settrade;
using Pi.Financial.Client.Settrade.Api;
using Pi.Financial.Client.Settrade.Client;
using Pi.Financial.Client.Settrade.Model;
using Pi.WalletService.Application.Services.SetTrade;
using Pi.WalletService.Infrastructure.Models;
using Pi.WalletService.Infrastructure.Options;
using Pi.WalletService.Domain.Exceptions;
using Polly;

namespace Pi.WalletService.Infrastructure.Services;

public class SetTradeTransferService : ISetTradeService
{
    private readonly ISetTradeApi _setTradeApi;
    private readonly SetTradeOptions _options;
    private readonly IDistributedCache _cache;
    private readonly ILogger<SetTradeTransferService> _logger;

    public SetTradeTransferService(
        ISetTradeApi setTradeApi,
        IOptionsSnapshot<SetTradeOptions> options,
        IDistributedCache cache,
        ILogger<SetTradeTransferService> logger)
    {
        _setTradeApi = setTradeApi;
        _options = options.Value;
        _logger = logger;
        _cache = cache;
    }

    private async Task<T> WithRetryAuthToken<T>(Func<Task<T>> function)
    {
        var policy = await Policy
            .Handle<ApiException>(response => response.ErrorCode == (int)HttpStatusCode.Unauthorized)
            .RetryAsync(_options.AuthMaxRetry, async (result, retryCount, context) =>
            {
                await GenerateAccessToken();
            }).ExecuteAndCaptureAsync(function);

        if (policy.Outcome == OutcomeType.Successful)
        {
            return policy.Result;
        }

        throw policy.FinalException;
    }

    public async Task<SetTradeDepositWithdrawResponse> CashDeposit(string userId, string transactionId, string accountNo, decimal amount)
    {
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                var brokerId = _options.BrokerId;
                var accessToken = await GetAccessToken();
                var authorization = $"Bearer {accessToken}";

                // Bankers rounding amount to two decimal places
                var roundAmount = Math.Round(amount, 2, MidpointRounding.ToEven);

                // Call SetTrade DepositCash
                var depositRequest = new CashDepositRequest(roundAmount);
                _logger.LogInformation(
                    "SetTrade CashDeposit info log, Method: {Method} body: {Body}",
                    "DepositCashWithHttpInfoAsync",
                    depositRequest.ToString());
                var depositResponse = await _setTradeApi.DepositCashWithHttpInfoAsync(brokerId, accountNo, authorization, depositRequest);
                if (!depositResponse.StatusCode.Equals(HttpStatusCode.OK))
                {
                    _logger.LogError(
                        "SetTrade CashDeposit: Failed to update SetTrade deposit amount. Error: {Response}",
                        depositResponse.StatusCode);
                    return new SetTradeDepositWithdrawResponse(false, userId, transactionId, accountNo, amount, depositResponse.StatusCode.ToString());
                }

                return new SetTradeDepositWithdrawResponse(true, userId, transactionId, accountNo, roundAmount);
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SetTrade CashDeposit: Failed to update SetTrade deposit amount. Error: {Message}", e.Message);
            throw new SetTradeDepositException("Internal Errors.");
        }
    }

    public async Task<decimal> GetWithdrawalBalance(string accountNo)
    {
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                var brokerId = _options.BrokerId;
                var accessToken = await GetAccessToken();
                var authorization = $"Bearer {accessToken}";
                var accountInfoResponse = await _setTradeApi.GetAccountInfoAsync(brokerId, accountNo, authorization);
                return accountInfoResponse.ExcessEquity;
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SetTrade GetWithdrawalBalance: Failed to get SetTrade account withdrawal balance. Error: {Message}", e.Message);
            throw new SetTradeAccountInfoException("Internal Errors.");
        }
    }

    public async Task<SetTradeDepositWithdrawResponse> CashWithdraw(string userId, string transactionId, string accountNo, decimal amount)
    {
        try
        {
            return await WithRetryAuthToken(async () =>
            {
                // Bankers rounding amount to two decimal places
                var roundAmount = Math.Round(amount, 2, MidpointRounding.ToEven);

                // Get SetTrade account info to check cash balance if it is enough to withdraw
                var withdrawalBalance = await GetWithdrawalBalance(accountNo);

                if (withdrawalBalance < roundAmount)
                {
                    _logger.LogError("SetTrade account cash balance is less than the requested amount. " + "Cash balance: {CashBalance}", withdrawalBalance);
                    return new SetTradeDepositWithdrawResponse(false, userId, transactionId, accountNo, amount, null, $"SetTrade account cash balance is less than the requested amount. Cash balance: {withdrawalBalance}");
                }

                var brokerId = _options.BrokerId;
                var accessToken = await GetAccessToken();
                var authorization = $"Bearer {accessToken}";

                // Call SetTrade WithdrawCash
                var withdrawRequest = new CashWithdrawalRequest(roundAmount);
                var withdrawResponse = await _setTradeApi.WithdrawCashWithHttpInfoAsync(brokerId, accountNo, authorization, withdrawRequest);
                if (!withdrawResponse.StatusCode.Equals(HttpStatusCode.OK))
                {
                    _logger.LogError(
                        "SetTrade CashWithdraw: Failed to update SetTrade withdrawal amount with Status {StatusCode}",
                        withdrawResponse.StatusCode);
                    return new SetTradeDepositWithdrawResponse(false, userId, transactionId, accountNo, amount, withdrawResponse.StatusCode.ToString());
                }

                return new SetTradeDepositWithdrawResponse(true, userId, transactionId, accountNo, roundAmount);
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SetTrade CashWithdraw: Failed to update SetTrade withdrawal amount. Error: {Message}", e.Message);
            throw new SetTradeWithdrawException("Internal Errors.");
        }
    }

    public async Task<string> GetAccessToken()
    {
        var accessToken = await _cache.GetStringAsync(CacheKeys.SetTradeAccessToken);
        if (accessToken != null)
        {
            return accessToken;
        }

        return await GenerateAccessToken();
    }

    public async Task<string> GenerateAccessToken()
    {
        // get new access token from SetTrade and update cache
        var authTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var authResponse = await Auth("", authTime);
        var expireIn = authResponse.ExpiresIn;
        var accessToken = authResponse.AccessToken;

        var cacheEntryOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromSeconds(expireIn));

        await _cache.SetStringAsync(CacheKeys.SetTradeAccessToken, accessToken, cacheEntryOptions);

        return accessToken;
    }

    private async Task<SetTradeAuthResponse> Auth(string authParam, long timestamp)
    {
        try
        {
            var brokerId = _options.BrokerId;
            var appCode = _options.Application;
            var apiKey = _options.ApiKey;
            var appSecret = _options.AppSecret;
            var signature = SetTradeSignature.CreateSignature(appSecret, apiKey, authParam, timestamp);

            var authRequest = new SettradeAuthRequest(apiKey, authParam, signature, timestamp.ToString());
            var authResponse = await _setTradeApi.AuthLoginAsync(brokerId, appCode, authRequest);

            return new SetTradeAuthResponse(true, authResponse.TokenType, authResponse.AccessToken, authResponse.RefreshToken, authResponse.ExpiresIn);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "SetTrade Authentication: Failed to log into SetTrade. Error: {Message}", e.Message);
            throw new SetTradeAuthException("Internal Errors.");
        }
    }

}
