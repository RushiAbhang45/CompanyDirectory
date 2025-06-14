using System.ComponentModel.DataAnnotations;

namespace CompanyDirectory.Models
{
    public class Product
    {
        public long Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        [Required]
        public long CompanyId { get; set; }

        public Company? Company { get; set; }

        public string? ImagePaths { get; set; } // Store multiple image paths, comma-separated

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
