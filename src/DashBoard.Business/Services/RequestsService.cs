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
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(domainModel.Url);
                client.DefaultRequestHeaders.Accept.Clear();

                var sw = new Stopwatch();
                sw.Start();
                HttpResponseMessage response = await client.GetAsync("");
                sw.Stop();
                if (response.IsSuccessStatusCode)
                {
                    return new
                    {
                        DomainUrl = domainModel.Url,
                        Status = response.StatusCode,
                        RequestTime = sw.ElapsedMilliseconds
                    };
                }
            }
            return null;
        }

        public async Task<object> GetService(int id)
        {
            var domainModel = _context.Domains.Find(id);

            if (domainModel != null)            
            {
               
                if (domainModel.Service_Type == ServiceType.ServiceRest)
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri(domainModel.Url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    if (domainModel.Method == RequestMethod.Get)
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "");
                        return await DoRestRequest(client, request, domainModel);
                    }
                    else if (domainModel.Method == RequestMethod.Post)
                    {
                        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
                        return await DoRestRequest(client, request, domainModel);
                    }                    
                } 
                else if (domainModel.Service_Type == ServiceType.ServiceSoap)
                {

                }


                //await client.SendAsync(request)
                //      .ContinueWith(responseTask =>
                //      {
                //          Console.WriteLine("Response: {0}", responseTask.Result);
                //      });
            }
            return null;
        }

        async Task<object> DoRestRequest(HttpClient client, HttpRequestMessage request, DomainModel domainModel)
        {
            if (domainModel.Basic_Auth)
            {
                //http://www.httpwatch.com/httpgallery/authentication/authenticatedimage/default.aspx?0.8738778301275651
                //httpwatch

                Random random = new Random();
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue(
                        "Basic", Convert.ToBase64String(
                            System.Text.ASCIIEncoding.ASCII.GetBytes(
                               $"{domainModel.Auth_User}:{domainModel.Auth_Password + random.Next(10000)}")));            }
            else
            {


            }         


            var aJSON = new
            {
                username = "test",
                password = "1234"
            };

            string jsonString = JsonSerializer.Serialize(aJSON);            
            request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var sw = new Stopwatch();
            sw.Start();
            var response = await client.SendAsync(request);
            sw.Stop();

            if (response.IsSuccessStatusCode)
            {
                return new
                {
                    DomainUrl = domainModel.Url,
                    Status = response.StatusCode,
                    RequestTime = sw.ElapsedMilliseconds
                };
            }

            return null;
        }

    }
}
