using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Currencies.DataServices.Tests")]

namespace Currencies.DataServices.Response;
internal record ExchangeTable
{
    public required string No { get; init; }
    public required DateOnly EffectiveDate { get; init; }
    public required DateOnly TradingDate { get; init; }
    public List<Rate> Rates { get; init; } = new List<Rate>();
}