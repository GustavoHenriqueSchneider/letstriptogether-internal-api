using MediatR;

namespace Application.UseCases.v1.Auth.Command.SendRegisterConfirmationEmail;

public class SendRegisterConfirmationEmailCommand : IRequest<SendRegisterConfirmationEmailResponse>
{
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
}
