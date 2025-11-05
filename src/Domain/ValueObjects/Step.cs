using LetsTripTogether.InternalApi.Domain.Common.Exceptions;

namespace LetsTripTogether.InternalApi.Domain.ValueObjects;

public class Step
{
    public const string ValidateEmail = "validate-email";
    public const string SetPassword = "set-password";

    private string _step {  get; set; }

    private static readonly HashSet<string> ValidSteps =
    [
        ValidateEmail,
        SetPassword
    ];

    public Step(string step)
    {
        if (!ValidSteps.Contains(step))
        {
            throw new DomainBusinessRuleException($"Invalid step: {step}");
        }

        _step = step;
    }

    public override string ToString() => _step;

    public static implicit operator string(Step s) => s._step;
}
