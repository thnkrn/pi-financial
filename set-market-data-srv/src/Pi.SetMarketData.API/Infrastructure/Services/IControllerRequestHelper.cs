using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Pi.SetMarketData.API.Infrastructure.Services;

public interface IControllerRequestHelper
{
    [SuppressMessage(
        "Major Code Smell",
        "S2436",
        Justification =
            "This method requires four generic parameters to handle complex market data requests with custom fallback behavior. The additional generic parameter provides type safety for fallback creation."
    )]
    Task<object> ExecuteMarketDataRequest<TRequest, TResponse, TBusRequest, TBusResponse>(
        TRequest request,
        Func<TRequest, TBusRequest> busRequestFactory,
        Func<ModelStateDictionary, bool> validateModel,
        ModelStateDictionary modelState)
        where TRequest : class
        where TResponse : class
        where TBusRequest : class
        where TBusResponse : class;

    Task<object> ExecuteMarketDataManagementRequest<TRequest, TResponse>(
        TRequest request,
        Func<TRequest, bool> validateRequest)
        where TRequest : class
        where TResponse : class;
}