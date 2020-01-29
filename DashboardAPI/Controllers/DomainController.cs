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

        // GET: api/Domain
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DomainModel>>> GetDomains()
        {
            return await _context.Domains.ToListAsync();
        }

        // GET: api/Domain/5
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
        // POST: api/Domain
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.

        [HttpPost]
        public async Task<ActionResult<DomainModel>> PostDomainModel(DomainModel domainModel)
        {
            if (ModelState.IsValid) //validacija backendo. patikrina ar yra visi required fieldai modeli.
            {

                DomainModel newModel = domainModel;
                newModel.Created_By = default; // dbr default poto:
                //newModel.Created_By = MiscFunctions.GetCurentUser(this.User);
                newModel.Modified_By = default;
                newModel.Last_Fail = default;
                newModel.Date_Created = DateTime.Now;
                newModel.Date_Modified = DateTime.Now;
                
                _context.Domains.Add(newModel);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetDomainModel", new { id = domainModel.Id }, domainModel);
            }
            else
            {
                return BadRequest();
            }
        }

        // PUT: api/Domain/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<ActionResult<DomainModel>> EditDomainModel(int id, DomainModel domainModel)
        {

            if (ModelState.IsValid)
            {
                var updatedModel = await _context.Domains.FindAsync(id);
                //galima su automapper nugetu graziai surasyti sintaxe.
                if (updatedModel.Service_Name != domainModel.Service_Name)
                {
                    updatedModel.Service_Name = domainModel.Service_Name;
                }
                if (updatedModel.Url != domainModel.Url)
                {
                    updatedModel.Url = domainModel.Url;
                }
                if (updatedModel.Service_Type != domainModel.Service_Type)
                {
                    updatedModel.Service_Type = domainModel.Service_Type;
                }
                if (updatedModel.Method != domainModel.Method)
                {
                    updatedModel.Method = domainModel.Method;
                }
                if (updatedModel.Basic_Auth != domainModel.Basic_Auth)
                {
                    updatedModel.Basic_Auth = domainModel.Basic_Auth;
                    if (updatedModel.Basic_Auth == false)
                    {
                        updatedModel.Auth_User = null;
                        updatedModel.Auth_Password = null;
                    }
                    updatedModel.Auth_User = domainModel.Auth_User;
                    updatedModel.Auth_Password = domainModel.Auth_Password;
                }
                if (updatedModel.Parameters != domainModel.Parameters)
                {
                    updatedModel.Parameters = domainModel.Parameters;
                }
                if (updatedModel.Notification_Email != domainModel.Notification_Email)
                {
                    updatedModel.Notification_Email = domainModel.Notification_Email;
                }
                if (updatedModel.Interval_Ms != domainModel.Interval_Ms)
                {
                    updatedModel.Interval_Ms = domainModel.Interval_Ms;
                }
                if (updatedModel.Active != domainModel.Active)
                {
                    updatedModel.Active = domainModel.Active;
                }
                //updatedModel.Modified_By = MiscFunctions.GetCurentUser(this.User);    
                updatedModel.Date_Modified = DateTime.Now;

                _context.Entry(updatedModel).State = EntityState.Modified; //ar reikia sitos eilutes?
                _context.Domains.Update(updatedModel);
                _context.SaveChanges();
                return updatedModel;
            }
            return BadRequest();
        }

        // PUT for delete: api/Domain/5
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
