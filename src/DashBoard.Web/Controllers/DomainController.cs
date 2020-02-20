using System.Threading.Tasks;
using DashBoard.Business.DTOs.Domains;
using DashBoard.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DashBoard.Web.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class DomainController : ControllerBase
    {
        private readonly IDomainService _domainService;
        public string LoggedInUser => User.Identity.Name; //this gets current user ID. We need to pass it to services in parameters. This is not ideal, couldn't find a way to get user in controller.
        public DomainController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] //are these necessary ? Removed them from other actions.
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetAllNotDeletedDomains()
        {
            var userId = LoggedInUser;
            var result = _domainService.GetAllNotDeleted(userId);
            if (result == null)
            {
                return NotFound(); //NoContent() is also an option here.
            }
            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetDomainModel(int id)
        {
            var userId = LoggedInUser;
            var result = _domainService.GetById(id, userId);
            if(result == null)
            {
                return NotFound(); //NoContent() is also an option here.
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDomainModel(DomainForCreationDto domain)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var userId = LoggedInUser;
            var result = await _domainService.Create(domain, userId);
            return CreatedAtAction("GetDomainModel", new {id = result.Id}, result);
        }

        //this might need protection from deleting/updating not your team_key items.
        [HttpPut("{id}")]
        public IActionResult EditDomainModel(int id, DomainForUpdateDto domain) //client has to send full DomaintForUPdateDto model. Other option is to use Patch HTTP method instead of PUT
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = LoggedInUser;
            var result = _domainService.Update(id, domain, userId);
            if (result == null)
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPut("del/{id}")]
        public IActionResult PseudoDeleteDomainModel(int id)
        {
            var userId = LoggedInUser;
            var result = _domainService.PseudoDelete(id, userId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
