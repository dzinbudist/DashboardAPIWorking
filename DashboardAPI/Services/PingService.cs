using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;
using System.Net.NetworkInformation;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IPingService
    {
        PingResponse PingDomainFromDB(int id);
    }

    public class PingService : IPingService
    {
        private DataContext _context;
        public PingService(DataContext context)
        {
            _context = context;
        }
        //cia noretusi grazint daugiau info, bet controleriui reik tik pingresponse
        public PingResponse PingDomainFromDB(int id)
        {
            var domainModel = _context.Domains.Find(id);
            if (domainModel == null)
            {
                return null;
            }
            string hostname = domainModel.Url;
            Ping domainPing = new Ping();
            try
            {
                var reply = domainPing.Send(hostname);
                if(reply.Status != IPStatus.Success)
                {
                    DateTime currentTime = DateTime.Now;
                    domainModel.Last_Fail = currentTime;
                    LogModel log_entry = new LogModel {Domain_Id = domainModel.Id, Log_Date = currentTime, Error_Text = reply.Status.ToString()};
                    _context.Add(log_entry);
                    _context.SaveChanges();
                }
                PingResponse serverResponse = new PingResponse
                {   
                    //pingas neturi status code
                    Url_Pinged = domainModel.Url,
                    Status = reply.Status.ToString(),
                    LatencyMS = reply.RoundtripTime
                };

                return serverResponse;
            }
            catch
            {
                return null;
            }
        }
    }

}
