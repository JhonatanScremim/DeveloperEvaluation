using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        public Guid SaleId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Cancelled { get; set; }

        public static SaleItem Create(Guid productId, string productName, int quantity, decimal unitPrice)
        {
            if (productId == Guid.Empty)
                throw new DomainException("Product is required");

            if (string.IsNullOrWhiteSpace(productName))
                throw new DomainException("Product name is required");

            var item = new SaleItem
            {
                ProductId = productId,
                ProductName = productName.Trim()
            };

            item.SetQuantityAndPrice(quantity, unitPrice);
            return item;
        }

        public void SetQuantityAndPrice(int quantity, decimal unitPrice)
        {
            if (Cancelled)
                throw new DomainException("Cannot modify a cancelled item");

            var calculator = new SalesDiscountCalculator();
            Quantity = quantity;
            UnitPrice = unitPrice;
            Discount = calculator.CalculateDiscountAmount(quantity, unitPrice);
            TotalAmount = calculator.CalculateTotal(quantity, unitPrice);
        }
    }
}
