using MessageBroker.Events;
using Proto.Custom.Types;
using System.ComponentModel.DataAnnotations;

namespace Order.API.Data.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public int Quantity { get; set; }

        public Side Side { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? ExecuteDate { get; set; }

        [Required]
        [StringLength(50)]
        public required string Ticker { get; set; }

        public bool? IsSuccessful { get; set; }

        [StringLength(100)]
        public string? FailureReason { get; set; }

        public Guid UserId { get; set; }

        public decimal CurrentPrice { get; set; }
    }
}
