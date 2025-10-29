namespace MonitorGlass.Application.Windows.Commands.AddServer;

internal class AddServerCommandValidator : AbstractValidator<AddServerCommand>
{
    public AddServerCommandValidator()
    {
        RuleFor(v => v.ServerName)
            .NotEmpty().WithMessage("Server name is required.")
            .MaximumLength(100).WithMessage("Server name must not exceed 100 characters.");

        RuleFor(v => v.ServerName)
        .Matches(@"^[a-zA-Z0-9\-\.]+$")
        .WithMessage("Server name contains invalid characters.");
    }
}