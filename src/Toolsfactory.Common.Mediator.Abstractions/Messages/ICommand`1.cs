namespace Toolsfactory.Common.Mediator.Abstractions.Messages
{
    /// <summary>
    /// Represents a command with a return value of type TResult
    /// </summary>
    public interface ICommand<out TResult> { }
}
