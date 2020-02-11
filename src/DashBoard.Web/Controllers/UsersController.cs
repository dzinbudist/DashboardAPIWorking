using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using DashBoard.Business.CustomExceptions;
using DashBoard.Business.DTOs.Users;
using DashBoard.Business.Services;
using DashBoard.Data.Entities;
using DashBoard.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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
                // create user
                _userService.Create(model, model.Password);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var usersDto = _userService.GetAll();
            return Ok(usersDto);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id) //this might be missing not found actionResult.
        {
            var userDto = _userService.GetById(id);
            
            return Ok(userDto);
        }

        //Authorize(Roles = Role.Admin)]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]UpdateModelDto model)
        {
            try
            {
                // update user 
                _userService.Update(id, model);
                return Ok();
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")] //this might be missing not found actionResult.
        public IActionResult Delete(int id)
        {
            _userService.Delete(id);
            return Ok();
        }
    }
}
