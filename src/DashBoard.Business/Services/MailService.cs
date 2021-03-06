﻿using System;
using System.Collections.Generic;
using System.Text;
using SendGrid;
using SendGrid.Helpers.Mail;
using DashBoard.Data.Entities;
using DashBoard.Data.Data;
using DashBoard.Business.Helpers;
using System.Threading.Tasks;
using System.Linq;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Options;

namespace DashBoard.Business.Services
{
    public interface IMailService
    {
        Task<bool> SendEmailNew(DomainModel domainModel, Guid teamKey, string responseCode);
        Task<bool> SendEmail(DomainModel domainModel, Guid teamKey, string responseCode);
        Task<bool> SendEmailJet(DomainModel domainModel, Guid teamKey, string responseCode);
    }
    public class MailService: IMailService
    {
        private readonly DataContext _context;
        private readonly AppMailSettings _mailSettings;
        public MailService(DataContext context, IOptions<AppMailSettings> mailSettings)
        {
            _context = context;
            _mailSettings = mailSettings.Value;
        }

        public async Task<bool> SendEmailNew(DomainModel domainModel, Guid teamKey, string responseCode)
        {
            string logList = "";

            if (domainModel.Last_Notified <= DateTime.Now.AddHours(-1))
            {
                var logs = _context.Logs.Where(x => x.Domain_Id == domainModel.Id && x.Team_Key == teamKey && x.Log_Date >= DateTime.Now.AddHours(-1)).OrderBy(x => x.Log_Date).ToList();

                if (logs.Count > 0)
                {
                    foreach (LogModel aModel in logs)
                    { 
                       logList = logList + $"<li>Log time: {aModel.Log_Date.ToString("yyyy-MM-dd HH:mm")}. Response: {aModel.Error_Text}</li><br>";    
                    }

                    if (IsValidEmail(domainModel.Notification_Email))
                    {
                        var client = new SendGridClient(_mailSettings.SendGridKey);
                        var from = new EmailAddress("notify@watchhound.com", "Watch Hound");
                        var subject = "Watch Hound - Something wrong with  " + domainModel.Service_Name;
                        var to = new EmailAddress(domainModel.Notification_Email, "Watch Hound");
                        var plainTextContent = "";//"Something wrong with  " + domainModel.Service_Name;
                        var htmlContent = "<span>A problem occurred with: </span>  <strong>" + domainModel.Service_Name + "</strong><br>" +
                                          "<span>Endpoint: </span><strong>" + domainModel.Url + "</strong><br>" + "<p>Last hour logs:  </p><ul>" + logList + "</ul>";
                        var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                        var response = await client.SendEmailAsync(msg);

                        if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
                        {
                            return true;
                        }
                    }
                }                
            }
            return false;
        }


        public async Task<bool> SendEmail(DomainModel domainModel, Guid teamKey, string responseCode)
        {
            bool datePass = false;
            bool notifiedPass = true;

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
                            var client = new SendGridClient("");
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
            bool datePass = false;
            bool notifiedPass = true;

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
                            MailjetClient client = new MailjetClient("")
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




