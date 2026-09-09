using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Profile for mapping between ListSalesCommand and SaleListCriteria
/// </summary>
public class ListSalesProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for ListSales operation
    /// </summary>
    public ListSalesProfile()
    {
        CreateMap<ListSalesCommand, SaleListCriteria>();
    }
}
