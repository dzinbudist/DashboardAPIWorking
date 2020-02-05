using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DashBoard.Data.Data;

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
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(domainModel.Url);
               // client.BaseAddress = new Uri("http://40.85.76.116/api/users/authenticate"); //http://40.85.76.116/api/users/authenticate
                client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(
                //new MediaTypeWithQualityHeaderValue("application/json"));


                var aJSON = new
                {
                    username = "test",
			        password = "1234"
                };

                string jsonString;
                jsonString = JsonSerializer.Serialize(aJSON);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "");
                request.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                //await client.SendAsync(request)
                //      .ContinueWith(responseTask =>
                //      {
                //          Console.WriteLine("Response: {0}", responseTask.Result);
                //      });
                



                var sw = new Stopwatch();
                sw.Start();
                //HttpResponseMessage response = await client.GetAsync("");
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
            }
            return null;
        }

    }
}
