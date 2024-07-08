namespace Currencies.Core.Model;

public record Currency
{
    public required string Code { get; init; }
    public required string Name { get; init; }

    public ICollection<ExchangeRate> BaseCurrencyRates { get; init; } = [];
    public ICollection<ExchangeRate> ExchangeCurrencyRates { get; init; } = [];

}
