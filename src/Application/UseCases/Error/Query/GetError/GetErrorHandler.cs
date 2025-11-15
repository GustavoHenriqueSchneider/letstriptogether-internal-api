using Application.Common.Exceptions;
using Domain.Common.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.UseCases.Error.Query.GetError;

public class GetErrorHandler : IRequestHandler<GetErrorQuery, GetErrorResponse>
{
    public Task<GetErrorResponse> Handle(GetErrorQuery request, CancellationToken cancellationToken)
    {
        var exception = request.Exception;

        int status;
        string title;
        string? detail;
        var instance = request.Path ?? string.Empty;
        var extensions = new Dictionary<string, object>();

        switch (exception)
        {
            case BaseException baseException:
                status = baseException.StatusCode;
                title = baseException.Title ?? "An error occurred";
                detail = baseException.Message;

                if (exception is ValidationException validationException 
                    && validationException.Errors.Count != 0)
                {
                    extensions["errors"] = validationException.Errors;
                }
                break;

            case DomainBusinessRuleException domainException:
                status = domainException.StatusCode;
                title = domainException.Title ?? "Business Rule Violation";
                detail = domainException.Message;
                break;

            default:
                status = StatusCodes.Status500InternalServerError;
                title = "An error occurred while processing your request";
                detail = exception?.Message;
                break;
        }

        var response = new GetErrorResponse
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = instance,
            Extensions = extensions.Count == 0 ? null :  extensions
        };

        return Task.FromResult(response);
    }
}
