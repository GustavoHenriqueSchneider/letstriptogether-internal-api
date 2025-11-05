using FluentValidation;

namespace LetsTripTogether.InternalApi.Application.UseCases.Auth.Command.ValidateRegisterConfirmationCode;

public class ValidateRegisterConfirmationCodeValidator : AbstractValidator<ValidateRegisterConfirmationCodeCommand>
{
    public ValidateRegisterConfirmationCodeValidator()
    {
        // TODO : validar codigo entre 100000 e 999999
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required");
    }
}
