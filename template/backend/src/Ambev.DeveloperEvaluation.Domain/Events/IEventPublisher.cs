namespace Ambev.DeveloperEvaluation.Domain.Events
{
    /// <summary>
    /// Publishes domain events. The default implementation writes to the application log.
    /// </summary>
    public interface IEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : class;
    }
}
