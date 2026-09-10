using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Handler for processing CancelSaleItemCommand requests
/// </summary>
public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, SaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    /// <summary>
    /// Initializes a new instance of CancelSaleItemHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="eventPublisher">The domain event publisher</param>
    public CancelSaleItemHandler(ISaleRepository saleRepository, IMapper mapper, IEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    /// <summary>
    /// Handles the CancelSaleItemCommand request
    /// </summary>
    /// <param name="request">The CancelSaleItem command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale details after cancelling the item</returns>
    public async Task<SaleResult> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleItemValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        sale.CancelItem(request.ItemId);

        await _saleRepository.UpdateAsync(sale, cancellationToken);
        await _eventPublisher.PublishAsync(new ItemCancelledEvent(sale, request.ItemId), cancellationToken);

        return _mapper.Map<SaleResult>(sale);
    }
}
