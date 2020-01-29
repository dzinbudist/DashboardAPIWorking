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
        [HttpGet("domain/{id}")]
        //Veliau paziureti, ar imanoma kitaip grazinti errorus/exceptionus is servico.
        public ActionResult<PingResponse> PingService(int id)
        {
            var response = _pingService.PingDomainFromDB(id);
            if (response == null)
            {
                return NotFound(new { message = $"Problem pinging service with id {id}" });
            }
            return response;
        }
    }
}

