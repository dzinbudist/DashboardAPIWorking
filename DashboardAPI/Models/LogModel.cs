using System;

namespace WebApi.Models
{
    public class LogModel
    {
        public int Id { get; set; }
        public DateTime Log_Date { get; set; }
        public string Error_Code { get; set; }
        public string Error_Text { get; set; }
        public string Request_Type { get; set; }
        public int Request_Ref { get; set; }
    }
}