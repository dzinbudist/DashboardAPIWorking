using System;

namespace WebApi.Models
{
    public class ServiceModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Admin_Email { get; set; }
        public int Interval_Ms { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }
        public DateTime Date_Created { get; set; }
        public DateTime Date_Modified { get; set; }
        public int Created_By { get; set; }
        public int Modified_By { get; set; }
        public DateTime Last_Fail { get; set; }
    }
}
