using Toolsfactory.Common.Mediator.Abstractions.Messages;

namespace Toolsfactory.Common.Mediator.Abstractions
{
    /// <summary>
    /// Helper class as a substitute for void in async tasks
    /// </summary>
    public struct Unit
    {
        public static readonly Unit Value = new();
    }
}
