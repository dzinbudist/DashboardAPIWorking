using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Business.Services;
using WebApi.Data.Entities;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly IPingService _pingService;
        public PingController(IPingService pingService)
        {
            _pingService = pingService;
        }
        [HttpGet("domain/{id}")]
        //Veliau paziureti, ar imanoma kitaip grazinti errorus/exceptionus is servico.
        public ActionResult<object> PingService(int id)
        {
            var response = _pingService.PingDomainFromDb(id); //cia ne DTO bet complex type custom object
            if (response == null)
            {
                return NotFound(new { message = $"Problem pinging service with id {id}" });
            }
            return response;
        }
    }
}

