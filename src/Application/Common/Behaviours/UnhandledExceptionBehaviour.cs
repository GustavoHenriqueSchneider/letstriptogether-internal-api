using LetsTripTogether.InternalApi.Application.Common.Exceptions;
using LetsTripTogether.InternalApi.Domain.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LetsTripTogether.InternalApi.Application.Common.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> _logger;

    public UnhandledExceptionBehaviour(ILogger<UnhandledExceptionBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            // Re-throw custom exceptions to be handled by ErrorController
            if (ex is BaseException or DomainBusinessRuleException)
            {
                _logger.LogWarning(
                    "Custom exception in handler: {ExceptionType} - {Message}",
                    ex.GetType().Name,
                    ex.Message);
                throw;
            }

            // Log unhandled exceptions
            _logger.LogError(
                ex,
                "Unhandled exception in handler: {RequestType} - {Message}",
                typeof(TRequest).Name,
                ex.Message);

            throw;
        }
    }
}
