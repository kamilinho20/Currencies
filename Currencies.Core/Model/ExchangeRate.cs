using System.Text.Json.Serialization;

namespace Currencies.Core.Model
{
    public record ExchangeRate
    {
        public required string BaseCurrencyCode { get; init; }
        public required string ExchangeCurrencyCode { get; init; }
        public required decimal BidRate { get; init; }
        public required decimal AskRate { get; init; }
        public required DateTime ExDate { get; init; }

        [JsonIgnore]
        public Currency? BaseCurrency { get; init; }
        [JsonIgnore]
        public Currency? ExchangeCurrency { get; init; }


    }
}
