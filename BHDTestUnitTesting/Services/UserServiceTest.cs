using BHDTest.DTOs;
using BHDTest.Models;
using BHDTest.Repositories;
using BHDTest.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace BHDTestUnitTesting.Services
{
    public class UserServiceTest
    {
        private readonly IConfiguration _configuration;
        private readonly UserService _userService;
        private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
        private readonly Mock<IPhoneRepository> _phoneRepositoryMock = new Mock<IPhoneRepository>();

        public UserServiceTest()
        {
            // Key simulation
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(c => c["Jwt:Key"]).Returns("La super mega ultra duper expectacular clave del JWT ");

            _configuration = configurationMock.Object;

            // UserService Building
            _userService = new UserService(_configuration,_userRepositoryMock.Object, _phoneRepositoryMock.Object);
        }
        #region Add User
        [Fact]
        public async Task Add_ShouldAddUserAndReturnATuple()
        {
            // Arrange
            var request = new UserCreateRequestDto
            {
                Nombre = "Jesus",
                Email = "Jesus@gmail.com",
                Password = "Hunter123000$#",
                Phones = new List<PhoneRequestDto>
                {
                    new PhoneRequestDto { Number = "1234567", CityCode = "10", CountryCode = "657" }
                }
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmail(request.Email))
                .ReturnsAsync((User)null!);

            _userRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<User>()))
                .ReturnsAsync(true); 

            // Act
            var (success, result) = await _userService.Add(request);

            // Assert
            Assert.True(success);
            Assert.NotNull(result);
            Assert.NotNull(result.Token);
            Assert.NotEmpty(result.Token);
            Assert.NotEmpty(result.Id.ToString());

        }
        // Error cases
        // Email already exists
        [Fact]
        public async Task Add_EmailAlreadyExists_ShouldReturnInvalidOperationException()
        {
            // Arrange
            var request = new UserCreateRequestDto
            {
                Nombre = "Jesus",
                Email = "jesus@gmail.com",
                Password = "Hunter123000$##",
                Phones = new List<PhoneRequestDto>
                {
                    new PhoneRequestDto { Number = "987654321", CityCode = "10", CountryCode = "657" }
                }
            };
            var response = new User
            {
                Id = Guid.NewGuid(),
                Name = "nombre",
                Email = "jesus@gmail.com",
                Password = "Prueba123#@2200",
                Created = DateTime.UtcNow,
                Modified = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
                IsActive = true,
                Phones = new List<Phone>
                {
                    new Phone { Number = "98765432", CityCode = "10", CountryCode = "657" }
                }
            };

            _userRepositoryMock
                .Setup(repo => repo.GetByEmail(request.Email))
                .ReturnsAsync(response);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _userService.Add(request));
            Assert.NotNull(ex);
            Assert.Equal("Ya existe un usuario registrado con el correo 'jesus@gmail.com'.", ex.Message);
        }

        // Database error
        [Fact]
        public async Task Add_DataBaseFailed_ShouldThrowApplicationException()
        {
            // Arrange
            var request = new UserCreateRequestDto
            {
                Nombre = "Jesus",
                Email = "jesus@gmail.com",
                Password = "Hunter123000$##",
                Phones = new List<PhoneRequestDto>
                {
                    new PhoneRequestDto { Number = "987654321", CityCode = "10", CountryCode = "657" }
                }
            };

            _userRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<User>()))
                .ThrowsAsync(new DbUpdateException());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _userService.Add(request));
            Assert.NotNull(ex);
            Assert.Equal("Error de base de datos al agregar el usuario", ex.Message);
        }

        // General error
        [Fact]
        public async Task Add_UnexpectedError_ShouldThrowApplicationException()
        {
            // Arrange
            var request = new UserCreateRequestDto
            {
                Nombre = "Jesus",
                Email = "jesus@gmail.com",
                Password = "Hunter123000$##",
                Phones = new List<PhoneRequestDto>
                {
                    new PhoneRequestDto { Number = "987654321", CityCode = "10", CountryCode = "657" }
                }
            };

            _userRepositoryMock
                .Setup(repo => repo.Add(It.IsAny<User>()))
                .ThrowsAsync(new Exception());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _userService.Add(request));
            Assert.NotNull(ex);
            Assert.Equal("Ocurrió un error inesperado al agregar el usuario", ex.Message);
        }
        #endregion

        #region Get All 
        [Fact]
        public async Task GetAll_ShouldReturnUserDtoList()
        {
            // Arrange
            var userData = new List<User>
            {
                new User
                {
                    Id = Guid.Parse("24d36962-1d90-4e03-831b-dacd2c754417"),
                    Name = "Jesus",
                    Email = "Jesus@gmail.com",
                    Password = "DummyPassword2",
                    Created = DateTime.UtcNow,
                    Modified = DateTime.UtcNow,
                    LastLogin = DateTime.UtcNow,
                    IsActive = true                   
                }
            };

            var phoneData = new List<Phone>
            {
                new Phone
                {
                    Id = 1,
                    Number = "987654321",
                    CityCode = "10",
                    CountryCode = "657",
                    UserId = Guid.Parse("24d36962-1d90-4e03-831b-dacd2c754417")
                }
            };
            _userRepositoryMock
                .Setup(repo => repo.GetAll())
                .ReturnsAsync(userData);

            _phoneRepositoryMock
                .Setup(repo => repo.GetByUserID(It.IsAny<Guid>()))
                .ReturnsAsync(phoneData);

            // Act
            var result = await _userService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<UserDto>>(result);
            Assert.Single(result);

        }
        // Error cases
        // Database error
        [Fact]
        public async Task GetAll_DataBaseFailed_ShouldThrowApplicationException()
        {
            _userRepositoryMock
                .Setup(repo => repo.GetAll())
                .ThrowsAsync(new DbUpdateException());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _userService.GetAll());
            Assert.NotNull(ex);
            Assert.Equal("Error de base de datos al obtener los usuarios", ex.Message);
        }

        // General error
        [Fact]
        public async Task GetAll_UnexpectedError_ShouldThrowApplicationException()
        {
          
            _userRepositoryMock
                .Setup(repo => repo.GetAll())
                .ThrowsAsync(new Exception());

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ApplicationException>(() => _userService.GetAll());
            Assert.NotNull(ex);
            Assert.Equal("Ocurrió un error inesperado al obtener los usuarios", ex.Message);
        }


        #endregion

        #region Get token
        [Fact]
        public void GetToken_ShouldReturnValidJwtToken()
        {
            // Arrange
            var username = "Jesus@gmail.com";

            // Act
            var token = _userService.GetToken(username);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            Assert.NotNull(jwtToken);
        }
        #endregion
    }
}