using System.ComponentModel.DataAnnotations;

namespace CompanyDirectory.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "First Name")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "First name must contain only letters and spaces.")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
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
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }

        // Captcha fields
        [Required(ErrorMessage = "Captcha answer is required")]
        [Display(Name = "Captcha Answer")]
        public string? CaptchaAnswer { get; set; }

        public int CaptchaNum1 { get; set; }
        public int CaptchaNum2 { get; set; }

        // New Role field
        [Required(ErrorMessage = "Please select a role.")]
        [Display(Name = "Select Role")]
        public string? Role { get; set; }
    }
}
