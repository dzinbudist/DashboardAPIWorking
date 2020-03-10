using System.ComponentModel.DataAnnotations;

namespace DashBoard.Business.DTOs.Users
{
    public class TokenRefresh
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
