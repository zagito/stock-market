namespace Order.API.Contracts.Orders
{
    public record CreateOrderRequest
    {
        public string Ticker { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public Side Side { get; set; }
    }
}
