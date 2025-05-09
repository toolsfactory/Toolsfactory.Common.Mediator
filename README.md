# Toolsfactory.Common Mediator Implementation

The `Toolsfactory.Common.Mediator` library implements the Mediator pattern to decouple the sender and receiver of requests. It provides a centralized mechanism for handling commands, queries, and notifications in a clean and extensible way.

## Key Features
- **Command Handling**: Send commands with or without return values to their respective handlers.
- **Query Handling**: Execute queries and retrieve results from their handlers.
- **Notification Publishing**: Publish notifications to all registered handlers.
- **Pipeline Behaviors**: Add custom behaviors (e.g., logging, validation, or caching) around request processing.
- **Zero-Allocation Logging**: Efficient logging using source generators for better performance.

## Usage
1. **Register the Mediator**:
   Use the `AddMediator` extension method to register the Mediator and all handlers in your dependency injection container.


2. **Send Commands and Queries**:
   Use the `IMediator` interface to send commands or queries.


3. **Publish Notifications**:
   Publish notifications to all registered handlers.


## Benefits
- **Decoupled Architecture**: Simplifies communication between components by removing direct dependencies.
- **Extensibility**: Easily add custom behaviors using pipeline extensions.
- **Performance**: Optimized for high-performance scenarios with zero-allocation logging and caching.

This library is designed for modern .NET applications targeting `.NET 9` and leverages advanced features like dependency injection & logging.
         