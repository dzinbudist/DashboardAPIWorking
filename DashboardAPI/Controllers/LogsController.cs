using System.Collections.Generic;
using System.Threading.Tasks;
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

        [HttpGet]
        public IActionResult GetAllLogs()
        {
            var result = _logsService.GetAllLogs();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetLogsByDomain(int id)
        {
            var result = _logsService.GetLogsByDomainId(id);
            return Ok(result);
        }
    }
}