namespace Portfolio.API.Data.Entities
{
    public class Portfolio : Entity
    {
        public Guid UserId { get; set; }

        public ICollection<PortfolioStock> PortfolioStocks { get; } = new List<PortfolioStock>();
    }
}
