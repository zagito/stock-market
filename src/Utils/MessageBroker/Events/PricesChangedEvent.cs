namespace MessageBroker.Events
{
    public record PricesChangedEvent(Dictionary<string, decimal> StockPrices);
}
