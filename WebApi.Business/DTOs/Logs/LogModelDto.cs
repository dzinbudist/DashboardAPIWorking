using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Business.DTOs.Logs
{
    public class LogModelDto
    {
        public int Id { get; set; }
        public int Domain_Id { get; set; }
        public DateTime Log_Date { get; set; }
        public string Error_Text { get; set; }
    }
}
