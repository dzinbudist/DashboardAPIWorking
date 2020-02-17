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
        public string Password { get; set; }
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }
        public bool UpdatedByAdmin { get; set; }
    }
}