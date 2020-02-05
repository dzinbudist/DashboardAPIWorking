using DashBoard.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace DashBoard.Web.Controllers
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

