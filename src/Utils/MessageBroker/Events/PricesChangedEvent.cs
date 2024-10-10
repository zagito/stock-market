namespace MessageBroker.Events
{
    public record PricesChangedEvent(StockPrice[] StockPrices);

    public record StockPrice(string Ticker, decimal Price);
}
