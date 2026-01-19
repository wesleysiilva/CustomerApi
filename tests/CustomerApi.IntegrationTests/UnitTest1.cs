using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CustomerApi.IntegrationTests;

public class CustomerApiIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        await _factory.DisposeAsync();
    }

    private StringContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    #region Create Customer Tests

    [Fact]
    public async Task CreateCustomer_WithValidData_ShouldReturn201()
    {
        // Arrange
        var createRequest = new
        {
            firstName = "John",
            lastName = "Doe",
            email = "john@example.com"
        };

        // Act
        var response = await _client.PostAsync("/api/customers", CreateJsonContent(createRequest));
        var responseBody = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var json = responseBody;
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        using var doc = JsonDocument.Parse(json);
        Assert.NotEqual(0, doc.RootElement.GetProperty("id").GetInt32());
        Assert.Equal("John Doe", doc.RootElement.GetProperty("fullName").GetString());
    }

    [Fact]
    public async Task CreateCustomer_WithMultipleCustomers_ShouldPersistAll()
    {
        // Arrange
        var customer1 = new { firstName = "Alice", lastName = "Smith", email = "alice@example.com" };
        var customer2 = new { firstName = "Bob", lastName = "Jones", email = "bob@example.com" };

        // Act
        var response1 = await _client.PostAsync("/api/customers", CreateJsonContent(customer1));
        var response2 = await _client.PostAsync("/api/customers", CreateJsonContent(customer2));

        // Assert
        Assert.Equal(HttpStatusCode.Created, response1.StatusCode);
        Assert.Equal(HttpStatusCode.Created, response2.StatusCode);
    }

    #endregion

    #region Get Customer Tests

    [Fact]
    public async Task GetCustomer_WithValidId_ShouldReturn200WithData()
    {
        // Arrange - Create a customer first
        var createRequest = new { firstName = "Charlie", lastName = "Brown", email = "charlie@example.com" };
        var createResponse = await _client.PostAsync("/api/customers", CreateJsonContent(createRequest));
        var createJson = await createResponse.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        using var createDoc = JsonDocument.Parse(createJson);
        var customerId = createDoc.RootElement.GetProperty("id").GetInt32();

        // Act
        var response = await _client.GetAsync($"/api/customers/{customerId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.Equal("Charlie Brown", doc.RootElement.GetProperty("fullName").GetString());
    }

    [Fact]
    public async Task GetCustomer_WithInvalidId_ShouldReturn404()
    {
        // Act
        var response = await _client.GetAsync("/api/customers/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region Get All Customers Tests

    [Fact]
    public async Task GetAllCustomers_WithNoCustomers_ShouldReturnEmptyList()
    {
        // Act
        var response = await _client.GetAsync("/api/customers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(0, doc.RootElement.GetArrayLength());
    }

    [Fact]
    public async Task GetAllCustomers_WithMultipleCustomers_ShouldReturnAll()
    {
        // Arrange
        var customer1 = new { firstName = "Frank", lastName = "Johnson", email = "frank@example.com" };
        var customer2 = new { firstName = "Grace", lastName = "Davis", email = "grace@example.com" };
        var customer3 = new { firstName = "Henry", lastName = "Martinez", email = "henry@example.com" };

        await _client.PostAsync("/api/customers", CreateJsonContent(customer1));
        await _client.PostAsync("/api/customers", CreateJsonContent(customer2));
        await _client.PostAsync("/api/customers", CreateJsonContent(customer3));

        // Act
        var response = await _client.GetAsync("/api/customers");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        using var doc = JsonDocument.Parse(json);
        Assert.Equal(3, doc.RootElement.GetArrayLength());
    }

    #endregion

    #region Update Customer Tests

    [Fact]
    public async Task UpdateCustomer_WithValidData_ShouldReturn200()
    {
        // Arrange - Create a customer first
        var createRequest = new { firstName = "David", lastName = "Lee", email = "david@example.com" };
        var createResponse = await _client.PostAsync("/api/customers", CreateJsonContent(createRequest));
        var createJson = await createResponse.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        using var createDoc = JsonDocument.Parse(createJson);
        var customerId = createDoc.RootElement.GetProperty("id").GetInt32();

        // Update request
        var updateRequest = new { id = customerId, firstName = "David", lastName = "Miller", email = "david.miller@example.com" };

        // Act
        var response = await _client.PutAsync($"/api/customers/{customerId}", CreateJsonContent(updateRequest));

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        Assert.Equal("David Miller", doc.RootElement.GetProperty("fullName").GetString());
    }

    #endregion

    #region Delete Customer Tests

    [Fact]
    public async Task DeleteCustomer_WithValidId_ShouldReturn204()
    {
        // Arrange - Create a customer first
        var createRequest = new { firstName = "Eve", lastName = "Wilson", email = "eve@example.com" };
        var createResponse = await _client.PostAsync("/api/customers", CreateJsonContent(createRequest));
        var createJson = await createResponse.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        using var createDoc = JsonDocument.Parse(createJson);
        var customerId = createDoc.RootElement.GetProperty("id").GetInt32();

        // Act
        var response = await _client.DeleteAsync($"/api/customers/{customerId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Verify customer is deleted
        var getResponse = await _client.GetAsync($"/api/customers/{customerId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteCustomer_WithNonExistentId_ShouldReturn400()
    {
        // Act
        var response = await _client.DeleteAsync("/api/customers/9999");

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region End-to-End Scenario

    [Fact]
    public async Task CompleteLifecycle_CreateReadUpdateDelete_ShouldWorkCorrectly()
    {
        // Create
        var createRequest = new { firstName = "Iris", lastName = "Taylor", email = "iris@example.com" };
        var createResponse = await _client.PostAsync("/api/customers", CreateJsonContent(createRequest));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createJson = await createResponse.Content.ReadAsStringAsync();
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        using var createDoc = JsonDocument.Parse(createJson);
        var customerId = createDoc.RootElement.GetProperty("id").GetInt32();
        Assert.Equal("Iris Taylor", createDoc.RootElement.GetProperty("fullName").GetString());

        // Read
        var getResponse = await _client.GetAsync($"/api/customers/{customerId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var getJson = await getResponse.Content.ReadAsStringAsync();
        using var getDoc = JsonDocument.Parse(getJson);
        Assert.Equal("Iris Taylor", getDoc.RootElement.GetProperty("fullName").GetString());

        // Update
        var updateRequest = new { id = customerId, firstName = "Iris", lastName = "Anderson", email = "iris.anderson@example.com" };
        var updateResponse = await _client.PutAsync($"/api/customers/{customerId}", CreateJsonContent(updateRequest));
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updateJson = await updateResponse.Content.ReadAsStringAsync();
        using var updateDoc = JsonDocument.Parse(updateJson);
        Assert.Equal("Iris Anderson", updateDoc.RootElement.GetProperty("fullName").GetString());

        // Delete
        var deleteResponse = await _client.DeleteAsync($"/api/customers/{customerId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        // Verify deletion
        var finalGetResponse = await _client.GetAsync($"/api/customers/{customerId}");
        Assert.Equal(HttpStatusCode.NotFound, finalGetResponse.StatusCode);
    }

    #endregion
}
