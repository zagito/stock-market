using System.ComponentModel.DataAnnotations;

namespace Portfolio.API.Data.Entities
{
    public class Entity
    {
        [Key]
        public Guid Id { get; set; }
    }
}
