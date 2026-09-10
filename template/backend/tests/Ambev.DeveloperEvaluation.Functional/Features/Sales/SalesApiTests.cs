using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Functional.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
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
        var body = await ReadAsync<ApiResponseWithData<SaleResponse>>(response);
        body!.Data.Should().NotBeNull();
        body.Data!.Id.Should().NotBe(Guid.Empty);
        body.Data.Items.Should().ContainSingle(i => i.Product.Name == "Beer");
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
        var body = await ReadAsync<ApiResponseWithData<SaleResponse>>(response);
        body!.Data!.Id.Should().Be(created.Id);
        body.Data.SaleNumber.Should().Be(created.SaleNumber);
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
        var body = await ReadAsync<SalesListBody>(response);
        body!.Data.Should().Contain(s => s.Id == created.Id);
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
        var body = await ReadAsync<ApiResponseWithData<SaleResponse>>(response);
        body!.Data!.Customer.Name.Should().Be("Jane Customer");
        body.Data.Items.Should().ContainSingle(i => i.Product.Name == "Wine");
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
        var body = await ReadAsync<ApiResponseWithData<SaleResponse>>(response);
        body!.Data!.Cancelled.Should().BeTrue();
        body.Data.TotalAmount.Should().Be(0m);
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
        var body = await ReadAsync<ApiResponseWithData<SaleResponse>>(response);
        body!.Data!.Cancelled.Should().BeFalse();
        body.Data.Items.Single(i => i.Id == beerId).Cancelled.Should().BeTrue();
        body.Data.TotalAmount.Should().Be(20m);
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

        var body = await ReadAsync<ApiResponseWithData<SaleResponse>>(response);
        return body!.Data!;
    }

    /// <summary>
    /// Deserializes the HTTP response body.
    /// </summary>
    /// <typeparam name="T">The response body type.</typeparam>
    /// <param name="response">The HTTP response message.</param>
    /// <returns>The deserialized body.</returns>
    private static async Task<T?> ReadAsync<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOptions);
    }

    /// <summary>
    /// Response body used to deserialize the paginated sales list.
    /// </summary>
    private sealed class SalesListBody
    {
        public List<SaleResponse>? Data { get; set; }
    }
}
