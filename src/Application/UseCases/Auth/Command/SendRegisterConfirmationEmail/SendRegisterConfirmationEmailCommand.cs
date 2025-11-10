using MediatR;

namespace Application.UseCases.Auth.Command.SendRegisterConfirmationEmail;

public class SendRegisterConfirmationEmailCommand : IRequest<SendRegisterConfirmationEmailResponse>
{
    public string Name { get; init; } = null!;
    public string Email { get; init; } = null!;
}
