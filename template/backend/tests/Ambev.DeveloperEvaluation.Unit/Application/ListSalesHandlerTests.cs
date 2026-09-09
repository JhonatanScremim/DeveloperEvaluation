using Ambev.DeveloperEvaluation.Application.Common;
using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="ListSalesHandler"/> class.
/// </summary>
public class ListSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ListSalesHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListSalesHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public ListSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListSalesHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid sales listing request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid listing data When listing sales Then returns paginated response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = ListSalesHandlerTestData.GenerateValidCommand();
        var sale = SaleTestData.GenerateValidSale();
        var sales = new List<Sale> { sale };
        var criteria = new SaleListCriteria { Page = command.Page, Size = command.Size };
        var saleResults = new List<SaleResult> { new() { Id = sale.Id } };
        const int totalCount = 25;

        _mapper.Map<SaleListCriteria>(command).Returns(criteria);
        _saleRepository.ListAsync(criteria, Arg.Any<CancellationToken>())
            .Returns((sales, totalCount));
        _mapper.Map<List<SaleResult>>(sales).Returns(saleResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
        result.Data[0].Id.Should().Be(sale.Id);
        result.CurrentPage.Should().Be(command.Page);
        result.TotalCount.Should().Be(totalCount);
        result.TotalPages.Should().Be(3);
        await _saleRepository.Received(1).ListAsync(criteria, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid sales listing request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid paging data When listing sales Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new ListSalesCommand { Page = 0, Size = 10 };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that an empty sales list returns an empty paginated response.
    /// </summary>
    [Fact(DisplayName = "Given no matching sales When listing sales Then returns empty paginated response")]
    public async Task Handle_NoSalesFound_ReturnsEmptyResult()
    {
        // Given
        var command = ListSalesHandlerTestData.GenerateValidCommand();
        var criteria = new SaleListCriteria { Page = command.Page, Size = command.Size };
        var sales = new List<Sale>();
        var saleResults = new List<SaleResult>();

        _mapper.Map<SaleListCriteria>(command).Returns(criteria);
        _saleRepository.ListAsync(criteria, Arg.Any<CancellationToken>())
            .Returns((sales, 0));
        _mapper.Map<List<SaleResult>>(sales).Returns(saleResults);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.CurrentPage.Should().Be(command.Page);
    }

    /// <summary>
    /// Tests that the mapper is called with the listing command.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps command to listing criteria")]
    public async Task Handle_ValidRequest_MapsCommandToCriteria()
    {
        // Given
        var command = ListSalesHandlerTestData.GenerateValidCommand();
        var criteria = new SaleListCriteria
        {
            Page = command.Page,
            Size = command.Size,
            Order = command.Order,
            SaleNumber = command.SaleNumber,
            CustomerName = command.CustomerName,
            BranchName = command.BranchName
        };
        var sales = new List<Sale>();

        _mapper.Map<SaleListCriteria>(command).Returns(criteria);
        _saleRepository.ListAsync(criteria, Arg.Any<CancellationToken>())
            .Returns((sales, 0));
        _mapper.Map<List<SaleResult>>(sales).Returns(new List<SaleResult>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<SaleListCriteria>(Arg.Is<ListSalesCommand>(c =>
            c.Page == command.Page &&
            c.Size == command.Size &&
            c.Order == command.Order &&
            c.SaleNumber == command.SaleNumber &&
            c.CustomerName == command.CustomerName &&
            c.BranchName == command.BranchName));
    }
}
