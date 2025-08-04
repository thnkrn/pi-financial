using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Pi.Client.ExanteUserManagement.Api;
using Pi.Client.ExanteUserManagement.Client;
using Pi.Client.ExanteUserManagement.Model;
using Pi.WalletService.Application.Commands.GlobalWalletTransfer;
using Pi.WalletService.Application.Services.GlobalEquities;
using Pi.WalletService.Infrastructure.Models;
using TransferResponse = Pi.WalletService.Application.Services.GlobalEquities.TransferResponse;

namespace Pi.WalletService.Infrastructure.Services;

public class ExanteUserManagementService : IGlobalUserManagementService
{
    private readonly IExanteUserManagementApi _exanteUserManagementApi;
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<ExanteUserManagementService> _logger;
    private readonly string _username;
    private readonly string _password;

    public ExanteUserManagementService(
        IExanteUserManagementApi exanteUserManagementApi,
        IConfiguration configuration,
        IMemoryCache memoryCache,
        ILogger<ExanteUserManagementService> logger)
    {
        _exanteUserManagementApi = exanteUserManagementApi;
        _memoryCache = memoryCache;
        _logger = logger;
        _username = configuration["Exante:UserManagementUsername"] ?? string.Empty;
        _password = configuration["Exante:UserManagementPassword"] ?? string.Empty;
    }

    public async Task<TransferResponse> TransferMoney(string sourceAccountId, string targetAccountId, string currency, decimal amount)
    {
        return await ExecuteTransferMoney(sourceAccountId, targetAccountId, currency, amount, retryOn401: true);
    }

    private async Task<TransferResponse> ExecuteTransferMoney(string sourceAccountId, string targetAccountId, string currency, decimal amount, bool retryOn401 = true)
    {
        await Login();

        try
        {
            var response = await _exanteUserManagementApi.EnClientsareaAccountWithdrawalCreatePostAsync(new TransferRequest(sourceAccountId, targetAccountId, currency, amount));
            return new TransferResponse(response.AccountId, response.SecondAccount, response.Asset, response.Amount, response.SequenceId);
        }
        catch (ApiException e)
        {
            _logger.LogError(
                e,
                "Error Calling ExanteUserManagementService. ErrorCode: {ErrorCodes}, Content: {ErrorContent}",
                e.ErrorCode,
                e.ErrorContent);

            switch (e.ErrorCode)
            {
                case 400:
                    {
                        var errorContent = JsonConvert.DeserializeObject<TransferErrorResponse>((string)e.ErrorContent);
                        if (errorContent is { Success: false, Errors: not null } &&
                            errorContent.Errors.TryGetValue("amount", out var amountErrorMessage) &&
                            amountErrorMessage.StartsWith("Transfer has not been completed."))
                        {
                            throw new TransferInsufficientBalanceException("Insufficient account balance");
                        }
                        break;
                    }
                case 401:
                    InvalidateAuthHeaderCache();
                    if (retryOn401)
                    {
                        return await ExecuteTransferMoney(sourceAccountId, targetAccountId, currency, amount, retryOn401: false);
                    }
                    break;
            }
            throw;
        }
    }

    private async Task Login()
    {
        if (!_memoryCache.TryGetValue(CacheKeys.ExanteUserManagementAuthToken, out string? token))
        {
            var response = await _exanteUserManagementApi.ApiUserAuthPostAsync(new LoginRequest(_username, _password));
            token = response.Token;
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(55));
            _memoryCache.Set(CacheKeys.ExanteTradeAuthToken, token, cacheEntryOptions);
        }

        if (string.IsNullOrEmpty(token))
        {
            throw new ArgumentNullException(token);
        }

        _exanteUserManagementApi.Configuration.DefaultHeaders["Authorization"] = $"token {token}";
    }

    private void InvalidateAuthHeaderCache()
    {
        _memoryCache.Remove(CacheKeys.ExanteUserManagementAuthToken);
    }
}