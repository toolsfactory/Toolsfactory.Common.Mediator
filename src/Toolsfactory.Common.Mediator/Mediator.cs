using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;
using Toolsfactory.Common.Mediator.Abstractions;
using Toolsfactory.Common.Mediator.Abstractions.Handlers;
using Toolsfactory.Common.Mediator.Abstractions.Messages;
using Toolsfactory.Common.Mediator.Abstractions.Pipeline;

namespace Toolsfactory.Common.Mediator
{
    /// <summary>
    /// Implements the Mediator pattern to decouple the sender and receiver of requests.
    /// Provides methods to send commands, queries, and publish notifications.
    /// </summary>
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<Mediator> _logger;

        // Cache for handler types to improve performance by avoiding repeated type lookups
        private static readonly ConcurrentDictionary<Type, Type> _handlerTypes = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Mediator"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider used to resolve handlers and pipeline behaviors.</param>
        public Mediator(IServiceProvider serviceProvider, ILogger<Mediator> logger)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Sends a command without a return value to its corresponding handler.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command to send.</typeparam>
        /// <param name="command">The command instance to process.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A <see cref="Unit"/> value indicating the command was processed.</returns>
        public async Task<Unit> SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand
        {
            Log.SendingCommand(_logger, typeof(TCommand).Name);
            return await ExecutePipeline<TCommand, Unit>(command, async () =>
            {
                var handlerType = typeof(ICommandHandler<TCommand>);
                var handler = _serviceProvider.GetRequiredService(handlerType);

                Log.ResolvedHandler(_logger, handlerType.Name, typeof(TCommand).Name);

                var method = handlerType.GetMethod("Handle");
                var task = (Task<Unit>)method?.Invoke(handler, new object[] { command, cancellationToken })!;

                return await task.ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a command with a return value to its corresponding handler.
        /// </summary>
        /// <typeparam name="TCommand">The type of the command to send.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the handler.</typeparam>
        /// <param name="command">The command instance to process.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The result of processing the command.</returns>
        public async Task<TResult> SendCommandAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
            where TCommand : ICommand<TResult>
        {
            Log.SendingCommandWithResult(_logger, typeof(TCommand).Name, typeof(TResult).Name);
            return await ExecutePipeline<TCommand, TResult>(command, async () =>
            {
                var handlerType = typeof(ICommandHandler<TCommand, TResult>);
                var handler = _serviceProvider.GetRequiredService(handlerType);

                Log.ResolvedHandler(_logger, handlerType.Name, typeof(TCommand).Name);

                var method = handlerType.GetMethod("HandleAsync");
                var task = (Task<TResult>)method?.Invoke(handler, new object[] { command, cancellationToken })!;

                return await task.ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a query to its corresponding handler and retrieves the result.
        /// </summary>
        /// <typeparam name="TQuery">The type of the query to send.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the handler.</typeparam>
        /// <param name="query">The query instance to process.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The result of processing the query.</returns>
        public async Task<TResult> SendQueryAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default)
            where TQuery : IQuery<TResult>
        {
            Log.SendingQuery(_logger, typeof(TQuery).Name, typeof(TResult).Name);

            return await ExecutePipeline<TQuery, TResult>(query, async () =>
            {
                var handlerType = typeof(IQueryHandler<TQuery, TResult>);
                var handler = _serviceProvider.GetRequiredService(handlerType);

                Log.ResolvedHandler(_logger, handlerType.Name, typeof(TQuery).Name);
                
                var method = handlerType.GetMethod("HandleAsync");
                var task = (Task<TResult>)method?.Invoke(handler, new object[] { query, cancellationToken })!;

                return await task.ConfigureAwait(false);
            }, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Publishes a notification to all registered handlers.
        /// </summary>
        /// <typeparam name="TNotification">The type of the notification to publish.</typeparam>
        /// <param name="notification">The notification instance to publish.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            Log.PublishingNotification(_logger, typeof(TNotification).Name);

            var handlerType = typeof(INotificationHandler<TNotification>);
            var handlers = _serviceProvider.GetServices(handlerType);

            if (!handlers.Any())
            {
                // No handlers registered - this is a valid case
                return;
            }

            var method = handlerType.GetMethod("HandleAsync");
            var tasks = handlers.Select(async handler =>
            {
                try
                {
                    Log.InvokingHandler(_logger, handler!.GetType().Name, typeof(TNotification).Name);
                    await (Task)method?.Invoke(handler, new object[] { notification, cancellationToken })!;
                }
                catch (Exception ex)
                {
                    // Log the exception or handle it as needed
                    Log.HandlerFailed(_logger, handler!.GetType().Name, typeof(TNotification).Name);
                }
            });

            // Execute in parallel, but wait for all
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// Executes the pipeline of behaviors around the request handler.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request being processed.</typeparam>
        /// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
        /// <param name="request">The request instance to process.</param>
        /// <param name="handlerCallback">The callback to invoke the actual handler.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>The response from the handler or pipeline.</returns>
        private async Task<TResponse> ExecutePipeline<TRequest, TResponse>(
            TRequest request,
            RequestHandlerDelegate<TResponse> handlerCallback,
            CancellationToken cancellationToken)
        {
            // Get all pipeline behaviors for this request type
            var behaviors = _serviceProvider
                .GetServices<IPipelineBehavior<TRequest, TResponse>>()
                .ToList();

            if (behaviors.Count == 0)
            {
                // No pipeline behaviors, execute the handler directly
                return await handlerCallback().ConfigureAwait(false);
            }

            // Build the pipeline by nesting the behaviors
            RequestHandlerDelegate<TResponse> pipeline = handlerCallback;

            behaviors.Reverse();
            // Build the pipeline from the last behavior to the first
            foreach (var behavior in behaviors)
            {
                var currentPipeline = pipeline;
                pipeline = () => behavior.Handle(request, currentPipeline, cancellationToken);
            }

            // Execute the pipeline
            return await pipeline().ConfigureAwait(false);
        }
    }
}
