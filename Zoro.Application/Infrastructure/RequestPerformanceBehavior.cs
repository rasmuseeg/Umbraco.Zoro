using MediatR;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Umbraco.Core.Logging;

namespace Zoro.Application.Infrastructure
{
    public class RequestPerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly Stopwatch _timer;
        private readonly ILogger _logger;

        public RequestPerformanceBehaviour(ILogger logger)
        {
            _timer = new Stopwatch();

            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();

            var response = await next();

            _timer.Stop();

            if (_timer.ElapsedMilliseconds > 500)
            {
                var name = typeof(TRequest).Name;

                _logger.Warn<TRequest>("Long Running Request: {0} ({1} milliseconds) {2}", () => name, () => _timer.ElapsedMilliseconds, () => request);
            }

            return response;
        }
    }
}
