using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace LetsTripTogether.InternalApi.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController : ControllerBase
{
    [HttpGet]
    [HttpPost]
    [HttpPut]
    [HttpDelete]
    [HttpPatch]
    public IActionResult Error()
    {
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        var problemDetails = new ProblemDetails
        {
            Instance = exceptionHandlerPathFeature?.Path ?? string.Empty
        };

        switch (exception)
        {
            case BaseException baseException:
                problemDetails.Status = baseException.StatusCode;
                problemDetails.Title = baseException.Title ?? "An error occurred";
                problemDetails.Detail = baseException.Message;

                if (exception is ValidationException validationException && validationException.Errors.Any())
                {
                    problemDetails.Extensions["errors"] = validationException.Errors;
                }
                break;

            case DomainBusinessRuleException domainException:
                problemDetails.Status = domainException.StatusCode;
                problemDetails.Title = domainException.Title ?? "Business Rule Violation";
                problemDetails.Detail = domainException.Message;
                break;

            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "An error occurred while processing your request";
                problemDetails.Detail = exception?.Message;
                break;
        }

        return StatusCode(problemDetails.Status ?? StatusCodes.Status500InternalServerError, problemDetails);
    }
}
