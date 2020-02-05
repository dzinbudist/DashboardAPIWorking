using System;

namespace DashBoard.Data.Entities
{
    public class LogModel
    {
        public int Id { get; set; }
        public int Domain_Id { get; set; }
        public DateTime Log_Date { get; set; }
        public string Error_Text { get; set; }
    }
}