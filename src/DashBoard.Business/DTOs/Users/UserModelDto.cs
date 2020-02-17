using System;

namespace DashBoard.Business.DTOs.Users
{
  public class UserModelDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string UserEmail { get; set; }
        //public Guid Team_Key { get; set; } is this needed?
    }
}