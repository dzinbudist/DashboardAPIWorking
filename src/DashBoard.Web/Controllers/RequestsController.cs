using System.Threading.Tasks;
using DashBoard.Business.Services;
using DashBoard.Business.DTOs.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DashBoard.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RequestsController : ControllerBase
    {
        private readonly IRequestService _requestService;
        public RequestsController(IRequestService requestService)
        {
            _requestService = requestService;
        }

        [HttpGet("getservice/{id}")]
        public async Task<ActionResult<object>> GetService(int id)
        {
            var response = await _requestService.GetService(id, null);
            if (response == null)
            {
                return NotFound(new { message = $"Problem reaching service with id {id}" });
            }
            return response;
        }

        [HttpPost("testservice")]
        public async Task<ActionResult<object>> TestService(DomainForCreationDto domain)
        {
            var response = await _requestService.GetService(-555, domain);
            if (response == null)
            {
                return NotFound(new { message = $"Problem reaching portal" });
            }
            return response;
        }
    }
}