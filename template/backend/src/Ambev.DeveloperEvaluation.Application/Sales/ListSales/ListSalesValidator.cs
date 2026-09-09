using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Validator for ListSalesCommand
/// </summary>
public class ListSalesValidator : AbstractValidator<ListSalesCommand>
{
    /// <summary>
    /// Initializes validation rules for ListSalesCommand
    /// </summary>
    public ListSalesValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.Size).InclusiveBetween(1, 100);
    }
}
