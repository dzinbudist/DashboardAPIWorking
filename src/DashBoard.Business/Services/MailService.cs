using System;
using System.Collections.Generic;
using System.Text;
using SendGrid;
using SendGrid.Helpers.Mail;
using DashBoard.Data.Entities;
using DashBoard.Data.Data;
using System.Linq;

namespace DashBoard.Business.Services
{
    public interface IMailService
    {
        void SendEmail(DomainModel domainModel, Guid teamKey);
    }
    public class MailService: IMailService
    {
        private readonly DataContext _context;
        public MailService(DataContext context)
        {
            _context = context;
        }
        public async void SendEmail(DomainModel domainModel, Guid teamKey)
        {
            double intervalMultiplier;
            bool datePass = false;

            if (domainModel.Interval_Ms < 600000)
            {
                double intervalMultiplierDouble = 600000 / domainModel.Interval_Ms;
                intervalMultiplier = Math.Round(intervalMultiplierDouble, MidpointRounding.AwayFromZero);
            }
            else
            {
                intervalMultiplier = 1;
            }
             
            var blackoutTime = DateTime.Now.AddMilliseconds(-(domainModel.Interval_Ms * intervalMultiplier));
            var logs = _context.Logs.Where(x => x.Domain_Id == domainModel.Id && x.Team_Key == teamKey && x.Log_Date >= blackoutTime).OrderBy(x => x.Log_Date).ToList();

            if (logs.Count > 0)
            {
                datePass = logs.First().Log_Date.AddSeconds(-10) < blackoutTime;

                if (datePass)
                {

                }
            }

            //var logs = _context.Logs.Where(x => x.Domain_Id == 79).OrderBy(x => x.Log_Date).ToList();

            var client = new SendGridClient("");
            var from = new EmailAddress("test@example.com", "Example User");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("dzinbudist@gmail.com", "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
