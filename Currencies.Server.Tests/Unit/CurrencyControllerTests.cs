using Currencies.Core.Interface;
using Currencies.Core.Model;
using Currencies.Server.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace Currencies.Server.Tests.Unit;

public class CurrencyControllerTests
{
    private readonly Mock<ICurrencyRepository> _mockCurrencyRepository;
    private readonly Mock<ILogger<CurrencyController>> _mockLogger;
    private readonly CurrencyController _controller;

    public CurrencyControllerTests()
    {
        _mockCurrencyRepository = new Mock<ICurrencyRepository>();
        _mockLogger = new Mock<ILogger<CurrencyController>>();
        _controller = new CurrencyController(_mockCurrencyRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetLast_ReturnsOkResult_WithListOfCurrencies()
    {
        // Arrange
        var currencies = new List<Currency> { new Currency { Code = "USD", Name = "dolar" }, new Currency { Code = "EUR", Name = "euro" } };
        _mockCurrencyRepository.Setup(repo => repo.GetLastAsync()).ReturnsAsync(currencies);

        // Act
        var result = await _controller.GetLast();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnCurrencies = Assert.IsType<List<Currency>>(okResult.Value);
        Assert.Equal(2, returnCurrencies.Count);
    }

    [Fact]
    public async Task GetLast_Returns500_WhenExceptionIsThrown()
    {
        // Arrange
        _mockCurrencyRepository.Setup(repo => repo.GetLastAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetLast();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        Assert.Equal("Something went wrong!", statusCodeResult.Value);
    }

    [Fact]
    public async Task GetExchanges_ReturnsOkResult_WithCurrency()
    {
        // Arrange
        var currency = new Currency { Code = "USD", Name = "dolar" };
        _mockCurrencyRepository.Setup(repo => repo.GetAsync("USD", It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(currency);

        // Act
        var result = await _controller.GetExchanges("USD", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnCurrency = Assert.IsType<Currency>(okResult.Value);
        Assert.Equal("USD", returnCurrency.Code);
    }

    [Fact]
    public async Task GetExchanges_ReturnsBadRequest_WhenCurrencyNotFound()
    {
        // Arrange
        _mockCurrencyRepository.Setup(repo => repo.GetAsync("USD", It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(null as Currency);

        // Act
        var result = await _controller.GetExchanges("USD", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Currency with USD not found!", badRequestResult.Value);
    }

    [Fact]
    public async Task GetExchanges_Returns500_WhenExceptionIsThrown()
    {
        // Arrange
        _mockCurrencyRepository.Setup(repo => repo.GetAsync("USD", It.IsAny<DateTime>(), It.IsAny<DateTime>())).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetExchanges("USD", DateTime.UtcNow.AddDays(-1), DateTime.UtcNow);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        Assert.Equal("Something went wrong!", statusCodeResult.Value);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfCurrencies()
    {
        // Arrange
        var currencies = new List<Currency> { new Currency { Code = "USD", Name = "dolar" }, new Currency { Code = "EUR", Name = "euro" } };
        _mockCurrencyRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(currencies);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnCurrencies = Assert.IsType<List<Currency>>(okResult.Value);
        Assert.Equal(2, returnCurrencies.Count);
    }

    [Fact]
    public async Task GetAll_Returns500_WhenExceptionIsThrown()
    {
        // Arrange
        _mockCurrencyRepository.Setup(repo => repo.GetAllAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        Assert.Equal("Something went wrong!", statusCodeResult.Value);
    }
}