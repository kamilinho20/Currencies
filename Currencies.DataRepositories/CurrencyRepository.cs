using Currencies.Core.Interface;
using Currencies.Core.Model;
using Currencies.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Currencies.DataRepositories;

public class CurrencyRepository(AppDbContext dbContext) : ICurrencyRepository
{
    public async Task<Currency> AddAsync(Currency currency)
    {
        var addedEntry = await dbContext.AddAsync(currency);
        return addedEntry.Entity;
    }

    public async Task<ICollection<Currency>> GetAllAsync()
    {
        var result = await dbContext.Currencies.ToListAsync();
        return result;
    }

    public Task<Currency?> GetAsync(string currencyCode, DateTime from, DateTime to)
    {
        return dbContext.Currencies.Where(c => c.Code == currencyCode).Include(c => c.ExchangeCurrencyRates.Where(ex => ex.ExDate >= from.Date && ex.ExDate <= to)).SingleOrDefaultAsync();
    }

    public async Task<ICollection<Currency>> GetLastAsync()
    {

        var lastExchangeDate = (await dbContext.Exchanges.OrderByDescending(ex => ex.ExDate).FirstOrDefaultAsync())?.ExDate ?? DateTime.Today;
        return await dbContext.Currencies
        .Select(c => new Currency
        {
            Code = c.Code,
            Name = c.Name,
            ExchangeCurrencyRates = c.ExchangeCurrencyRates
                .Where(ex => ex.ExDate >= lastExchangeDate)
                .ToList()
        })
        .ToListAsync();
    }
}