using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pi.ActivityService.IntegrationEvents;
using Pi.BackofficeService.API.Factories;
using Pi.BackofficeService.API.Models;
using Pi.BackofficeService.API.Startup;
using Pi.BackofficeService.Application.Commands.Ticket;
using Pi.BackofficeService.Application.Commands.User;
using Pi.BackofficeService.Application.Factories;
using Pi.BackofficeService.Application.Queries;
using Pi.BackofficeService.Application.Queries.Filters;
using Pi.BackofficeService.Domain.AggregateModels.TicketAggregate;
using Pi.BackofficeService.Domain.Events.Ticket;
using Pi.Common.Http;

namespace Pi.BackofficeService.API.Controllers;

[Authorize]
[ApiController]
[Route("tickets")]
public class TicketController : ControllerBase
{
    private readonly IBus _bus;
    private readonly ITicketQueries _ticketQueries;
    private readonly ILogger<TicketController> _logger;

    public TicketController(IBus bus, ITicketQueries ticketQueries, ILogger<TicketController> logger)
    {
        _bus = bus;
        _ticketQueries = ticketQueries;
        _logger = logger;
    }

    [Authorize(Policy = "TicketIndexRead")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiPaginateResponse<List<TicketDetailResponse>>))]
    public async Task<IActionResult> Get([FromQuery] TicketPaginateQuery paginate)
    {
        var filters = new TicketFilters(paginate.ResponseCodeId, paginate.CustomerCode, paginate.Status);
        var records = await _ticketQueries.GetTickets(
            paginate.Page,
            paginate.PageSize,
            paginate.OrderBy,
            paginate.OrderDir?.ToLower() ?? "desc",
            filters
        );
        var total = await _ticketQueries.CountTickets(filters);

        return Ok(new ApiPaginateResponse<List<TicketDetailResponse>>(
            records.Select(DtoFactory.NewTicketResponse).ToList(),
            paginate.Page,
            paginate.PageSize,
            total,
            paginate.OrderBy,
            paginate.OrderDir
        ));
    }

    [Authorize(Policy = "TicketRead")]
    [HttpGet("transactions/{transactionNo}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<TicketDetailResponse>>))]
    public async Task<IActionResult> GetByTransactionNo(string transactionNo)
    {
        var records = await _ticketQueries.GetTicketsByTransactionNo(transactionNo);

        return Ok(new ApiResponse<List<TicketDetailResponse>>(records.Select(DtoFactory.NewTicketResponse).ToList()));
    }

    [Authorize(Policy = "TicketWrite")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketState>))]
    public async Task<IActionResult> Create(TicketRequest request)
    {
        var correlationId = Guid.NewGuid();
        var userId = await GetUserId();

        try
        {
            Response response = await _bus.CreateRequestClient<TicketCreateRequest>()
                .GetResponse<TicketNoGeneratedResponse, CannotCreateTicketResponse>(
                    new TicketCreateRequest(
                        correlationId,
                        request.TransactionNo,
                        request.TransactionType,
                        request.Payload)
                );

            switch (response.Message)
            {
                case TicketNoGeneratedResponse todoTicket:
                    var pendingTicket = await _bus.CreateRequestClient<MakerRequestActionRequest>()
                        .GetResponse<TicketState>(new MakerRequestActionRequest(todoTicket.CorrelationId, userId,
                            request.Method, request.Remark));
                    await _bus.Publish(EntityFactory
                        .NewActivityEvent(userId,
                            "Created Ticket",
                            CommonActivityType.Created.ToString(),
                            todoTicket.CorrelationId));
                    return Ok(new ApiResponse<TicketResponse>(DtoFactory.NewTicketResponse(pendingTicket.Message)));
                case CannotCreateTicketResponse cannotCreateTicketResponse:
                    return Problem(statusCode: StatusCodes.Status409Conflict,
                        detail: cannotCreateTicketResponse.ErrorMessage);
                default:
                    return Problem(statusCode: StatusCodes.Status500InternalServerError,
                        detail: "something went wrong");
            }
        }
        catch (RequestFaultException e)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, detail: e.Message);
        }
    }

    [Authorize(Policy = "TicketEdit")]
    [HttpPost("{ticketNo}/check")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<TicketState>))]
    public async Task<IActionResult> CheckTicket([Required] string ticketNo, [FromBody] CheckTicketRequest request)
    {
        var userId = await GetUserId();
        string? errorMsg = null;

        try
        {
            var timeout = TimeSpan.FromSeconds(60);
            using var source = new CancellationTokenSource(timeout);
            Response response = await _bus.CreateRequestClient<CheckerSelectActionRequest>()
                .GetResponse<TicketState, ExecuteActionFailed>(new CheckerSelectActionRequest(ticketNo,
                    userId, request.Method, request.Remark), source.Token, timeout);

            switch (response.Message)
            {
                case TicketState:
                    var ticket = (TicketState)response.Message;
                    await _bus.Publish(EntityFactory
                        .NewActivityEvent(userId,
                            "Verified Ticket",
                            CommonActivityType.Updated.ToString(),
                            ticket.CorrelationId));
                    break;
                case ExecuteActionFailed responseMessage:
                    errorMsg = responseMessage.FailedResponse;
                    break;
                default:
                    errorMsg = "something went wrong";
                    break;
            }
        }
        catch (Exception e)
        {
            errorMsg = e.Message;
            if (e is RequestFaultException ex)
            {
                errorMsg = ex.Fault != null
                    ? string.Join(", ", ex.Fault.Exceptions.Select(ei => ei.Message))
                    : ex.Message;
            }

            _logger.LogError(e, "Check endpoint got error: {ErrorMsg}", errorMsg);
        }

        if (errorMsg != null)
        {
            _logger.LogError("Check endpoint got error: {ErrorMsg}", errorMsg);
        }

        return Ok(new CheckTicketResponse
        {
            IsSuccess = errorMsg == null,
            ErrorMsg = errorMsg,
        });
    }

    [Authorize(Policy = "TicketIndexRead")]
    [HttpGet("{ticketNo}/payload")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<PayloadResponse?>))]
    public async Task<IActionResult> GetPayload([Required] string ticketNo)
    {
        try
        {
            var ticket = await _ticketQueries.GetTicketByTicketNo(ticketNo);

            return Ok(new ApiResponse<PayloadResponse?>(new PayloadResponse
            {
                Action = ticket.RequestAction.ToString(),
                Payload = ticket.Payload
            }));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<Guid> GetUserId()
    {
        var response = await _bus.CreateRequestClient<UserUpdateOrCreateRequest>()
            .GetResponse<UserIdResponse>(new UserUpdateOrCreateRequest(
                (Guid)User.GetIamId()!,
                User.FindFirstValue("first_name")!,
                User.FindFirstValue("last_name")!,
                User.FindFirstValue(ClaimTypes.Email)!
            ));

        return response.Message.Id;
    }
}