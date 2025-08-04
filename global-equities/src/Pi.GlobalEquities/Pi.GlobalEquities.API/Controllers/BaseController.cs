using Microsoft.AspNetCore.Mvc;
using Pi.GlobalEquities.Errors;

namespace Pi.GlobalEquities.API.Controllers;

public class BaseController : ControllerBase
{
    private readonly ILogger<CustomProblemDetailsFactory> _logger;
    public BaseController(ILoggerFactory loggerFact)
    {
        _logger = loggerFact.CreateLogger<CustomProblemDetailsFactory>();
    }

    protected IActionResult BadRequest(Error error)
    {
        return ValidationProblem(CustomProblemDetailsFactory.CreateValidationProblemDetails(_logger, error));
    }

    protected IActionResult InternalServerError(Error error)
    {
        return CustomProblemDetailsFactory.CreateProblemDetails(error, StatusCodes.Status500InternalServerError)
            .ToObjectResult();
    }

    protected IActionResult Forbid(Error error)
    {
        return CustomProblemDetailsFactory.CreateValidationResult(_logger,
                error, StatusCodes.Status403Forbidden)
            .ToObjectResult();
    }

    protected IActionResult PaymentRequired()
    {
        return CustomProblemDetailsFactory.CreateValidationResult(_logger,
                AccountErrors.InsufficientBalance, StatusCodes.Status402PaymentRequired)
            .ToObjectResult();
    }
}
