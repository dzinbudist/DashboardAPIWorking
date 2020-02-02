using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Business.Services;
using WebApi.Data.Entities;
using WebApi.Helpers;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DomainController : ControllerBase
    {
        private IDomainService _domainService;
        public DomainController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        // GET: api/Domain
        [HttpGet]
        public IActionResult GetAllNotDeletedDomains()
        {
            return Ok(_domainService.GetAllNotDeleted()); // ar veiks? paprastesnis variantas apacioj.

        }
        //public IEnumerable<DomainModel> GetAllNotDeletedDomains()
        //{
        //    return _domainService.GetAllNotDeleted();

        //}

        // GET: api/Domain/5
        [HttpGet("{id}")]
        public ActionResult<DomainModel> GetDomainModel(int id)
        {
            DomainModel model = _domainService.GetById(id);
            if(model == null)
            {
                return NotFound();
            }
            return model;
        }

        [HttpPost]
        public ActionResult<DomainModel> CreateDomainModel(DomainModel domainModel)
        {
            if (ModelState.IsValid) //validacija backendo. patikrina ar yra visi required fieldai modeli.
            {
                DomainModel createdModel = _domainService.Create(domainModel);
                if(createdModel == null)
                {
                    return BadRequest();
                }
                return Ok(createdModel);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public ActionResult<DomainModel> EditDomainModel(int id, DomainModel domainModel)
        {

            if (ModelState.IsValid)
            {
                return _domainService.Update(id, domainModel);
            }
            return BadRequest();
        }

        // PUT for delete: api/Domain/5
        [HttpPut("del/{id}")]
        public ActionResult<DomainModel> PseudoDeleteDomainModel(int id)
        {
            DomainModel model = _domainService.PseudoDelete(id);
            if (model == null)
            {
                return NotFound();
            }
            return model;
        }
    }
}
