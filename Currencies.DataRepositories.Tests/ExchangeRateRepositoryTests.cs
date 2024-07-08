namespace Currencies.DataRepositories.Tests;

using Currencies.Core.Model;
using Currencies.DataAccess;
using Currencies.DataRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

public class ExchangeRateRepositoryTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly Mock<ILogger<ExchangeRateRepository>> _mockLogger;
    private readonly ExchangeRateRepository _repository;

    public ExchangeRateRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _dbContext = new AppDbContext(options);
        _mockLogger = new Mock<ILogger<ExchangeRateRepository>>();
        _repository = new ExchangeRateRepository(_dbContext, _mockLogger.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldAddExchangeRate()
    {
        // Arrange
        var exchangeRate = new ExchangeRate
        {
            BaseCurrencyCode = "USD",
            ExchangeCurrencyCode = "EUR",
            ExDate = DateTime.Now,
            AskRate = 1.2m,
            BidRate = 1.1m,
            BaseCurrency = new Currency { Code = "USD", Name = "US Dollar" },
            ExchangeCurrency = new Currency { Code = "EUR", Name = "Euro" }
        };

        // Act
        var result = await _repository.AddAsync(exchangeRate);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("USD", result.BaseCurrencyCode);
        Assert.Equal("EUR", result.ExchangeCurrencyCode);
        Assert.True(await _dbContext.Exchanges.AnyAsync(ex => ex.Equals(result)));
    }

    [Fact]
    public async Task AddAsync_ShouldReturnNull_WhenExchangeRateExists()
    {
        // Arrange
        var exchangeRate = new ExchangeRate
        {
            BaseCurrencyCode = "USD",
            ExchangeCurrencyCode = "EUR",
            ExDate = DateTime.Now,
            AskRate = 1.2m,
            BidRate = 1.1m,
            BaseCurrency = new Currency { Code = "USD", Name = "US Dollar" },
            ExchangeCurrency = new Currency { Code = "EUR", Name = "Euro" }
        };

        await _dbContext.Exchanges.AddAsync(exchangeRate);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.AddAsync(exchangeRate);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddBulkAsync_ShouldAddMultipleExchangeRates()
    {
        // Arrange
        var exchangeRates = new List<ExchangeRate>
        {
            new ExchangeRate
            {
                BaseCurrencyCode = "USD",
                ExchangeCurrencyCode = "EUR",
                ExDate = DateTime.Now,
                AskRate = 1.2m,
                BidRate = 1.1m,
                BaseCurrency = new Currency { Code = "USD", Name = "US Dollar" },
                ExchangeCurrency = new Currency { Code = "EUR", Name = "Euro" }
            },
            new ExchangeRate
            {
                BaseCurrencyCode = "USD",
                ExchangeCurrencyCode = "GBP",
                ExDate = DateTime.Now,
                AskRate = 1.3m,
                BidRate = 1.2m,
                BaseCurrency = new Currency { Code = "USD", Name = "US Dollar" },
                ExchangeCurrency = new Currency { Code = "GBP", Name = "British Pound" }
            }
        };

        // Act
        var result = await _repository.AddBulkAsync(exchangeRates);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        Assert.True(await _dbContext.Exchanges.AnyAsync(ex => ex.Equals(exchangeRates[0])));
        Assert.True(await _dbContext.Exchanges.AnyAsync(ex => ex.Equals(exchangeRates[1])));
    }

    [Fact]
    public async Task AddBulkAsync_ShouldReturnFailedExchangeRates_WhenSomeExist()
    {
        // Arrange
        var existingExchangeRate = new ExchangeRate
        {
            BaseCurrencyCode = "USD",
            ExchangeCurrencyCode = "EUR",
            ExDate = DateTime.Now,
            AskRate = 1.2m,
            BidRate = 1.1m,
            BaseCurrency = new Currency { Code = "USD", Name = "US Dollar" },
            ExchangeCurrency = new Currency { Code = "EUR", Name = "Euro" }
        };

        await _dbContext.Exchanges.AddAsync(existingExchangeRate);
        await _dbContext.SaveChangesAsync();

        var exchangeRates = new List<ExchangeRate> { existingExchangeRate };

        // Act
        var result = await _repository.AddBulkAsync(exchangeRates);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("USD", result[0].BaseCurrencyCode);
        Assert.Equal("EUR", result[0].ExchangeCurrencyCode);
    }

    [Fact]
    public async Task AnyDataExists_ShouldReturnTrue_WhenDataExists()
    {
        // Arrange
        var exchangeRate = new ExchangeRate
        {
            BaseCurrencyCode = "USD",
            ExchangeCurrencyCode = "EUR",
            ExDate = DateTime.Now,
            AskRate = 1.2m,
            BidRate = 1.1m,
            BaseCurrency = new Currency { Code = "USD", Name = "US Dollar" },
            ExchangeCurrency = new Currency { Code = "EUR", Name = "Euro" }
        };

        await _dbContext.Exchanges.AddAsync(exchangeRate);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.AnyDataExists();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task AnyDataExists_ShouldReturnFalse_WhenNoDataExists()
    {
        // Act
        var result = await _repository.AnyDataExists();

        // Assert
        Assert.False(result);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
    }
}