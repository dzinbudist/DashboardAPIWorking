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
    public class DomainController : ControllerBase
    {
        private readonly DataContext _context;
        public DomainController(DataContext context)
        {
            _context = context;
        }

        // GET: api/Domains
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DomainModel>>> GetDomains()
        {
            return await _context.Domains.ToListAsync();
        }

        // GET: api/Domains/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DomainModel>> GetDomainModel(int id)
        {
            var DomainModel = await _context.Domains.FindAsync(id);

            if (DomainModel == null)
            {
                return NotFound();
            }

            return DomainModel;
        }
        // POST: api/Domains
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.

        [HttpPost]
        public async Task<ActionResult<DomainModel>> PostDomainModel(DomainModel DomainModel)
        {

            //DomainModel.Created_By = MiscFunctions.GetCurentUser(this.User);
            DomainModel.Date_Created = DateTime.Now;

            _context.Domains.Add(DomainModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDomainModel", new { id = DomainModel.Id }, DomainModel);
        }

        // PUT: api/Domains/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<ActionResult<DomainModel>> PutDomainModel(int id, DomainModel DomainModel)
        {

            var updatedModel =await _context.Domains.FindAsync(id);

            //updatedModel.Modified_By = MiscFunctions.GetCurentUser(this.User);
            updatedModel.Date_Modified = DateTime.Now;
            _context.Entry(updatedModel).State = EntityState.Modified; //ar reikia sitos eilutes?
            _context.Domains.Update(updatedModel);
            _context.SaveChanges();
            return updatedModel;
        }

        // PUT for delete: api/Domains/5
        [HttpPut("del/{id}")]
        public async Task<ActionResult<DomainModel>> DeleteDomainModel(int id)
        {  
            var DomainModel = await _context.Domains.FindAsync(id);
            if (DomainModel == null)
            {
                return NotFound();
            }

            //DomainModel.Modified_By = MiscFunctions.GetCurentUser(this.User);
            DomainModel.Date_Modified = DateTime.Now;
            DomainModel.Deleted = true;
            await _context.SaveChangesAsync();

            return DomainModel;
        }
    }
}
