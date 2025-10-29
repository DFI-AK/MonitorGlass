namespace MonitorGlass.Application.Windows.Commands.DeleteServer;

public class DeleteServerCommandValidator : AbstractValidator<DeleteServerCommand>
{
    public DeleteServerCommandValidator()
    {
        RuleFor(x => x.ServerId).NotEmpty().WithMessage("Server id should not be empty.");
    }
}
