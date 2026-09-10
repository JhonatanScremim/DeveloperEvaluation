using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
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
/// Contains unit tests for the <see cref="CancelSaleHandler"/> class.
/// </summary>
public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;
    private readonly CancelSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _eventPublisher = Substitute.For<IEventPublisher>();
        _handler = new CancelSaleHandler(_saleRepository, _mapper, _eventPublisher);
    }

    /// <summary>
    /// Tests that a valid sale cancellation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given existing sale When cancelling sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
        var sale = SaleTestData.GenerateValidSale();
        sale.Id = command.Id;
        sale.AddItem(Guid.NewGuid(), "Beer", 2, 10m);

        var result = new SaleResult
        {
            Id = sale.Id,
            Cancelled = true
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(result);

        // When
        var cancelSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        cancelSaleResult.Should().NotBeNull();
        cancelSaleResult.Id.Should().Be(sale.Id);
        sale.Cancelled.Should().BeTrue();
        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await _eventPublisher.Received(1).PublishAsync(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid sale cancellation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale id When cancelling sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CancelSaleCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that cancelling a sale that does not exist throws a not found exception.
    /// </summary>
    [Fact(DisplayName = "Given unknown sale id When cancelling sale Then throws not found exception")]
    public async Task Handle_SaleNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    /// <summary>
    /// Tests that the mapper is called with the cancelled sale entity.
    /// </summary>
    [Fact(DisplayName = "Given existing sale When handling Then maps sale entity to result")]
    public async Task Handle_ValidRequest_MapsSaleToResult()
    {
        // Given
        var command = CancelSaleHandlerTestData.GenerateValidCommand();
        var sale = SaleTestData.GenerateValidSale();
        sale.Id = command.Id;

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(new SaleResult { Id = sale.Id });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<SaleResult>(sale);
    }
}
