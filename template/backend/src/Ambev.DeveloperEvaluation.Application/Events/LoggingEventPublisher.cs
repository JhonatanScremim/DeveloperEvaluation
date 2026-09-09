using Ambev.DeveloperEvaluation.Domain.Events;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Events
{
    /// <summary>
    /// Publishes domain events by writing them to the application log.
    /// A message broker can replace this later without changing the handlers.
    /// </summary>
    public class LoggingEventPublisher : IEventPublisher
    {
        private readonly ILogger<LoggingEventPublisher> _logger;

        public LoggingEventPublisher(ILogger<LoggingEventPublisher> logger)
        {
            _logger = logger;
        }

        public Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : class
        {
            _logger.LogInformation("Event published: {EventName} {@Event}", typeof(TEvent).Name, domainEvent);
            return Task.CompletedTask;
        }
    }
}
