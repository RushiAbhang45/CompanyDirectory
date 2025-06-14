using System.ComponentModel.DataAnnotations;

namespace CompanyDirectory.ViewModels
{
    public class ProductViewModel
    {
        public long Id { get; set; }

        [Required]
        public string? Name { get; set; }

        public string? Description { get; set; }

        public long CompanyId { get; set; }

        public List<IFormFile>? Images { get; set; }

    }
}
