using System.ComponentModel.DataAnnotations;

namespace CompanyDirectory.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required, Phone]
        public string? Mobile { get; set; }

        [Required]
        public string? PasswordHash { get; set; }
    }
}
