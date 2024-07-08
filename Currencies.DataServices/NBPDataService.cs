using Currencies.Core.Interface;
using Currencies.Core.Model;
using Currencies.DataServices.Response;
using System.Net.Http.Json;

namespace Currencies.DataServices;
public class NBPDataService(HttpClient client) : IExchangesDataService
{
    const string AllCurrenciesEndpoint = "tables/c/last?format=json";
    const string CurrencyHistoricalEndpoint = "tables/c/{0:yyyy-MM-dd}/{1:yyyy-MM-dd}?format=json";


    public async Task<List<ExchangeRate>> GetLastExchanges(CancellationToken ct = default)
    {
        var currenciesData = await client.GetFromJsonAsync<List<ExchangeTable>>(AllCurrenciesEndpoint, ct);
        return currenciesData?.SelectMany(cd => cd.Rates.Select(r => new {cd.EffectiveDate, rate = r})).Select(r => new ExchangeRate {
            BaseCurrencyCode = "PLN",
            ExchangeCurrencyCode = r.rate.Code,
            ExchangeCurrency = new Currency { Code = r.rate.Code, Name = r.rate.Currency },
            BaseCurrency = new Currency { Code = "PLN", Name = "Polski złoty"},
            AskRate = r.rate.Ask,
            BidRate = r.rate.Bid,
            ExDate = r.EffectiveDate.ToDateTime(TimeOnly.MinValue)
        })?.ToList() ?? [];
    }

    public async Task<List<ExchangeRate>> GetHistoricalExchanges(DateTime dateFrom, DateTime dateTo, CancellationToken ct = default)
    {
        var currenciesData = await client.GetFromJsonAsync<List<ExchangeTable>>(string.Format(CurrencyHistoricalEndpoint, dateFrom, dateTo), ct);
        return currenciesData?.SelectMany(cd => cd.Rates.Select(r => new { cd.EffectiveDate, rate = r })).Select(r => new ExchangeRate
        {
            BaseCurrencyCode = "PLN",
            ExchangeCurrencyCode = r.rate.Code,
            ExchangeCurrency = new Currency { Code = r.rate.Code, Name = r.rate.Currency },
            BaseCurrency = new Currency { Code = "PLN", Name = "Polski złoty" },
            AskRate = r.rate.Ask,
            BidRate = r.rate.Bid,
            ExDate = r.EffectiveDate.ToDateTime(TimeOnly.MinValue)
        })?.ToList() ?? [];
    }
}
