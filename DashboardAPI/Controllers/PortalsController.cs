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
    public class PortalsController : ControllerBase
    {
        private readonly DataContext _context;

        public PortalsController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Portals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PortalModel>>> GetPortals()
        {
            return await _context.Portals.ToListAsync();
        }

        // GET: api/Portals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PortalModel>> GetPortalModel(int id)
        {
            var portalModel = await _context.Portals.FindAsync(id);

            if (portalModel == null)
            {
                return NotFound();
            }

            return portalModel;
        }

        // PUT: api/Portals/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPortalModel(int id, PortalModel portalModel)
        {
            if (id != portalModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(portalModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PortalModelExists(id))
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

        // POST: api/Portals
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<PortalModel>> PostPortalModel(PortalModel portalModel)
        {
            _context.Portals.Add(portalModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPortalModel", new { id = portalModel.Id }, portalModel);
        }

        // DELETE: api/Portals/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<PortalModel>> DeletePortalModel(int id)
        {
            var portalModel = await _context.Portals.FindAsync(id);
            if (portalModel == null)
            {
                return NotFound();
            }

            _context.Portals.Remove(portalModel);
            await _context.SaveChangesAsync();

            return portalModel;
        }

        private bool PortalModelExists(int id)
        {
            return _context.Portals.Any(e => e.Id == id);
        }
    }
}
