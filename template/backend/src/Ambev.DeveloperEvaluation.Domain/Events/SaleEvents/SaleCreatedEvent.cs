using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events.SaleEvents
{
    public class SaleCreatedEvent
    {
        public Sale Sale { get; }

        public SaleCreatedEvent(Sale sale)
        {
            Sale = sale;
        }
    }
}
