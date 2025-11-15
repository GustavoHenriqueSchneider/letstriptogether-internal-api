using MediatR;

namespace Application.UseCases.Error.Query.GetError;

public class GetErrorQuery : IRequest<GetErrorResponse>
{
    public Exception? Exception { get; init; }
    public string? Path { get; init; }
}
