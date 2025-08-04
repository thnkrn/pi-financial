using System.Diagnostics.CodeAnalysis;
using MassTransit;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Pi.SetMarketData.API.ApiConstants;
using Pi.SetMarketData.API.Infrastructure.Exceptions;
using Pi.SetMarketData.API.Infrastructure.Helpers;
using Pi.SetMarketData.Application.Abstractions;

namespace Pi.SetMarketData.API.Infrastructure.Services;

public class ControllerRequestHelper : IControllerRequestHelper
{
    private const string ErrorTemplate = "An unexpected error occurred: {Message}";
    private const string UnauthorizedTemplate = "Unauthorized access attempt during processing: {Message}";

    private const string InvalidOperationOrNotFoundExceptionTemplate =
        "Invalid operation or not found exception during processing: {Message}";

    private readonly ILogger<ControllerRequestHelper> _logger;
    private readonly IRequestBus _requestBus;

    /// <summary>
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="requestBus"></param>
    public ControllerRequestHelper(ILogger<ControllerRequestHelper> logger, IRequestBus requestBus)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _requestBus = requestBus ?? throw new ArgumentNullException(nameof(requestBus));
    }

    [SuppressMessage(
        "Major Code Smell",
        "S2436",
        Justification =
            "This method requires four generic parameters to handle complex market data requests with custom fallback behavior. The additional generic parameter provides type safety for fallback creation."
    )]
    public async Task<object> ExecuteMarketDataRequest<TRequest, TResponse, TBusRequest, TBusResponse>(
        TRequest request,
        Func<TRequest, TBusRequest> busRequestFactory,
        Func<ModelStateDictionary, bool> validateModel,
        ModelStateDictionary modelState)
        where TRequest : class
        where TResponse : class
        where TBusRequest : class
        where TBusResponse : class
    {
        try
        {
            if (!validateModel(modelState))
            {
                _logger.LogWarning("Invalid request received for type {RequestType}: {RequestPayload}",
                    typeof(TRequest).Name, request);
                throw ExceptionHelper.BadRequestException(ApiConstantValues.InvalidRequestError);
            }

            var busRequest = busRequestFactory(request);
            var busResponse = await _requestBus.GetResponse<TBusRequest, TBusResponse>(busRequest);

            if (!busResponse.IsValid || busResponse.Result == null)
            {
                _logger.LogWarning("No result found for request type {RequestType}: {RequestPayload}",
                    typeof(TRequest).Name, request);

                // Create and log fallback response
                var fallbackResult = ObjectInitializer.InitializeObject<TResponse>();
                return fallbackResult;
            }

            return busResponse.Result;
        }
        catch (RequestException ex)
        {
            if (ex.InnerException is InvalidOperationException or NotFoundException)
            {
                _logger.LogError(ex, InvalidOperationOrNotFoundExceptionTemplate, ex.Message);

                // Create and log fallback response
                var fallbackResult = ObjectInitializer.InitializeObject<TResponse>();
                return fallbackResult;
            }

            throw;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, UnauthorizedTemplate, ex.Message);
            throw ExceptionHelper.UnauthorizedErrorException(ApiConstantValues.UnauthorizedAccessError, ex);
        }
        catch (Exception ex) when (ex is not BadRequestException
                                   && ex is not InvalidOperationException
                                   && ex is not NotFoundException
                                   && ex is not UnauthorizedErrorException)
        {
            _logger.LogError(ex, ErrorTemplate, ex.Message);
            throw ExceptionHelper.InternalServerErrorException(ApiConstantValues.InternalServerError, ex);
        }
    }

    public async Task<object> ExecuteMarketDataManagementRequest<TRequest, TResponse>(
        TRequest request,
        Func<TRequest, bool> validateRequest)
        where TRequest : class
        where TResponse : class
    {
        try
        {
            if (!validateRequest(request))
            {
                _logger.LogWarning("Invalid request received for type {RequestType}: {RequestPayload}",
                    typeof(TRequest).Name, request);
                throw ExceptionHelper.BadRequestException(ApiConstantValues.InvalidRequestError);
            }

            var busResponse = await _requestBus.GetResponse<TRequest, TResponse>(request);

            if (!busResponse.IsValid || busResponse.Result == null)
            {
                _logger.LogWarning("No result found for request type {RequestType}: {RequestPayload}",
                    typeof(TRequest).Name, request);

                throw ExceptionHelper.NotFoundException(ApiConstantValues.NoResultError);
            }

            return busResponse.Result;
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogError(ex, UnauthorizedTemplate, ex.Message);
            throw ExceptionHelper.UnauthorizedErrorException(ApiConstantValues.UnauthorizedAccessError, ex);
        }
        catch (Exception ex) when (ex is not BadRequestException
                                   && ex is not NotFoundException
                                   && ex is not UnauthorizedErrorException)
        {
            _logger.LogError(ex, ErrorTemplate, ex.Message);
            throw ExceptionHelper.InternalServerErrorException(ApiConstantValues.InternalServerError, ex);
        }
    }
}