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

namespace DashBoard.Business.Services
{
    public interface IRequestService
    {
        Task<object> GetService(int id, DomainForCreationDto domain, string userId);
    }
    public class RequestsService: IRequestService
    {
        private readonly DataContext _context;
        private readonly IMailService _mailService;
        public RequestsService(DataContext context, IMailService mailService)
        {
            _context = context;
            _mailService = mailService;
        }


        public async Task<object> GetService(int id, DomainForCreationDto domain, string userId)
        {
            DomainModel domainModel;

            if (domain == null)
            {
                var user = await _context.Users.FindAsync(Convert.ToInt32(userId));
                var teamKey = user.Team_Key;
                domainModel = _context.Domains.FirstOrDefault(x => x.Id == id && x.Deleted == false && x.Team_Key == teamKey);
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
                    Random random = new Random(); // laikinas
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Basic", Convert.ToBase64String(
                                System.Text.ASCIIEncoding.ASCII.GetBytes(
                                   $"{domainModel.Auth_User}:{domainModel.Auth_Password}"))); //+random.Next(10000)

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
                    SaveLog(domainModel, response);
                }

                return GetResponseObject(domainModel, sw, response);
            }
            catch
            {
                if (domainModel.Id != -555 && domainModel.Service_Name != "ModelForTesting")
                {
                    SaveLogFailed(domainModel);
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

        private static object GetResponseObject(DomainModel domainModel, Stopwatch sw, HttpResponseMessage response)
        {
            return new
            {
                DomainUrl = domainModel.Url,
                Status = response.StatusCode,
                RequestTime = sw.ElapsedMilliseconds
            };
        }

        private void SaveLog(DomainModel domainModel, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                domainModel.Last_Fail = DateTime.Now;
                // new LogModel entity added to Database
                var logEntry = new LogModel
                {
                    Domain_Id = domainModel.Id,
                    Log_Date = DateTime.Now,
                    Error_Text = response.StatusCode.ToString(),
                    Team_Key = domainModel.Team_Key
                };
                _context.Logs.Add(logEntry);
                _context.SaveChanges();
            }
        }

        private void SaveLogFailed(DomainModel domainModel)
        {
            domainModel.Last_Fail = DateTime.Now;
            var logEntry = new LogModel
            {
                Domain_Id = domainModel.Id,
                Log_Date = DateTime.Now,
                Error_Text = "503",
                Team_Key = domainModel.Team_Key
            };
            _context.Logs.Add(logEntry);
            _context.SaveChanges();
        }

        private static DomainModel GetDomainModel(DomainForCreationDto domain)
        {
            DomainModel modelForTest = new DomainModel
            {
                Id = -555,
                Service_Name = "ModelForTesting",
                Service_Type = domain.Service_Type,
                Url = domain.Url,
                Method = domain.Method,
                Notification_Email = domain.Notification_Email,
                Basic_Auth = domain.Basic_Auth,
                Auth_User = domain.Auth_User,
                Auth_Password = domain.Auth_Password,
                Parameters = domain.Parameters,
                Active = domain.Active,
                Interval_Ms = 4000
            };

            return modelForTest;
        }
    }
}
