using Toolsfactory.Common.Mediator.Abstractions.Messages;

namespace Toolsfactory.Common.Mediator.Abstractions.Handlers
{
    /// <summary>
    /// Handler interface for a command without a return value
    /// </summary>
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task<Unit> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}
