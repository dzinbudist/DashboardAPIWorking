using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly DataContext _context;
        public PingController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("service/{id}")]
        public async Task<ActionResult<PingResponse>> PingService(int id)
        {
            var serviceModel = await _context.Services.FindAsync(id);

            if (serviceModel == null)
            {
                return NotFound($"Service with id: {id} was not found in DB");
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
            //Cia jei kazkas negerai su Ping, grazina 404 su exceptionu. Pakeisti veliau turbut.
            catch(Exception e)
            {
                return NotFound($"Tried pinging: {hostname} \n {e}");
            }
        }

        [HttpGet("portal/{id}")]
        public async Task<ActionResult<PingResponse>> PingPortal(int id)
        {
            var portalModel = await _context.Portals.FindAsync(id);

            if (portalModel == null)
            {
                return NotFound($"Portal with id: {id} was not found in DB");
            }
            string hostname = portalModel.Url;

            Ping portalPing = new Ping();
            try
            {
                var reply = portalPing.Send(hostname);
                PingResponse serverResponse = new PingResponse
                {
                    Url_Pinged = portalModel.Url,
                    Status = reply.Status.ToString(),
                    LatencyMS = reply.RoundtripTime
                };

                return serverResponse;
            }
            //Cia jei kazkas negerai su Ping, grazina 404 su exceptionu. Pakeisti veliau turbut.
            catch (Exception e)
            {
                return NotFound($"Tried pinging: {hostname} \n {e}");
            }
        }
    }
}

