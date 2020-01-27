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
        // POST: api/Services
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        // [Authorize]
        [HttpPost]
        public async Task<ActionResult<ServiceModel>> PostServiceModel(ServiceModel serviceModel)
        {
            int userId = MiscFunctions.GetCurentUser(this.User);

            serviceModel.Created_By = userId;
            serviceModel.Date_Created = DateTime.Now;
            serviceModel.Date_Created = DateTime.Now;
            _context.Services.Add(serviceModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServiceModel", new { id = serviceModel.Id }, serviceModel);
        }

        // PUT: api/Services/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceModel(int id, ServiceModel serviceModel)
        {   
            //This line is needed , so you don't have to write Id in request Body
            serviceModel.Id = id;
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

        // PUT for delete: api/Services/5
        [HttpPut("del/{id}")]
        public async Task<ActionResult<ServiceModel>> DeleteServiceModel(int id)
        {  
            var serviceModel = await _context.Services.FindAsync(id);
            if (serviceModel == null)
            {
                return NotFound();
            }

            serviceModel.Deleted = true;
            serviceModel.Date_Modified = DateTime.Now;
            await _context.SaveChangesAsync();

            return serviceModel;
        }

        private bool ServiceModelExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
