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
        PingResponse PingServiceFromDB(int id);
        PingResponse PingPortalFromDB(int id);
    }

    public class PingService : IPingService
    {
        private DataContext _context;
        public PingService(DataContext context)
        {
            _context = context;
        }
        //cia noretusi grazint daugiau info, bet controleriui reik tik pingresponse
        public PingResponse PingServiceFromDB(int id)
        {
            var serviceModel = _context.Services.Find(id);
            if (serviceModel == null)
            {
                return null;
            }
            string hostname = serviceModel.Url;
            Ping servicePing = new Ping();
            try
            {
                var reply = servicePing.Send(hostname);
                if(reply.Status != IPStatus.Success)
                {
                    DateTime currentTime = DateTime.Now;
                    serviceModel.Last_Fail = currentTime;
                    LogModel log_entry = new LogModel { Request_Type = "service", Request_Ref = serviceModel.Id, Log_Date = currentTime, Error_Text = reply.Status.ToString()};
                    _context.Add(log_entry);
                    _context.SaveChanges();
                }
                PingResponse serverResponse = new PingResponse
                {
                    Url_Pinged = serviceModel.Url,
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

        //Ne DRY, turbut galima su enums, sumazinti sita koda.
        public PingResponse PingPortalFromDB(int id)
        {
            var portalModel = _context.Portals.Find(id);
            if (portalModel == null)
            {
                return null;
            }
            string hostname = portalModel.Url;
            Ping servicePing = new Ping();
            try
            {
                var reply = servicePing.Send(hostname);
                if (reply.Status != IPStatus.Success)
                {
                    DateTime currentTime = DateTime.Now;
                    portalModel.Last_Fail = currentTime;
                    LogModel log_entry = new LogModel { Request_Type = "service", Request_Ref = portalModel.Id, Log_Date = currentTime, Error_Text = reply.Status.ToString() };
                    _context.Add(log_entry);
                    _context.SaveChanges();
                }
                PingResponse serverResponse = new PingResponse
                {
                    Url_Pinged = portalModel.Url,
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
