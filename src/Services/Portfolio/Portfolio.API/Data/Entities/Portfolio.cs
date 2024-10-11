namespace Portfolio.API.Data.Entities
{
    public class Portfolio : Entity
    {
        public Guid UserId { get; set; }

        public decimal Cash { get; set; }

        public ICollection<PortfolioStock> PortfolioStocks { get; } = new List<PortfolioStock>();

        public void AddStock(int quantity, Guid stockId) 
        {
            PortfolioStocks.Add(new PortfolioStock 
            {
                Id = Guid.NewGuid(),
                Quantity = quantity,
                StockId = stockId,
                PortfolioId = Id
            });
        }
    }
}
