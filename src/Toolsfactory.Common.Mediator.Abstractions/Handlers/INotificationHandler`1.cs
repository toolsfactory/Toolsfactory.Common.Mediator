using Toolsfactory.Common.Mediator.Abstractions.Messages;

namespace Toolsfactory.Common.Mediator.Abstractions.Handlers
{
    /// <summary>
    /// Handler Interface für eine Notification
    /// </summary>
    public interface INotificationHandler<in TNotification>
        where TNotification : INotification
    {
        Task HandleAsync(TNotification notification, CancellationToken cancellationToken);
    }
}
