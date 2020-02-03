using System.ComponentModel.DataAnnotations;

namespace WebApi.Business.DTOs.Users
{
    public class AuthenticateModelDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}