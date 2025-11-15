using Application.UseCases.Error.Query.GetError;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [HttpPost]
    [HttpPut]
    [HttpDelete]
    [HttpPatch]
    public async Task<IActionResult> Error()
    {
        var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        var query = new GetErrorQuery
        {
            Exception = exceptionHandlerPathFeature?.Error,
            Path = exceptionHandlerPathFeature?.Path ?? string.Empty
        };

        var response = await mediator.Send(query);
        return StatusCode(response.Status, response);
    }
}
