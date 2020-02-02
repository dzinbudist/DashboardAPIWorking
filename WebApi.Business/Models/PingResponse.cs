namespace WebApi.Business.Models
{
    public class PingResponse
    {   
        public string Url_Pinged { get; set; }
        public string Status { get; set; }
        public long LatencyMS { get; set; }
    }
}
