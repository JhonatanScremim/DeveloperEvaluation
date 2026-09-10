using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Functional.TestData;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Features.Sales;

/// <summary>
/// Functional tests for the Sales API endpoints.
/// </summary>
public class SalesApiTests : IClassFixture<SalesWebApplicationFactory>
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _client;

    /// <summary>
    /// Initializes a new instance of <see cref="SalesApiTests"/>.
    /// </summary>
    /// <param name="factory">The web application factory used to create the HTTP client.</param>
    public SalesApiTests(SalesWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    /// <summary>
    /// Tests that creating a valid sale returns 201 with the persisted sale.
    /// </summary>
    [Fact(DisplayName = "Given valid sale When creating Then returns created sale")]
    public async Task CreateSale_ValidRequest_ReturnsCreated()
    {
        // Given
        var request = SaleApiTestData.NewCreateRequest();

        // When
        var response = await _client.PostAsJsonAsync("/api/sales", request);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var sale = await ReadSaleAsync(response);
        sale.Id.Should().NotBe(Guid.Empty);
        sale.Items.Should().ContainSingle(i => i.Product.Name == "Beer");
    }

    /// <summary>
    /// Tests that getting an existing sale by id returns 200 with the sale details.
    /// </summary>
    [Fact(DisplayName = "Given existing sale When getting by id Then returns sale")]
    public async Task GetSale_ExistingId_ReturnsSale()
    {
        // Given
        var created = await CreateSaleAsync();

        // When
        var response = await _client.GetAsync($"/api/sales/{created.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sale = await ReadSaleAsync(response);
        sale.Id.Should().Be(created.Id);
        sale.SaleNumber.Should().Be(created.SaleNumber);
    }

    /// <summary>
    /// Tests that listing sales with a sale number filter returns the matching sale.
    /// </summary>
    [Fact(DisplayName = "Given existing sales When listing Then returns matching sale")]
    public async Task ListSales_WithSaleNumberFilter_ReturnsMatch()
    {
        // Given
        var created = await CreateSaleAsync();

        // When
        var response = await _client.GetAsync($"/api/sales?saleNumber={created.SaleNumber}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sales = await ReadSalesAsync(response);
        sales.Should().Contain(s => s.Id == created.Id);
    }

    /// <summary>
    /// Tests that updating an existing sale returns 200 with the updated data.
    /// </summary>
    [Fact(DisplayName = "Given existing sale When updating Then returns updated customer")]
    public async Task UpdateSale_ValidRequest_ReturnsUpdated()
    {
        // Given
        var created = await CreateSaleAsync();
        var update = SaleApiTestData.NewUpdateRequest();

        // When
        var response = await _client.PutAsJsonAsync($"/api/sales/{created.Id}", update);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sale = await ReadSaleAsync(response);
        sale.Customer.Name.Should().Be("Jane Customer");
        sale.Items.Should().ContainSingle(i => i.Product.Name == "Wine");
    }

    /// <summary>
    /// Tests that cancelling a sale returns 200 and marks the sale as cancelled.
    /// </summary>
    [Fact(DisplayName = "Given existing sale When cancelling Then marks sale as cancelled")]
    public async Task CancelSale_ExistingId_ReturnsCancelled()
    {
        // Given
        var created = await CreateSaleAsync();

        // When
        var response = await _client.DeleteAsync($"/api/sales/{created.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sale = await ReadSaleAsync(response);
        sale.Cancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(0m);
    }

    /// <summary>
    /// Tests that cancelling a sale item returns 200 and recalculates the sale total.
    /// </summary>
    [Fact(DisplayName = "Given sale with items When cancelling one item Then recalculates total")]
    public async Task CancelSaleItem_ExistingItem_ReturnsUpdatedSale()
    {
        // Given
        var created = await CreateSaleAsync(itemCount: 2);
        var beerId = created.Items.Single(i => i.Product.Name == "Beer").Id;

        // When
        var response = await _client.DeleteAsync($"/api/sales/{created.Id}/item/{beerId}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var sale = await ReadSaleAsync(response);
        sale.Cancelled.Should().BeFalse();
        sale.Items.Single(i => i.Id == beerId).Cancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(20m);
    }

    /// <summary>
    /// Creates a sale through the API for use as test arrange data.
    /// </summary>
    /// <param name="itemCount">The number of items to include in the sale.</param>
    /// <returns>The created sale response.</returns>
    private async Task<SaleResponse> CreateSaleAsync(int itemCount = 1)
    {
        var request = SaleApiTestData.NewCreateRequest(itemCount: itemCount);
        var response = await _client.PostAsJsonAsync("/api/sales", request);
        response.EnsureSuccessStatusCode();
        return await ReadSaleAsync(response);
    }

    /// <summary>
    /// Reads the sale payload whether the API returned a single or double envelope
    /// (Created vs Ok via BaseController).
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    /// <returns>The deserialized sale.</returns>
    private static async Task<SaleResponse> ReadSaleAsync(HttpResponseMessage response)
    {
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = doc.RootElement.GetProperty("data");

        if (data.ValueKind == JsonValueKind.Object &&
            data.TryGetProperty("data", out var inner) &&
            inner.ValueKind == JsonValueKind.Object)
        {
            return inner.Deserialize<SaleResponse>(JsonOptions)!;
        }

        return data.Deserialize<SaleResponse>(JsonOptions)!;
    }

    /// <summary>
    /// Reads the sales list payload whether the API returned a single or double envelope.
    /// </summary>
    /// <param name="response">The HTTP response message.</param>
    /// <returns>The deserialized sales list.</returns>
    private static async Task<List<SaleResponse>> ReadSalesAsync(HttpResponseMessage response)
    {
        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var data = doc.RootElement.GetProperty("data");

        if (data.ValueKind == JsonValueKind.Object &&
            data.TryGetProperty("data", out var inner) &&
            inner.ValueKind == JsonValueKind.Array)
        {
            return inner.Deserialize<List<SaleResponse>>(JsonOptions)!;
        }

        return data.Deserialize<List<SaleResponse>>(JsonOptions)!;
    }
}
