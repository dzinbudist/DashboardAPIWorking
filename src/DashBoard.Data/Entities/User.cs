using System;
using System.ComponentModel.DataAnnotations;
using DashBoard.Data.Enums;

namespace DashBoard.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Role { get; set; }
        public string UserEmail { get; set; }
        public Guid Team_Key { get; set; }
        public int Created_By { get; set; } 
        public int Modified_By { get; set; } 
        public DateTime Date_Created { get; set; } 
        public DateTime Date_Modified { get; set; } = DateTime.Now;
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenValidDate { get; set; }
    }
}