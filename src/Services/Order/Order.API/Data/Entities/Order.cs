using Order.API.Contracts.Orders;
using System.ComponentModel.DataAnnotations;

namespace Order.API.Data.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        public Side Side { get; set; }

        [Required]
        [StringLength(50)]
        public required string Ticker { get; set; }
    }
}
