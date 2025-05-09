using Microsoft.Extensions.Logging;

namespace Toolsfactory.Common.Mediator
{
    internal static partial class Log
    {
        [LoggerMessage(1, LogLevel.Information, "Sending command of type {CommandType}")]
        public static partial void SendingCommand(ILogger logger, string commandType);

        [LoggerMessage(2, LogLevel.Information, "Sending command of type {CommandType} with result {ResultType}")]
        public static partial void SendingCommandWithResult(ILogger logger, string commandType, string resultType);

        [LoggerMessage(3, LogLevel.Information, "Sending query of type {QueryType} with result {ResultType}")]
        public static partial void SendingQuery(ILogger logger, string queryType, string resultType);

        [LoggerMessage(4, LogLevel.Information, "Publishing notification of type {NotificationType}")]
        public static partial void PublishingNotification(ILogger logger, string notificationType);

        [LoggerMessage(5, LogLevel.Warning, "No handlers registered for notification of type {NotificationType}")]
        public static partial void NoHandlersForNotification(ILogger logger, string notificationType);

        [LoggerMessage(6, LogLevel.Debug, "Resolved handler of type {HandlerType} for {RequestType}")]
        public static partial void ResolvedHandler(ILogger logger, string handlerType, string requestType);

        [LoggerMessage(7, LogLevel.Debug, "Invoking handler of type {HandlerType} for notification {NotificationType}")]
        public static partial void InvokingHandler(ILogger logger, string handlerType, string notificationType);

        [LoggerMessage(8, LogLevel.Error, "Handler of type {HandlerType} failed for notification {NotificationType}: {Exception}")]
        public static partial void HandlerFailed(ILogger logger, string handlerType, string notificationType, Exception exception);
    }
}
