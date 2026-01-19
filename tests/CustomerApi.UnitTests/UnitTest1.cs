using CustomerApi.Domain;
using CustomerApi.Repositories;
using CustomerApi.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace CustomerApi.UnitTests;

public class CustomerServiceTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<ILogger<CustomerService>> _mockLogger;
    private readonly CustomerService _service;

    public CustomerServiceTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockLogger = new Mock<ILogger<CustomerService>>();
        _service = new CustomerService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidCustomer_ReturnsSuccess()
    {
        var customer = new Customer { FirstName = "John", LastName = "Doe", Email = "john@example.com" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Customer>())).ReturnsAsync(customer);

        var result = await _service.CreateAsync(customer);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("John", result.Data.FirstName);
    }

    [Fact]
    public async Task GetAsync_WithValidId_ReturnsCustomer()
    {
        var customer = new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" };
        _mockRepository.Setup(r => r.GetAsync(1)).ReturnsAsync(customer);

        var result = await _service.GetAsync(1);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(1, result.Data.Id);
    }

    [Fact]
    public async Task GetAsync_WithInvalidId_ReturnsFail()
    {
        _mockRepository.Setup(r => r.GetAsync(999)).ReturnsAsync((Customer?)null);

        var result = await _service.GetAsync(999);

        Assert.False(result.IsSuccess);
        Assert.Equal("Customer not found.", result.Message);
    }

    [Fact]
    public async Task UpdateAsync_WithValidCustomer_ReturnsSuccess()
    {
        var customer = new Customer { Id = 1, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com" };
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Customer>())).ReturnsAsync(customer);

        var result = await _service.UpdateAsync(customer);

        Assert.True(result.IsSuccess);
        Assert.Equal("Jane", result.Data?.FirstName);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ReturnsSuccess()
    {
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result.IsSuccess);
        Assert.True(result.Data);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCustomers()
    {
        var customers = new List<Customer>
        {
            new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
            new Customer { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

        var result = await _service.GetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Data?.Count);
    }

    [Fact]
    public async Task CreateAsync_WhenRepositoryThrows_ReturnsFail()
    {
        var customer = new Customer { FirstName = "John", LastName = "Doe", Email = "john@example.com" };
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<Customer>())).ThrowsAsync(new Exception("Database error"));

        var result = await _service.CreateAsync(customer);

        Assert.False(result.IsSuccess);
        Assert.Equal("An error occurred while creating the customer.", result.Message);
    }
}

