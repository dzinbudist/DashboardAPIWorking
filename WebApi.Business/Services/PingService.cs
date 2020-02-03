using System;
using System.Net.NetworkInformation;
using WebApi.Data.Data;
using WebApi.Data.Entities;

namespace WebApi.Business.Services
{
    public interface IPingService
    {
        object PingDomainFromDb(int id);
    }

    public class PingService : IPingService
    {
        private readonly DataContext _context;
        public PingService(DataContext context)
        {
            _context = context;
        }

        public object PingDomainFromDb(int id)
        {
            var domainModel = _context.Domains.Find(id);
            if (domainModel == null)
            {
                return null;
            }
            var hostname = domainModel.Url;
            var domainPing = new Ping();
            try
            {
                var reply = domainPing.Send(hostname);
                if (reply != null && reply.Status == IPStatus.Success)
                {
                    var response = new
                    {
                        Url_Pinged = domainModel.Url,
                        Status = reply.Status.ToString(),
                        LatencyMS = reply.RoundtripTime
                    };
                    domainModel.Last_Fail = DateTime.Now;
                    // new LogModel entity added to Database
                    var logEntry = new LogModel
                    {
                        Domain_Id = domainModel.Id,
                        Log_Date = DateTime.Now,
                        Error_Text = reply.Status.ToString()
                    };
                    _context.Logs.Add(logEntry);
                    _context.SaveChanges();

                    return response;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
    }

}