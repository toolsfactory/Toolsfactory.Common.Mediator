using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Toolsfactory.Common.Mediator.Abstractions;
using Toolsfactory.Common.Mediator.Abstractions.Handlers;

namespace Toolsfactory.Common.Mediator
{
    public static class MediatorExtensions
    {
        /// <summary>
        /// Registers the Mediator and all handlers
        /// </summary>
        public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
        {
            // Register the mediator itself
            services.AddScoped<IMediator, Mediator>();

            // If no assemblies are specified, take the current assembly
            if (assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetCallingAssembly() };
            }

            // Register all command handlers
            RegisterCommandHandlers(services, assemblies);

            // Register all query handlers
            RegisterQueryHandlers(services, assemblies);

            // Register all notification handlers
            RegisterNotificationHandlers(services, assemblies);

            return services;
        }

        private static void RegisterCommandHandlers(IServiceCollection services, Assembly[] assemblies)
        {
            // Command handlers without return value (ICommand)
            var commandHandlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<>)))
                .ToList();

            foreach (var handlerType in commandHandlerTypes)
            {
                var commandHandlerInterface = handlerType.GetInterfaces().First(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));

                services.AddTransient(commandHandlerInterface, handlerType);
            }

            // Command handlers with return value (ICommand<TResult>)
            var commandResultHandlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
                .ToList();

            foreach (var handlerType in commandResultHandlerTypes)
            {
                var commandHandlerInterface = handlerType.GetInterfaces().First(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));

                services.AddTransient(commandHandlerInterface, handlerType);
            }
        }

        private static void RegisterQueryHandlers(IServiceCollection services, Assembly[] assemblies)
        {
            var queryHandlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                .ToList();

            foreach (var handlerType in queryHandlerTypes)
            {
                var queryHandlerInterface = handlerType.GetInterfaces().First(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

                services.AddTransient(queryHandlerInterface, handlerType);
            }
        }

        private static void RegisterNotificationHandlers(IServiceCollection services, Assembly[] assemblies)
        {
            var notificationHandlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(INotificationHandler<>)))
                .ToList();

            foreach (var handlerType in notificationHandlerTypes)
            {
                // Find all implemented NotificationHandler interfaces
                var notificationHandlerInterfaces = handlerType.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(INotificationHandler<>));

                // Register each handler for all its implemented notification types
                foreach (var handlerInterface in notificationHandlerInterfaces)
                {
                    // Important: Register as transient so each handler gets its own instance
                    services.AddTransient(handlerInterface, handlerType);
                }
            }
        }
    }
}
