using System.ComponentModel.DataAnnotations;
namespace DashBoard.Business.DTOs.Users
{
  public class UpdateModelDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 10)]
        [RegularExpression("^(?=\\S*[a-z])(?=\\S*[A-Z])(?=\\S*\\d)(?=\\S*[\\W_])\\S{10,128}$", ErrorMessage = "Passwords must be 10 to 128 characters long and contain: upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The passwords do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }
        public string Role { get; set; }
    }
}