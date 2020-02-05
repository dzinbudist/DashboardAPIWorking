using DashBoard.Business.DTOs.Domains;
using DashBoard.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DashBoard.Web.Controllers
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
        [ProducesResponseType(StatusCodes.Status200OK)] //are these necessary ? Removed them from other actions.
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllNotDeletedDomains()
        {
            var result = _domainService.GetAllNotDeleted();
            if (result == null)
            {
                return NotFound(); //NoContent() is also an option here.
            }
            return Ok(result);
        }

        // GET: api/Domain/5
        [HttpGet("{id}")]
        public IActionResult GetDomainModel(int id)
        {
            var result = _domainService.GetById(id);
            if(result == null)
            {
                return NotFound(); //NoContent() is also an option here.
            }
            return Ok(result);
        }

        [HttpPost]
        public IActionResult CreateDomainModel(DomainForCreationDto domain)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = _domainService.Create(domain);
            return CreatedAtAction("GetDomainModel", new {id = result.Id}, result);

        }

        [HttpPut("{id}")]
        public IActionResult EditDomainModel(int id, DomainForUpdateDto domain) //client has to send full DomaintForUPdateDto model.
        {

            if (ModelState.IsValid)
            {
                var result =_domainService.Update(id, domain);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok();
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
            return Ok();
        }
    }
}
