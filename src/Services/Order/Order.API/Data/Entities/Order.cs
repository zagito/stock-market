using Order.API.Contracts.Orders;
using System.ComponentModel.DataAnnotations;

namespace Order.API.Data.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public Side Side { get; set; }

        [Required]
        [StringLength(50)]
        public required string Ticker { get; set; }

        public OrderStatus Status { get; set; }

        [StringLength(50)]
        public required string StatusNormalized { get; set; }


        [StringLength(100)]
        public string? FailureReason { get; set; }


    }

    public enum OrderStatus 
    {
        Created,
        Execudet,
        Failed
    }
}
