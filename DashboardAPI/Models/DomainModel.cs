using System;

namespace WebApi.Models
{
    public class DomainModel
    {
        public int Id { get; set; }
        public string Service_Name { get; set; }
        public string Url { get; set; }
        public string Service_Type { get; set; }
        public string Method { get; set; }
        public bool Basic_Auth { get; set; }
        public string Auth_User { get; set; }
        public string Auth_Password { get; set; } //pagalvoti kaip saugoti PW
        public string Parameters { get; set; }
        public string Notification_Email { get; set; }
        public int Interval_Ms { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public int Created_By { get; set; }
        public int Modified_By { get; set; }
        public DateTime Date_Created { get; set; }
        public DateTime Date_Modified { get; set; }
        public DateTime Last_Fail { get; set; }
    }
}
