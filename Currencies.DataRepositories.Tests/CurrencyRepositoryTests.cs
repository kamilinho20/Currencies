namespace Currencies.DataRepositories.Tests;

using Currencies.Core.Model;
using Currencies.DataAccess;
using Currencies.DataRepositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class CurrencyRepositoryTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly CurrencyRepository _repository;

    public CurrencyRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _dbContext = new AppDbContext(options);
        _repository = new CurrencyRepository(_dbContext);
    }

    [Fact]
    public async Task AddAsync_ShouldAddCurrency()
    {
        // Arrange
        var currency = new Currency
        {
            Code = "USD",
            Name = "US Dollar"
        };

        // Act
        var result = await _repository.AddAsync(currency);
        await _dbContext.SaveChangesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("USD", result.Code);
    }

  

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllCurrencies()
    {
        // Arrange
        var currency1 = new Currency { Code = "USD", Name = "US Dollar" };
        var currency2 = new Currency { Code = "EUR", Name = "Euro" };

        await _dbContext.Currencies.AddRangeAsync(currency1, currency2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Code == "USD");
        Assert.Contains(result, c => c.Code == "EUR");
    }

    [Fact]
    public async Task GetAsync_ShouldReturnCurrencyWithExchangeRates()
    {
        // Arrange
        var currency = new Currency
        {
            Code = "USD",
            Name = "US Dollar",
            ExchangeCurrencyRates = new List<ExchangeRate>
            {
                new ExchangeRate { ExDate = DateTime.Today, AskRate = 1.2m, BidRate = 1.1m, BaseCurrencyCode = "PLN", ExchangeCurrencyCode = "USD" },
                new ExchangeRate { ExDate = DateTime.Today.AddDays(-1), AskRate = 1.2m, BidRate = 1.1m, BaseCurrencyCode = "PLN", ExchangeCurrencyCode = "USD" }
            }
        };

        await _dbContext.Currencies.AddAsync(currency);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync("USD", DateTime.Today.AddDays(-2), DateTime.Today);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("USD", result.Code);
        Assert.Equal(2, result.ExchangeCurrencyRates.Count);
    }

    [Fact]
    public async Task GetLastAsync_ShouldReturnCurrenciesWithLastExchangeRates()
    {
        // Arrange
        var currency = new Currency
        {
            Code = "USD",
            Name = "US Dollar",
            ExchangeCurrencyRates =
            [
                new() { ExDate = DateTime.Today, AskRate = 1.2m, BidRate = 1.1m, BaseCurrencyCode = "PLN", ExchangeCurrencyCode = "USD" },
                new() { ExDate = DateTime.Today.AddDays(-1), AskRate = 1.2m, BidRate = 1.1m, BaseCurrencyCode = "PLN", ExchangeCurrencyCode = "USD" }
            ]
        };

        await _dbContext.Currencies.AddAsync(currency);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetLastAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("USD", result.First().Code);
        Assert.Single(result.First().ExchangeCurrencyRates);
        Assert.Equal(DateTime.Today, result.First().ExchangeCurrencyRates.First().ExDate);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
    }
}