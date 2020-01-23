namespace WebApi.Models
{
    public class PortalModel
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Admin_Email { get; set; }
        public int Interval_Ms { get; set; }
        public bool Deleted { get; set; }
    }
}