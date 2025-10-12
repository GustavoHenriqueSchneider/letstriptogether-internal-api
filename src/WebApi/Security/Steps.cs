namespace WebApi.Security;

public class Steps
{
    public const string ValidateEmail = "validate-email";
    public const string SetPassword = "set-password";

    private string _step {  get; set; }

    private static readonly HashSet<string> ValidSteps =
    [
        ValidateEmail,
        SetPassword
    ];

    public Steps(string step)
    {
        if (!ValidSteps.Contains(step))
        {
            throw new InvalidOperationException($"Invalid step: {step}");
        }

        _step = step;
    }

    public override string ToString() => _step;

    public static implicit operator string(Steps s) => s._step;
}
