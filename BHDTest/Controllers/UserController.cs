using BHDTest.DTOs;
using BHDTest.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace BHDTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public ActionResult GetAllUsers()
        {
            return Ok();
        }

        [HttpGet("token")]
        public ActionResult GetToken()
        {
            string name = "jesus";
            string pass = "123";
            try
            {
                var token = _userService.GetToken(name, pass);
                UserDto usuario = new UserDto
                {
                    Token = token
                };
                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
