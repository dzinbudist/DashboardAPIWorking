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

        [HttpPost]
        public async Task<ActionResult<ServiceModel>> PostServiceModel(ServiceModel serviceModel)
        {

            //serviceModel.Created_By = MiscFunctions.GetCurentUser(this.User);
            serviceModel.Date_Created = DateTime.Now;


            _context.Services.Add(serviceModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServiceModel", new { id = serviceModel.Id }, serviceModel);
        }

        // PUT: api/Services/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<ActionResult<ServiceModel>> PutServiceModel(int id, ServiceModel serviceModel)
        {

            var updatedModel =await _context.Services.FindAsync(id);

            //updatedModel.Modified_By = MiscFunctions.GetCurentUser(this.User);
            updatedModel.Date_Modified = DateTime.Now;
            _context.Entry(updatedModel).State = EntityState.Modified; //ar reikia sitos eilutes?
            _context.Services.Update(updatedModel);
            _context.SaveChanges();
            return updatedModel;
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

            //serviceModel.Modified_By = MiscFunctions.GetCurentUser(this.User);
            serviceModel.Date_Modified = DateTime.Now;
            serviceModel.Deleted = true;
            await _context.SaveChangesAsync();

            return serviceModel;
        }
    }
}
