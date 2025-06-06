using BHDTest.DTOs;
using BHDTest.Models;
using BHDTest.Services;
using FluentValidation;
using FluentValidation.Results;
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

        // GET: api/User
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAll();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new ErrorResponseDto(ex.Message));
            }
        }
        // POST: api/User
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserCreateRequestDto userDto, [FromServices] IValidator<UserCreateRequestDto> validator)
        {
            ValidationResult validationResult = await validator.ValidateAsync(userDto);

            if (!validationResult.IsValid)
            {
                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage);
   
                return BadRequest(new ErrorResponseDto(string.Join(" ", errorMessages)));
            }

            try
            {
                Guid posibleUserId = Guid.Empty;
                var (success, createdUser) = await _userService.Add(userDto);

                if (success)
                {
                    return StatusCode(201, createdUser);
                }

                return BadRequest(new ErrorResponseDto("No se pudo crear el usuario en la base de datos."));
            }
            catch (InvalidOperationException)
            {
                return Conflict(new ErrorResponseDto("Ya existe un usuario registrado con ese correo."));
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new ErrorResponseDto(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponseDto("Error interno del servidor: " + ex.Message));
            }
        }



        [HttpGet("token")]
        public ActionResult GetToken()
        {
            string name = "Jrodriguez";
            try
            {
                var token = _userService.GetToken(name);
                UserDto user = new UserDto
                {
                    Token = token
                };
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
