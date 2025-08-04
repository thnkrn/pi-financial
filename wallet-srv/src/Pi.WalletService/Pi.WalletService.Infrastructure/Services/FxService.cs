using Microsoft.Extensions.Logging;
using Pi.WalletService.Application.Services.FxService;
using Pi.Client.FxService.Api;
using Pi.Client.FxService.Model;
using Pi.WalletService.Domain.AggregatesModel.GlobalWalletAggregate;
using ConfirmFxQuotationCommand = Pi.Client.FxService.Model.ConfirmFxQuotationCommand;
using QuoteType = Pi.Client.FxService.Model.QuoteType;

namespace Pi.WalletService.Infrastructure.Services;

public class FxService : IFxService
{
    private readonly IExchangeApi _fxApi;
    private readonly ILogger<FxService> _logger;

    public FxService(IExchangeApi fxApi, ILogger<FxService> logger)
    {
        _fxApi = fxApi;
        _logger = logger;
    }

    public async Task<InitiateResponse> Initiate(InitiateRequest request)
    {
        try
        {
            var initiateQuoteType = Enum.Parse<QuoteType>(((int)request.FxQuoteType).ToString());
            var initiateFxQuotationCommand = new InitiateFxQuotationCommand(
                initiateQuoteType,
                request.ContractCurrency,
                request.ContractAmount,
                request.ContractAccountId,
                request.CounterCurrency,
                request.CounterAccountId,
                request.CounterAmount,
                request.Ref1,
                request.RequestedBy,
                request.Ref2!
            );
            var response = await _fxApi.InitiateAsync(initiateFxQuotationCommand);

            if (!Enum.TryParse(response.Data.CounterCurrency, out Currency counterCur) || !Enum.TryParse(response.Data.ContractCurrency, out Currency contractCur))
            {
                throw new InvalidDataException($"Unexpect CounterCurrency: {response.Data.CounterCurrency}, ContractCurrency: {response.Data.ContractCurrency}");
            }

            return new InitiateResponse(
                response.Data.TransactionId,
                response.Data.ContractAmount,
                response.Data.CounterAmount,
                contractCur,
                counterCur,
                response.Data.ExchangeRate,
                response.Data.ExpiredAt
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e, "FxService:Initiate: Unable to initiate FX quotation. UserId {ID}", request.RequestedBy);
            throw new Exception("Unable to Initiate FX Quotation");
        }
    }

    public async Task Confirm(ConfirmRequest request)
    {
        try
        {
            var confirmFxQuotationCommand = new ConfirmFxQuotationCommand(request.TransactionId);
            await _fxApi.ConfirmAsync(confirmFxQuotationCommand);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "FxService:Confirm: Unable to confirm FX quotation. TransactionId {ID}", request.TransactionId);
            throw;
        }
    }

    public async Task<GetTransactionResponse> GetTransaction(string transactionId)
    {
        try
        {
            var res = await _fxApi.GetTransactionAsync(transactionId);
            return new GetTransactionResponse(
                res.Data.Id,
                res.Data.QuoteType switch
                {
                    QuoteType.NUMBER_66 => FxQuoteType.Buy,
                    QuoteType.NUMBER_83 => FxQuoteType.Sell,
                    null or _ => throw new IndexOutOfRangeException()
                },
                res.Data.ValueDate,
                res.Data.ContractAmount,
                res.Data.ContractCurrency,
                res.Data.CounterAmount,
                res.Data.CounterCurrency,
                res.Data.ExchangeRate,
                res.Data.TransactionDateTime
            );
        }
        catch (Exception e)
        {
            _logger.LogError(e, "FxService:Confirm: Unable to get FX quotation. TransactionId {ID}", transactionId);
            throw;
        }
    }
}