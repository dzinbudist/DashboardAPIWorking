using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Business.DTOs.Domains;
using WebApi.Business.Services;
using WebApi.Data.Entities;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DomainController : ControllerBase
    {
        private readonly IDomainService _domainService;
        public DomainController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        // GET: api/Domain
        [HttpGet]
        public IActionResult GetAllNotDeletedDomains()
        {
            var result = _domainService.GetAllNotDeleted();
            return Ok(result);
        }

        // GET: api/Domain/5
        [HttpGet("{id}")]
        public ActionResult<DomainModelDto> GetDomainModel(int id) //cia gal irgi geriau butu iactionresult?
        {
            var result = _domainService.GetById(id);
            if(result == null)
            {
                return NotFound();
            }
            return result;
        }

        [HttpPost]
        public IActionResult CreateDomainModel(DomainForCreationDto domain)
        {
            if (ModelState.IsValid) //validacija backendo. patikrina ar yra visi required fieldai modeli.
            {
                _domainService.Create(domain);

                return Ok(); //created at action reiktu.
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        [HttpPut("{id}")]
        public IActionResult EditDomainModel(int id, DomainForUpdateDto domain)
        {

            if (ModelState.IsValid)
            {
                var result =_domainService.Update(id, domain);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            else
            {
                return BadRequest(ModelState);
            }
            
        }

        // PUT for delete: api/Domain/5
        [HttpPut("del/{id}")]
        public IActionResult PseudoDeleteDomainModel(int id)
        {
            var result = _domainService.PseudoDelete(id);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }
    }
}
