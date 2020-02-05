using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Business.Services;
using WebApi.Data.Data;
using WebApi.Data.Entities;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ILogsService _logsService;

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
            var result = _logsService.GetAllLogs();
            if (result == null)
            {
                return NotFound();
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
                return NotFound();
            }
            return Ok(result);
        }
    }
}