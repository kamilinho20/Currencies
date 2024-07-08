using Currencies.Core.Model;

namespace Currencies.Core.Interface;

public interface ICurrencyRepository
{
    Task<ICollection<Currency>> GetLastAsync();
    Task<ICollection<Currency>> GetAllAsync();
    Task<Currency?> GetAsync(string currencyCode, DateTime from, DateTime to);
    Task<Currency> AddAsync(Currency currency);
}