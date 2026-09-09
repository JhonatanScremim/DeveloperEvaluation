using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

/// <summary>
/// Handler for processing ListSalesCommand requests
/// </summary>
public class ListSalesHandler : IRequestHandler<ListSalesCommand, ListSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of ListSalesHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the ListSalesCommand request
    /// </summary>
    /// <param name="request">The ListSales command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The paginated list of sales</returns>
    public async Task<ListSalesResult> Handle(ListSalesCommand request, CancellationToken cancellationToken)
    {
        var validator = new ListSalesValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var criteria = _mapper.Map<SaleListCriteria>(request);

        var (sales, totalCount) = await _saleRepository.ListAsync(criteria, cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.Size);

        return new ListSalesResult
        {
            Data = _mapper.Map<List<SaleResult>>(sales),
            CurrentPage = request.Page,
            TotalPages = totalPages,
            TotalCount = totalCount
        };
    }
}
