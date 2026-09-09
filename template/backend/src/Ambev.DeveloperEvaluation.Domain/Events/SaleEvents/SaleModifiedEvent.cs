using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events.SaleEvents
{
    public class SaleModifiedEvent
    {
        public Sale Sale { get; }

        public SaleModifiedEvent(Sale sale)
        {
            Sale = sale;
        }
    }
}
