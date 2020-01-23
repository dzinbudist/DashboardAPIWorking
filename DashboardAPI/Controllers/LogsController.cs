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
    [Route("api/[controller]")]
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

        // PUT: api/Logs/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogModel(int id, LogModel logModel)
        {
            if (id != logModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(logModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Logs
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<LogModel>> PostLogModel(LogModel logModel)
        {
            _context.Logs.Add(logModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLogModel", new { id = logModel.Id }, logModel);
        }

        // DELETE: api/Logs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<LogModel>> DeleteLogModel(int id)
        {
            var logModel = await _context.Logs.FindAsync(id);
            if (logModel == null)
            {
                return NotFound();
            }

            _context.Logs.Remove(logModel);
            await _context.SaveChangesAsync();

            return logModel;
        }

        private bool LogModelExists(int id)
        {
            return _context.Logs.Any(e => e.Id == id);
        }
    }
}
