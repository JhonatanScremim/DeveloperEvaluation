using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Events.SaleEvents
{
    public class SaleCancelledEvent
    {
        public Sale Sale { get; }

        public SaleCancelledEvent(Sale sale)
        {
            Sale = sale;
        }
    }
}
