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
    [Produces("application/json")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Obtiene todos los usuarios registrados en el sistema.
        /// </summary>
        /// <returns>Lista de usuarios con sus datos básicos.</returns>
        /// <response code="200">Retorna la lista de usuarios</response>
        /// <response code="400">Error al procesar la solicitud</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ErrorResponseDto), 401)]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), 200)]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
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

        /// <summary>
        /// Crea un nuevo usuario con sus datos y teléfonos asociados.
        /// </summary>
        /// <param name="userDto">Datos del usuario a crear</param>
        /// <param name="validator">Validador FluentValidation</param>
        /// <returns>Información del usuario creado</returns>
        /// <response code="201">Usuario creado correctamente</response>
        /// <response code="400">Datos inválidos o error al crear</response>
        /// <response code="409">Ya existe un usuario con el mismo correo</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpPost]
        [Consumes("application/json")]
        [ProducesResponseType(typeof(UserCreateResponseDto), 201)]
        [ProducesResponseType(typeof(ErrorResponseDto), 400)]
        [ProducesResponseType(typeof(ErrorResponseDto), 409)]
        [ProducesResponseType(typeof(ErrorResponseDto), 500)]
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

        /// <summary>
        /// Obtiene un token JWT de prueba (modo estático).
        /// </summary>
        /// <returns>Un objeto con el token JWT</returns>
        /// <response code="200">Token generado correctamente</response>
        /// <response code="400">Error al generar el token</response>
        [HttpGet("token")]
        [ProducesResponseType(typeof(UserDto), 200)]
        [ProducesResponseType(typeof(object), 400)]
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
