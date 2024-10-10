namespace Portfolio.API.Data.Entities
{
    public class PortfolioStock : Entity
    {
        public int Quantity { get; set; }

        public decimal BuyPrice { get; set; }

        public Guid StockId { get; set; }

        public Stock Stock { get; set; } = null!;

        public Guid PortfolioId { get; set; }

        public Portfolio Portfolio { get; set; } = null!;
    }
}
