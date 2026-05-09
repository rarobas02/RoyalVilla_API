using System.ComponentModel.DataAnnotations;

namespace RoyalVilla_API.Models
{
    public class VillaAmenities
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
