using System;

namespace DashBoard.Business.DTOs.Logs
{
    public class LogModelDto
    {
        public int Id { get; set; }
        public int Domain_Id { get; set; }
        public DateTime Log_Date { get; set; }
        public string Error_Text { get; set; }
        public string Service_Name { get; set; }

    }
}
