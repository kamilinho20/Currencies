using Currencies.Core.Model;

namespace Currencies.Core.Interface;

public interface IExchangesDataService
{
    Task<List<ExchangeRate>> GetLastExchanges(CancellationToken ct);
    Task<List<ExchangeRate>> GetHistoricalExchanges(DateTime dateFrom, DateTime dateTo, CancellationToken ct);
}
