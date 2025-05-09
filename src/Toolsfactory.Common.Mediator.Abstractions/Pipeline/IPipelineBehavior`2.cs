using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolsfactory.Common.Mediator.Abstractions.Pipeline
{
    /// <summary>
    /// Defines a pipeline behavior to be executed before and after a request
    /// </summary>
    public interface IPipelineBehavior<in TRequest, TResponse>
    {
        Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken);
    }

    /// <summary>
    /// Delegate for the next request handler in the pipeline
    /// </summary>
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
}
