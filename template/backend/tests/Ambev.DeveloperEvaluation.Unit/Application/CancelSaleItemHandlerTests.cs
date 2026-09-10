using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Events.SaleEvents;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="CancelSaleItemHandler"/> class.
/// </summary>
public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;
    private readonly CancelSaleItemHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelSaleItemHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _handler = new CancelSaleItemHandler(_saleRepository, _mapper, _eventPublisher);
    }

    /// <summary>
    /// Tests that a valid sale item cancellation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given existing sale item When cancelling item Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();
        sale.Id = Guid.NewGuid();
        sale.AddItem(Guid.NewGuid(), "Beer", 2, 10m);
        sale.Items[0].Id = Guid.NewGuid();

        var command = CancelSaleItemHandlerTestData.GenerateValidCommand(sale.Id, sale.Items[0].Id);
        var result = new SaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(result);

        // When
        var cancelItemResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        cancelItemResult.Should().NotBeNull();
        cancelItemResult.Id.Should().Be(sale.Id);
        sale.Items[0].Cancelled.Should().BeTrue();
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await _eventPublisher.Received(1).PublishAsync(Arg.Any<ItemCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid sale item cancellation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid ids When cancelling item Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CancelSaleItemCommand(Guid.Empty, Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that cancelling an item in a sale that does not exist throws a not found exception.
    /// </summary>
    [Fact(DisplayName = "Given unknown sale id When cancelling item Then throws not found exception")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = CancelSaleItemHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.SaleId} not found");
    }

    /// <summary>
    /// Tests that cancelling an item that does not exist throws a domain exception.
    /// </summary>
    [Fact(DisplayName = "Given unknown item id When cancelling item Then throws domain exception")]
    public async Task Handle_ItemNotFound_ThrowsDomainException()
    {
        // Given
        var sale = SaleTestData.GenerateValidSale();
        sale.Id = Guid.NewGuid();
        sale.AddItem(Guid.NewGuid(), "Beer", 2, 10m);
        sale.Items[0].Id = Guid.NewGuid();

        var command = CancelSaleItemHandlerTestData.GenerateValidCommand(sale.Id, Guid.NewGuid());
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<DomainException>()
            .WithMessage($"Item {command.ItemId} was not found in this sale");
    }
}
