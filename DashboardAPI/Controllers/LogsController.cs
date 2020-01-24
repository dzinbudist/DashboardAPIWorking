using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly DataContext _context;

        public LogsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Logs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogModel>>> GetLogs()
        {
            return await _context.Logs.ToListAsync();
        }

        // GET: api/Logs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LogModel>> GetLogModel(int id)
        {
            var logModel = await _context.Logs.FindAsync(id);

            if (logModel == null)
            {
                return NotFound();
            }

            return logModel;
        }

        // POST: api/Logs
        //Work to do here later after Front End sends error.
        [HttpPost]
        public async Task<ActionResult<LogModel>> PostLogModel(LogModel logModel)
        {

            logModel.Log_Date = DateTime.Now;
            _context.Logs.Add(logModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLogModel", new { id = logModel.Id }, logModel);
        }

    }
}
