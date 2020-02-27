using DashBoard.Business;
using DashBoard.Business.CustomExceptions;
using DashBoard.Business.DTOs.Users;
using DashBoard.Business.Services;
using DashBoard.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DashBoard.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly AppSettings _appSettings;
        public string LoggedInUser => User.Identity.Name; //this gets current user ID. It doesn't work in controller. We have to pass it from here.

        public UsersController(
            IUserService userService,
            IOptions<AppSettings> appSettings,
            ITokenService tokenService)
        {
            _tokenService = tokenService;
            _userService = userService;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]AuthenticateModelDto model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            var token = _tokenService.GenerateToken(user, _appSettings.Secret);

            // return basic user info and authentication token
            return Ok(new
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role,
                Token = token
            });
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterModelDto model)
        {
            try
            {
                var userId = LoggedInUser; //this will be null, since method is Anonymous.
                model.CreatedByAdmin = false;
                // create user
                var user = _userService.Create(model, model.Password, userId);
                return Ok(new {id = user.Id, username = user.Username});
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Roles = Role.Admin)]
        [HttpPost("admin/register")]
        public IActionResult RegisterByAdmin([FromBody]RegisterModelDto model)
        {
            try
            {
                var userId = LoggedInUser;
                model.CreatedByAdmin = true;
                // create user
                var user = _userService.Create(model, model.Password, userId);
                return Ok(new
                {
                    Id = user.Id,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role
                });
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = LoggedInUser;
            var usersDto = _userService.GetAll(userId);
            return Ok(usersDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id) //this might be missing not found actionResult.
        {
            var userId = LoggedInUser;
            var userDto = _userService.GetById(id, userId);

            if (userDto != null)
            {
                return Ok(userDto);
            }
            else
            {
                return NotFound();
            }
            
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UpdateModelDto model)
        {
            try
            {
                var userId = LoggedInUser;
                // update user 
                var result = _userService.Update(id, model, userId);
                
                if (result == "notAllowed")
                {
                    return StatusCode(403);
                }
                else
                {
                    return Ok();
                }
                
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")] //this might be missing not found actionResult.
        public IActionResult Delete(int id)
        {
            var userId = LoggedInUser;
            var result = _userService.Delete(id, userId);

            if (result == "ok")
            {
                return Ok();
            }
            else if (result == "notFound")
            {
                return NotFound();
            }
            else if (result == "notAllowed")
            {
                return StatusCode(403);
            }

            return NotFound();
        }
    }
}
