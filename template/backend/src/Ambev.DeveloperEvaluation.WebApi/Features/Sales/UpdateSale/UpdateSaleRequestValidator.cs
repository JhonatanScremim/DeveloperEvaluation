using Ambev.DeveloperEvaluation.Domain.Services;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Validator for UpdateSaleRequest that defines validation rules for sale update.
/// </summary>
public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    /// <summary>
    /// Initializes a new instance of the UpdateSaleRequestValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - SaleDate: Required
    /// - CustomerId: Required
    /// - CustomerName: Required, maximum 200 characters
    /// - BranchId: Required
    /// - BranchName: Required, maximum 200 characters
    /// - Items: At least one item is required
    /// - Item ProductId: Required
    /// - Item ProductName: Required, maximum 200 characters
    /// - Item Quantity: Must be greater than 0 and cannot exceed 20 identical items
    /// - Item UnitPrice: Must be greater than 0
    /// </remarks>
    public UpdateSaleRequestValidator()
    {
        RuleFor(sale => sale.SaleDate).NotEmpty();
        RuleFor(sale => sale.CustomerId).NotEmpty();
        RuleFor(sale => sale.CustomerName).NotEmpty().MaximumLength(200);
        RuleFor(sale => sale.BranchId).NotEmpty();
        RuleFor(sale => sale.BranchName).NotEmpty().MaximumLength(200);
        RuleFor(sale => sale.Items).NotEmpty().WithMessage("Sale must have at least one item");
        RuleForEach(sale => sale.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.ProductName).NotEmpty().MaximumLength(200);
            item.RuleFor(i => i.Quantity)
                .GreaterThan(0)
                .LessThanOrEqualTo(SalesDiscountCalculator.MaxQuantityPerProduct)
                .WithMessage("Cannot sell more than 20 identical items");
            item.RuleFor(i => i.UnitPrice).GreaterThan(0);
        });
    }
}
