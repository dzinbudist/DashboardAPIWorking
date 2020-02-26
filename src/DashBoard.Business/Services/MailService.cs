using System;
using System.Collections.Generic;
using System.Text;
using SendGrid;
using SendGrid.Helpers.Mail;
using DashBoard.Data.Entities;
using DashBoard.Data.Data;
using System.Threading.Tasks;
using System.Linq;

namespace DashBoard.Business.Services
{
    public interface IMailService
    {
        Task<bool> SendEmail(DomainModel domainModel, Guid teamKey);
    }
    public class MailService: IMailService
    {
        private readonly DataContext _context;
        public MailService(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> SendEmail(DomainModel domainModel, Guid teamKey)
        {
            //double intervalMultiplier;
            bool datePass = false;
            bool notifiedPass = true;

            //if (domainModel.Interval_Ms < 240000) //600000
            //{
            //    double intervalMultiplierDouble = 240000 / domainModel.Interval_Ms;
            //    intervalMultiplier = Math.Round(intervalMultiplierDouble, MidpointRounding.AwayFromZero);
            //}
            //else
            //{
            //    intervalMultiplier = 1;
            //}

            var blackoutTime = DateTime.Now.AddMinutes(-10);//DateTime.Now.AddMilliseconds(-(domainModel.Interval_Ms * intervalMultiplier));
            var logs = _context.Logs.Where(x => x.Domain_Id == domainModel.Id && x.Team_Key == teamKey && x.Log_Date >= blackoutTime).OrderBy(x => x.Log_Date).ToList();

            if (logs.Count > 0)
            {
                datePass = logs.First().Log_Date.AddSeconds(20) > blackoutTime;

                if (datePass)
                {
                    foreach (LogModel aModel in logs)
                    {
                        if (aModel.Notified)
                        {
                            notifiedPass = false;
                        }
                    }

                    if (notifiedPass)
                    {
                        var client = new SendGridClient("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");
                        var from = new EmailAddress("notify@watchhound.com", "Watch Hound");
                        var subject = "Watch Hound - Something wrong with  " + domainModel.Service_Name;
                        var to = new EmailAddress(domainModel.Notification_Email, "Watch Hound");
                        var plainTextContent = "Something wrong with  " + domainModel.Service_Name;
                        var htmlContent = "";//"<strong>and easy to do anywhere, even with C#</strong>";
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        var response = await client.SendEmailAsync(msg);

                        var aLog = logs.Last();

                        if (aLog != null)
                        {
                            aLog.Notified = true;
                            _context.Logs.Update(aLog);
                            _context.SaveChanges();
                        }
                    }
                }                
            }
            return true;
        }
    }
}
