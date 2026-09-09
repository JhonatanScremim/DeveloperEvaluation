namespace Ambev.DeveloperEvaluation.Application.Common
{
    public class SaleItemResult
    {
        public Guid Id { get; set; }
        public ExternalIdentityResult Product { get; set; } = new();
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Cancelled { get; set; }
    }
}
