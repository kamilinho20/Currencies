using Currencies.DataServices.Response;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace Currencies.DataServices.Tests;

public class NBPDataServiceTests
{
    private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private readonly HttpClient _client;
    private readonly NBPDataService _service;

    public NBPDataServiceTests()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _client = new HttpClient(_mockHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://fakeurl.com/")
        };
        _service = new NBPDataService(_client);
    }

    [Fact]
    public async Task GetLastExchanges_ReturnsListOfExchangeRates()
    {
        // Arrange
        var responseContent = JsonSerializer.Serialize(new List<ExchangeTable>
        {
            new() {
                No = "01",
                TradingDate = new DateOnly(2023, 7, 1),
                EffectiveDate = new DateOnly(2023, 7, 1),
                Rates =
                [
                    new() { Code = "USD", Currency = "Dollar", Ask = 3.8m, Bid = 3.7m },
                    new() { Code = "EUR", Currency = "Euro", Ask = 4.5m, Bid = 4.4m }
                ]
            }
        });

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        // Act
        var result = await _service.GetLastExchanges();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("USD", result[0].ExchangeCurrencyCode);
        Assert.Equal("EUR", result[1].ExchangeCurrencyCode);
    }

    [Fact]
    public async Task GetHistoricalExchanges_ReturnsListOfExchangeRates()
    {
        // Arrange
        var dateFrom = new DateTime(2023, 1, 1);
        var dateTo = new DateTime(2023, 1, 31);
        var responseContent = JsonSerializer.Serialize(new List<ExchangeTable>
        {
            new() {
                No = "02",
                TradingDate = new DateOnly(2023, 1, 15),
                EffectiveDate = new DateOnly(2023, 1, 15),
                Rates =
                [
                    new() { Code = "USD", Currency = "Dollar", Ask = 3.9m, Bid = 3.8m }
                ]
            }
        });

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        // Act
        var result = await _service.GetHistoricalExchanges(dateFrom, dateTo);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("USD", result[0].ExchangeCurrencyCode);
    }

    [Fact]
    public async Task GetLastExchanges_ReturnsEmptyList_WhenNoData()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[]")
            });

        // Act
        var result = await _service.GetLastExchanges();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetHistoricalExchanges_ReturnsEmptyList_WhenNoData()
    {
        // Arrange
        var dateFrom = new DateTime(2023, 1, 1);
        var dateTo = new DateTime(2023, 1, 31);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("[]")
            });

        // Act
        var result = await _service.GetHistoricalExchanges(dateFrom, dateTo);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetLastExchanges_ThrowsException_OnHttpRequestException()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException());

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetLastExchanges());
    }

    [Fact]
    public async Task GetHistoricalExchanges_ThrowsException_OnHttpRequestException()
    {
        // Arrange
        var dateFrom = new DateTime(2023, 1, 1);
        var dateTo = new DateTime(2023, 1, 31);

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new HttpRequestException());

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _service.GetHistoricalExchanges(dateFrom, dateTo));
    }
}
