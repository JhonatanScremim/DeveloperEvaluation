using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.ListSales;

/// <summary>
/// Validator for ListSalesRequest
/// </summary>
public class ListSalesRequestValidator : AbstractValidator<ListSalesRequest>
{
    /// <summary>
    /// Initializes validation rules for ListSalesRequest
    /// </summary>
    public ListSalesRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.Size).InclusiveBetween(1, 100);
    }
}
