using System;
using System.Collections.Generic;
using System.Text;
using SendGrid;
using SendGrid.Helpers.Mail;
using DashBoard.Data.Entities;
using DashBoard.Data.Data;
using System.Threading.Tasks;
using System.Linq;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;

namespace DashBoard.Business.Services
{
    public interface IMailService
    {
        Task<bool> SendEmail(DomainModel domainModel, Guid teamKey, string responseCode);
        Task<bool> SendEmailJet(DomainModel domainModel, Guid teamKey, string responseCode);
    }
    public class MailService: IMailService
    {
        private readonly DataContext _context;
        public MailService(DataContext context)
        {
            _context = context;
        }
        public async Task<bool> SendEmail(DomainModel domainModel, Guid teamKey, string responseCode)
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
                datePass = logs.First().Log_Date.AddSeconds(-30) <= blackoutTime;

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
                        if (IsValidEmail(domainModel.Notification_Email))
                        {
                            var client = new SendGridClient("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");
                            var from = new EmailAddress("notify@watchhound.com", "Watch Hound");
                            var subject = "Watch Hound - Something wrong with  " + domainModel.Service_Name;
                            var to = new EmailAddress(domainModel.Notification_Email, "Watch Hound");
                            var plainTextContent = "";//"Something wrong with  " + domainModel.Service_Name;
                            var htmlContent = "<p>Something wrong with</p>  <strong>" + domainModel.Service_Name + "</strong><br>" +
                                              "<p>Endpoint:  </p><strong>" + domainModel.Url + "</strong><br>" + "<p>Error code:  </p><strong>" + responseCode + "</strong>";
                            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                            var response = await client.SendEmailAsync(msg);

                            var aLog = logs.Last();

                            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                            {
                                if (aLog != null)
                                {
                                    aLog.Notified = true;
                                    _context.Logs.Update(aLog);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                }                
            }
            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }


        public async Task<bool> SendEmailJet(DomainModel domainModel, Guid teamKey, string responseCode)
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

            var blackoutTime = DateTime.Now.AddMinutes(-5);//DateTime.Now.AddMilliseconds(-(domainModel.Interval_Ms * intervalMultiplier));
            var logs = _context.Logs.Where(x => x.Domain_Id == domainModel.Id && x.Team_Key == teamKey && x.Log_Date >= blackoutTime).OrderBy(x => x.Log_Date).ToList();

            if (logs.Count > 0)
            {
                datePass = logs.First().Log_Date.AddSeconds(-30) <= blackoutTime;

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
                        if (IsValidEmail(domainModel.Notification_Email))
                        {
                            MailjetClient client = new MailjetClient("a73620d74b8a7717f2ff320bcf7c47b0", "968e06d73b9d95c5ad79bdfee153051b")
                            {
                                Version = ApiVersion.V3_1,
                            };
                            MailjetRequest request = new MailjetRequest
                            {
                                Resource = Send.Resource,
                            }
                             .Property(Send.Messages, new JArray {
                                 new JObject {
                                  {
                                   "From",
                                    new JObject 
                                    {
                                        {"Email", "andrius.vr@gmail.com"},
                                        {"Name", "WatchHound"}
                                    }
                                  }, {
                                    "To",
                                    new JArray 
                                    {
                                        new JObject 
                                        {
                                             {
                                              "Email",
                                              domainModel.Notification_Email
                                             }, {
                                              "Name",
                                              ""
                                             }
                                         }
                                   }
                                  }, 
                                     {
                                       "Subject",
                                       "Watch Hound - Something wrong with  " + domainModel.Service_Name
                                  }, 
                                     {
                                       "TextPart",
                                       ""
                                  }, 
                                     {
                                       "HTMLPart",
                                       "<p>Something wrong with</p>  <strong>" + domainModel.Service_Name + "</strong><br>" +
                                              "<p>Endpoint:  </p><strong>" + domainModel.Url + "</strong><br>" + "<p>Error code:  </p><strong>" + responseCode + "</strong>"
                        }, 
                                     {
                                       "CustomID",
                                       "AppGettingStartedTest"
                                  }
                                 }
                                   });                            

                            MailjetResponse response = await client.PostAsync(request);

                            if (response.IsSuccessStatusCode)
                            {
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
                }
            }
            return true;
        }

    }
}




