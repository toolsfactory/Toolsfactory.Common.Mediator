using Toolsfactory.Common.Mediator.Abstractions.Messages;

namespace Toolsfactory.Common.Mediator.Abstractions.Handlers
{
    /// <summary>
    /// Handler interface for a query
    /// </summary>
    public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
