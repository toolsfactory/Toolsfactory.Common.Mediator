using Toolsfactory.Common.Mediator.Abstractions.Messages;

namespace Toolsfactory.Common.Mediator.Abstractions
{
    public interface ISender
    {
        /// <summary>
        /// Sendet ein Command zur Ausführung (keine Rückgabe außer Exceptions)
        /// </summary>
        Task<Unit> SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand;

        /// <summary>
        /// Sendet ein Command zur Ausführung mit Rückgabewert
        /// </summary>
        Task<TResult> SendCommandAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResult>;

        /// <summary>
        /// Sendet eine Query zur Ausführung
        /// </summary>
        Task<TResult> SendQueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>;
    }
}
