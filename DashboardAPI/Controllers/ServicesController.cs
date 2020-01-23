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
    public class ServicesController : ControllerBase
    {
        private readonly DataContext _context;

        public ServicesController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Services
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceModel>>> GetServices()
        {
            return await _context.Services.ToListAsync();
        }

        // GET: api/Services/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceModel>> GetServiceModel(int id)
        {
            var serviceModel = await _context.Services.FindAsync(id);

            if (serviceModel == null)
            {
                return NotFound();
            }

            return serviceModel;
        }

        // PUT: api/Services/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceModel(int id, ServiceModel serviceModel)
        {
            if (id != serviceModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(serviceModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceModelExists(id))
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

        // POST: api/Services
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ServiceModel>> PostServiceModel(ServiceModel serviceModel)
        {
            _context.Services.Add(serviceModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServiceModel", new { id = serviceModel.Id }, serviceModel);
        }

        // DELETE: api/Services/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceModel>> DeleteServiceModel(int id)
        {
            var serviceModel = await _context.Services.FindAsync(id);
            if (serviceModel == null)
            {
                return NotFound();
            }

            _context.Services.Remove(serviceModel);
            await _context.SaveChangesAsync();

            return serviceModel;
        }

        private bool ServiceModelExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
