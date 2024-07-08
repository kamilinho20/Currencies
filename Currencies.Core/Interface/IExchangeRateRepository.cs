using Currencies.Core.Model;

namespace Currencies.Core.Interface;

public interface IExchangeRateRepository
{
    Task<bool> AnyDataExists();
    Task<ExchangeRate?> AddAsync(ExchangeRate exchangeRate);
    Task<List<ExchangeRate>> AddBulkAsync(List<ExchangeRate> exchangeRate);
}