using Currencies.Core.Interface;
using Currencies.Core.Model;
using Currencies.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Currencies.DataRepositories;

public class ExchangeRateRepository(AppDbContext dbContext, ILogger<ExchangeRateRepository> logger) : IExchangeRateRepository
{
    public async Task<ExchangeRate?> AddAsync(ExchangeRate exchangeRate)
    {
        var result = await AddExchange(exchangeRate);
        await dbContext.SaveChangesAsync();
        return result;
    }

    private async Task<ExchangeRate?> AddExchange(ExchangeRate exchangeRate)
    {
        var exists = await dbContext.Exchanges.FindAsync(exchangeRate.BaseCurrencyCode, exchangeRate.ExchangeCurrencyCode, exchangeRate.ExDate);
        if (exists != null) return null;
        var baseCurrency = await dbContext.Currencies.FindAsync(exchangeRate.BaseCurrencyCode) ?? new Currency { Code = exchangeRate.BaseCurrencyCode, Name = exchangeRate.BaseCurrency?.Name };
        var exchangeCurrency = await dbContext.Currencies.FindAsync(exchangeRate.ExchangeCurrencyCode) ?? new Currency { Code = exchangeRate.ExchangeCurrencyCode, Name = exchangeRate.ExchangeCurrency.Name };

        var result = await dbContext.AddAsync(new ExchangeRate
        {
            AskRate = exchangeRate.AskRate,
            BidRate = exchangeRate.BidRate,
            ExDate = exchangeRate.ExDate,
            BaseCurrency = baseCurrency,
            ExchangeCurrency = exchangeCurrency,
            BaseCurrencyCode = baseCurrency.Code,
            ExchangeCurrencyCode = exchangeCurrency.Code
        });
        return result.Entity;
    }

    public async Task<List<ExchangeRate>> AddBulkAsync(List<ExchangeRate> exchangeRate)
    {

        var failed = new List<ExchangeRate>();
        using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            foreach (var rate in exchangeRate)
            {
                if (await AddExchange(rate) is null)
                {
                    failed.Add(rate);
                }
            }
            await dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
        } catch(Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, ex.Message);
            throw;
        }
        return failed;
    }

    public Task<bool> AnyDataExists()
    {
        return dbContext.Exchanges.AnyAsync();
    }
}
