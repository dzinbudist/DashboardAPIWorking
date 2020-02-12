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
        [Required]
        public bool Active { get; set; }
        [Required]
        public string Role { get; set; }
    }
}