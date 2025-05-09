using Toolsfactory.Common.Mediator.Abstractions.Messages;

namespace Toolsfactory.Common.Mediator.Abstractions
{
    public interface IPublisher
    {
        /// <summary>
        /// Veröffentlicht eine Notification an alle registrierten Handler
        /// </summary>

        Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
                where TNotification : INotification;
    }
}
