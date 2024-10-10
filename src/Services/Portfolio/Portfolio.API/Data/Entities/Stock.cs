using System.ComponentModel.DataAnnotations;

namespace Portfolio.API.Data.Entities
{
    public class Stock : Entity
    {
        [Required]
        [StringLength(50)]
        public required string Ticker { get; set; }

        public decimal Price { get; set; }

        public ICollection<PortfolioStock> PortfolioStocks { get; } = new List<PortfolioStock>();
    }
}
