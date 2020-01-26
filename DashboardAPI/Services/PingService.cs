using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;
using System.Net.NetworkInformation;


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
            var serviceModel = _context.Portals.Find(id);
            if (serviceModel == null)
            {
                return null;
            }
            string hostname = serviceModel.Url;
            Ping servicePing = new Ping();
            try
            {
                var reply = servicePing.Send(hostname);
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
    }

}
