using System.ComponentModel.DataAnnotations;

namespace CompanyDirectory.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name must contain only letters and spaces.")]
        public string? FirstName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Last name must contain only letters and spaces.")]
        public string? LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string? Email { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string? Mobile { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        // Captcha fields
        [Required(ErrorMessage = "Captcha answer is required")]
        public string? CaptchaAnswer { get; set; }

        public int CaptchaNum1 { get; set; }
        public int CaptchaNum2 { get; set; }
    }
}
