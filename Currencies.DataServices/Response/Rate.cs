namespace Currencies.DataServices.Response;
internal record Rate
{
    public required string Code { get; init; }
    public required string Currency { get; init; }
    public required decimal Bid { get; init; }
    public required decimal Ask { get; init; }

}