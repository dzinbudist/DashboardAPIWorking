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

namespace DashBoard.Business.Services
{
    public interface IRequestService
    {
        Task<object> GetDomainByUrl(int id);
        Task<object> GetService(int id);
    }
    public class RequestsService: IRequestService
    {
        private readonly DataContext _context;
        public RequestsService(DataContext context)
        {
            _context = context;
        }

        public async Task<object> GetDomainByUrl(int id)
        {
            var domainModel = _context.Domains.Find(id);

            if (domainModel != null)
            {
                try
                {
                    if (domainModel.Service_Type == ServiceType.WebApp)
                    { 
                        HttpClient client = new HttpClient();
                        client.BaseAddress = new Uri(domainModel.Url);
                        client.DefaultRequestHeaders.Accept.Clear();

                        //do request and count response time
                        var sw = new Stopwatch();
                        sw.Start();
                        HttpResponseMessage response = await client.GetAsync("");
                        sw.Stop();

                        SaveLog(domainModel, response);

                        return GetResponseObject(domainModel, sw, response);
                    }
                    else
                    {
                        return null;
                    }
                }
                catch
                {
                    return GetFailObject(domainModel);
                }
            }
            return null;
        }

        public async Task<object> GetService(int id)
        {
            var domainModel = _context.Domains.Find(id);

            if (domainModel != null)            
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(domainModel.Url);
                client.DefaultRequestHeaders.Accept.Clear();

                if (domainModel.Service_Type == ServiceType.ServiceRest || domainModel.Service_Type == ServiceType.WebApp)
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

                SaveLog(domainModel, response);
                return GetResponseObject(domainModel, sw, response);
            }
            catch
            {
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
                    Error_Text = response.StatusCode.ToString()
                };
                _context.Logs.Add(logEntry);
                _context.SaveChanges();
            }
        }
    }
}
