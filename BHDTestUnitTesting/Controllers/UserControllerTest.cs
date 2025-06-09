using Microsoft.AspNetCore.Mvc;
using Moq;
using BHDTest.DTOs;
using BHDTest.Services;
using BHDTest.Controllers;
using FluentValidation;
using FluentValidation.Results;

namespace BHDTestUnitTesting.ControllersTests
{
    public class UserControllerTest
    {
        private readonly UserController _userController;
        private readonly Mock<IUserService> _userServiceMock = new Mock<IUserService>();
        private readonly Mock<IValidator<UserCreateRequestDto>> _validatorMock = new();

        public UserControllerTest()
        {
            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<UserCreateRequestDto>(), default))
                .ReturnsAsync(new ValidationResult());
            _userController = new UserController(_userServiceMock.Object, _validatorMock.Object);
        }
        // Get all users tests
        [Fact]
        public async Task GetAllUsers_ShouldReturnOkResult()
        {
            // Arrange
            var response = new List<UserDto>
            {
                new UserDto
                {
                    Id = Guid.Parse("24d36962-1d90-4e03-831b-dacd2c754416"),
                    Email = "jesusdRM@gmail.com",
                    Token = "",
                    Phones = new List<PhoneDto>
                    {
                        new PhoneDto{
                            Number = "1234567",
                            CityCode = "1",
                            CountryCode = "57"
                        }
                    }
                }
            };

            // Simulate that service returns a list of users
            _userServiceMock
                .Setup(s => s.GetAll())
                .ReturnsAsync(response);

            // Act
            var result = await _userController.GetAllUsers() as OkObjectResult;

            // Assert
            // Resoponse validation
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            // Response type validation
            Assert.IsType<List<UserDto>>(result.Value);
            var resultList = result.Value as List<UserDto>;
            Assert.NotNull(resultList);
            Assert.Equal("jesusdRM@gmail.com", resultList[0].Email);
        }

        // Add user tests
        [Fact]
        public async Task AddUser_ShouldReturnCreatedResultAsync()
        {
            // Arrange
            var request = new UserCreateRequestDto
            {
                Nombre = "Juan",
                Email = "juan@correo.com",
                Password = "Hunter20000$#",
                Phones = new List<PhoneRequestDto>
                {
                    new PhoneRequestDto { Number = "1234567", CityCode = "1", CountryCode = "57" }
                }
            };

            var response = new UserCreateResponseDto
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                Token = "token_jwt",
                IsActive = true
            };
            // Simulate that validator pass
            _userServiceMock
                .Setup(s => s.Add(It.IsAny<UserCreateRequestDto>()))
                .ReturnsAsync((true, response));

            // Act
            var result = await _userController.AddUser(request) as ObjectResult;
            // Assert

            // Resoponse validation
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);

            // Response type validation
            var value = Assert.IsType<UserCreateResponseDto>(result.Value);
            Assert.Equal(response.Id, value.Id);
        }

        // Error cases
        // Invalid request (Just 1 invalid request is tested because any other cases are the same just change Validation Failure message)
        [Fact]
        public async Task CreateUser_EmptyEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new UserCreateRequestDto
            {
                Nombre = "Juan",
                Email = "",
                Password = "Hunter20000$#",
                Phones = new List<PhoneRequestDto>
                {
                    new PhoneRequestDto { Number = "1234567", CityCode = "1", CountryCode = "57" }
                }
            };

            // Simulate that validator throw: the email is empty
            _validatorMock
                .Setup(v => v.ValidateAsync(request, default))
                .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
                {
                    new ValidationFailure("Email", "El email es obligatorio.")
                }));

            // Act
            var result = await _userController.AddUser(request) as BadRequestObjectResult;
            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.IsType<ErrorResponseDto>(result.Value);
            var errorResponse = result.Value as ErrorResponseDto;
            Assert.NotNull(errorResponse);
            Assert.Contains("El email es obligatorio.", errorResponse.Mensaje);

        }
        // User already exists
        [Fact]
        public async Task CreateUser_AlreadyExists_ShouldReturnConflict()
        {
            // Arrange
            var request = new UserCreateRequestDto
            {
                Nombre = "Juan",
                Email = "juan@correo.com",
                Password = "Hunter20000$#",
                Phones = new List<PhoneRequestDto>
                {
                    new PhoneRequestDto { Number = "1234567", CityCode = "1", CountryCode = "57" }
                }
            };

            // Simulate that service throw: the user already exists
            _userServiceMock
                .Setup(s => s.Add(It.IsAny<UserCreateRequestDto>()))
                .ThrowsAsync(new InvalidOperationException("Ya existe un usuario registrado con el correo 'juan@correo.com'."));

            // Act
            var result = await _userController.AddUser(request) as ConflictObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(409, result.StatusCode);
            Assert.IsType<ErrorResponseDto>(result.Value);
            var errorResponse = result.Value as ErrorResponseDto;
            Assert.NotNull(errorResponse);
            Assert.Equal("Ya existe un usuario registrado con ese correo.", errorResponse.Mensaje);


        }
        // Internal server error
        [Fact]
        public async Task CreateUser_InternalServerError_ShouldReturn500()
        {
            // Arrange
            var request = new UserCreateRequestDto
            {
                Nombre = "Juan",
                Email = "juan@correo.com",
                Password = "Hunter20000$#",
                Phones = new List<PhoneRequestDto>
                {
                    new PhoneRequestDto { Number = "1234567", CityCode = "1", CountryCode = "57" }
                }
            };

            // Simulate that service throw: an unexpected error
            _userServiceMock
                .Setup(s => s.Add(It.IsAny<UserCreateRequestDto>()))
                .ThrowsAsync(new Exception("Ocurrió un error inesperado al agregar el usuario"));

            // Act
            var result = await _userController.AddUser(request) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
            Assert.IsType<ErrorResponseDto>(result.Value);
            var errorResponse = result.Value as ErrorResponseDto;
            Assert.NotNull(errorResponse);
            Assert.Equal("Error interno del servidor: Ocurrió un error inesperado al agregar el usuario", errorResponse.Mensaje);
        }
    }
}
