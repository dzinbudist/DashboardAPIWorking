using DashBoard.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DashBoard.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    
    public class LogsController : ControllerBase
    {
        private readonly ILogsService _logsService;
        public string LoggedInUser => User.Identity.Name; //this gets current user ID. We need to pass it to services in parameters. This is not ideal, couldn't find a way to get user in controller.


        public LogsController(ILogsService logsService)
        {
            _logsService = logsService;
        }

        //Because there are multiple return types and paths in this type of action, liberal use of the[ProducesResponseType] attribute is necessary.
        [HttpGet]
       [ProducesResponseType(StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllLogs()
        {
            var userId = LoggedInUser;
            var result = _logsService.GetAllLogs(userId);
            if (result == null)
            {
                return NotFound(); //NoContent() is also an option here.
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetLogsByDomain(int id)
        {
            var result = _logsService.GetLogsByDomainId(id);
            if (result == null)
            {
                return NotFound(); //NoContent() is also an option here.
            }
            return Ok(result);
        }
    }
}