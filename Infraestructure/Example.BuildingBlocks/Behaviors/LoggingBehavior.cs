using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PAN.Mediator.Abstractions;

namespace Example.BuildingBlocks.Behaviors
{
    public sealed class LoggingBehavior<TRequest, TResponse>
        : IPanPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<TResponse> Handle(
            TRequest request,
            PanRequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "[START] Handle request={Request} - Response={Response} - RequestData={RequestData}",
                typeof(TRequest).Name,
                typeof(TResponse).Name,
                request);

            var timer = Stopwatch.StartNew();

            var response = await next();

            timer.Stop();

            if (timer.Elapsed.TotalSeconds > 3)
            {
                _logger.LogWarning(
                    "[PERFORMANCE] The request {Request} took {ElapsedMilliseconds} ms.",
                    typeof(TRequest).Name,
                    timer.ElapsedMilliseconds);
            }

            _logger.LogInformation(
                "[END] Handled {Request} with {Response} in {ElapsedMilliseconds} ms",
                typeof(TRequest).Name,
                typeof(TResponse).Name,
                timer.ElapsedMilliseconds);

            return response;
        }
    }
}
