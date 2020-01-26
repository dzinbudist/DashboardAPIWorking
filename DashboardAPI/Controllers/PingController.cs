using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private IPingService _pingService;
        public PingController(IPingService pingService)
        {
            _pingService = pingService;
        }
        [HttpGet("service/{id}")]
        //Veliau paziureti, ar imanoma kitaip grazinti errorus/exceptionus is servico.
        public ActionResult<PingResponse> PingService(int id)
        {
            var response = _pingService.PingServiceFromDB(id);
            if (response == null)
            {
                return NotFound($"Problem pinging service with id {id}");
            }
            return response;
        }
        [HttpGet("portal/{id}")]
        //Veliau paziureti, ar imanoma kitaip grazinti errorus/exceptionus is servico.
        public ActionResult<PingResponse> PingPortal(int id)
        {
            var response = _pingService.PingPortalFromDB(id);
            if (response == null)
            {
                return NotFound($"Problem pinging portal with id {id}");
            }
            return response;
        }

    }
}

