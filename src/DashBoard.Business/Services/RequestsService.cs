using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DashBoard.Data.Data;
using DashBoard.Data.Enums;
using DashBoard.Data.Entities;
using System.Net.Http.Headers;
using DashBoard.Business.DTOs.Domains;
using System.Linq;
using System.Dynamic;

namespace DashBoard.Business.Services
{
    public interface IRequestService
    {
        Task<object> GetService(int id, DomainForTestDto domain, string userId, string requestType = "default", DomainModel domainBackground = null);
    }
    public class RequestsService: IRequestService
    {
        private readonly DataContext _context;
        private readonly IMailService _mailService;
        private Guid teamKey;
        public RequestsService(DataContext context, IMailService mailService)
        {
            _context = context;
            _mailService = mailService;
        }

        public async Task<object> GetService(int id, DomainForTestDto domain, string userId, string requestType = "default", DomainModel domainBackground = null)
        {
            DomainModel domainModel;

            if (domain == null && requestType == "default")
            {
                var user = await _context.Users.FindAsync(Convert.ToInt32(userId));
                teamKey = user.Team_Key;
                domainModel = _context.Domains.FirstOrDefault(x => x.Id == id && x.Deleted == false && x.Team_Key == teamKey);
            }
            else if (domainBackground != null && requestType == "background")
            {
                domainModel = domainBackground;
            }
            else
            {    
                domainModel = GetDomainModel(domain);
            }

            if (domainModel != null)            
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(domainModel.Url);
                client.DefaultRequestHeaders.Accept.Clear();

                if (domainModel.Service_Type == ServiceType.ServiceRest)
                {

                    if (domainModel.Method == RequestMethod.Get)
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "");
                        return await DoRequest(client, request, domainModel, "application/json");
                    }
                    else if (domainModel.Method == RequestMethod.Post)
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
                        return await DoRequest(client, request, domainModel, "application/json");
                    }                    
                } 
                else if (domainModel.Service_Type == ServiceType.ServiceSoap)
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
                    return await DoRequest(client, request, domainModel, "text/xml");
                }

            }
            return null;
        }

        async Task<object> DoRequest(HttpClient client, HttpRequestMessage request, DomainModel domainModel, string mediaType)
        {
            try
            {
                if (domainModel.Basic_Auth)
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Basic", Convert.ToBase64String(
                                System.Text.ASCIIEncoding.ASCII.GetBytes(
                                   $"{domainModel.Auth_User}:{domainModel.Auth_Password}")));

                    if (domainModel.Parameters != null && request.Method != HttpMethod.Get)
                    {
                        var Json = domainModel.Parameters;
                        request.Content = new StringContent(Json, Encoding.UTF8, mediaType);
                    }
                }
                else
                {
                    if (domainModel.Parameters != null && request.Method != HttpMethod.Get)
                    {
                        var Json = domainModel.Parameters;
                        request.Content = new StringContent(Json, Encoding.UTF8, mediaType);
                    }
                }

                var sw = new Stopwatch();
                sw.Start();
                var response = await client.SendAsync(request);
                sw.Stop();

                if (domainModel.Id != -555 && domainModel.Service_Name != "ModelForTesting")
                {                    
                    await SaveLog(domainModel, response);
                }

                var data = response.ToString();
                var pageContents = await response.Content.ReadAsStringAsync();

                return GetResponseObject(domainModel, sw, response, pageContents);
            }
            catch
            {
                if (domainModel.Id != -555 && domainModel.Service_Name != "ModelForTesting")
                {
                    await SaveLogFailed(domainModel);
                }
                
                return GetFailObject(domainModel);
            }
        }

        private static object GetFailObject(DomainModel domainModel)
        {
            return new
            {
                DomainUrl = domainModel.Url,
                Status = "Failed" // add reason
            };
        }

        private static object GetResponseObject(DomainModel domainModel, Stopwatch sw, HttpResponseMessage response, string dataText)
        {
            return new
            {
                DomainUrl = domainModel.Url,
                Status = response.StatusCode,
                RequestTime = sw.ElapsedMilliseconds,
                Response = dataText
            };
        }

        private async Task<bool> SaveLog(DomainModel domainModel, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {                
                // new LogModel entity added to Database
                var logEntry = new LogModel
                {
                    Domain_Id = domainModel.Id,
                    Log_Date = DateTime.Now,
                    Error_Text = response.StatusCode.ToString(),
                    Team_Key = domainModel.Team_Key,
                    Service_Name = domainModel.Service_Name
                };
                _context.Logs.Add(logEntry);
                _context.SaveChanges();

                var result = await _mailService.SendEmailNew(domainModel, domainModel.Team_Key, response.StatusCode.ToString());
                domainModel.Last_Fail = DateTime.Now.AddHours(2);
                if (result)
                {
                    domainModel.Last_Notified = DateTime.Now;
                }                
                _context.Domains.Update(domainModel);
                _context.SaveChanges();
                return result;
            }
            return false;
        }

        private async Task<bool> SaveLogFailed(DomainModel domainModel)
        {            
            var logEntry = new LogModel
            {
                Domain_Id = domainModel.Id,
                Log_Date = DateTime.Now,
                Error_Text = "503",
                Team_Key = domainModel.Team_Key,
                Service_Name = domainModel.Service_Name
            };
            _context.Logs.Add(logEntry);
            _context.SaveChanges();

            var result = await _mailService.SendEmailNew(domainModel, domainModel.Team_Key, "503");
            domainModel.Last_Fail = DateTime.Now.AddHours(2);
            if (result)
            {
                domainModel.Last_Notified = DateTime.Now;
            }
            _context.Domains.Update(domainModel);
            _context.SaveChanges();
            return result;
        }

        private static DomainModel GetDomainModel(DomainForTestDto domain)
        {
            DomainModel modelForTest = new DomainModel
            {
                Id = -555,
                Service_Name = "ModelForTesting",
                Service_Type = domain.Service_Type,
                Url = domain.Url,
                Method = domain.Method,
                Notification_Email = "test@fortest",
                Basic_Auth = domain.Basic_Auth,
                Auth_User = domain.Auth_User,
                Auth_Password = domain.Auth_Password,
                Parameters = domain.Parameters,
                Active = true,
                Interval_Ms = 4000
            };

            return modelForTest;
        }
    }
}
