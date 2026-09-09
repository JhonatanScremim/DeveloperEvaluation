namespace Ambev.DeveloperEvaluation.Domain.Services
{
    public class SalesDiscountCalculator
    {
        public const int MinQuantityForDiscount = 4;
        public const int QuantityForTwentyPercent = 10;
        public const int MaxQuantityPerProduct = 20;

        public decimal GetDiscountPercent(int quantity)
        {
            if (quantity < 0)
                throw new DomainException("Quantity must be greater than zero");

            if (quantity > MaxQuantityPerProduct)
                throw new DomainException("Cannot sell more than 20 identical items");

            if (quantity < MinQuantityForDiscount)
                return 0m;

            if (quantity >= QuantityForTwentyPercent)
                return 0.20m;

            return 0.10m;
        }

        public decimal CalculateDiscountAmount(int quantity, decimal unitPrice)
        {
            if (unitPrice <= 0)
                throw new DomainException("Unit price must be greater than zero");
            var percent = GetDiscountPercent(quantity);
            return Math.Round(quantity * unitPrice * percent, 2, MidpointRounding.AwayFromZero);
        }

        public decimal CalculateTotal(int quantity, decimal unitPrice)
        {
            if (unitPrice <= 0)
                throw new DomainException("Unit price must be greater than zero");
            var percent = GetDiscountPercent(quantity);
            return Math.Round(quantity * unitPrice * (1 - percent), 2, MidpointRounding.AwayFromZero);
        }
    }
}
