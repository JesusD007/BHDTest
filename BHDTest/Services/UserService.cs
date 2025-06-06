using BHDTest.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BHDTest.Repositories;
using Microsoft.EntityFrameworkCore;


namespace BHDTest.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IPhoneRepository _phoneRepository;


        public UserService(IConfiguration configuration, IUserRepository repository, IPhoneRepository phoneRepository)
        {
            _configuration = configuration;
            _userRepository = repository;
            _phoneRepository = phoneRepository;

        }

        public async Task<(bool , UserCreateResponseDto)> Add(UserCreateRequestDto user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "El usuario no puede ser nulo");
            }
            try
            {
                var newUser = new Models.User();
                newUser.Name = user.Nombre!;
                newUser.Email = user.Email!;
                newUser.Password = user.Password!;

                if (user.Phones != null && user.Phones.Any())
                {
                    newUser.Phones = user.Phones.Select(p => new Models.Phone
                    {
                        Number = p.Number,
                        CityCode = p.CityCode,       
                        CountryCode = p.CountryCode, 
                        UserId = newUser.Id
                    }).ToList();
                }

                UserCreateResponseDto userCreateResponseDto = new UserCreateResponseDto
                {
                    Id = newUser.Id,
                    Created = newUser.Created,
                    Modified = newUser.Modified,
                    LastLogin = newUser.LastLogin,
                    IsActive = newUser.IsActive,
                    Token = GetToken(user.Email!)
                };
                var existingUser = await _userRepository.GetByEmail(user.Email!);
                if (existingUser != null)
                {
                    throw new InvalidOperationException($"Ya existe un usuario registrado con el correo '{user.Email}'.");
                }

                return (await _userRepository.Add(newUser), userCreateResponseDto);
            }
            catch (DbUpdateException ex)
            {
                throw new ApplicationException("Error de base de datos al agregar el usuario", ex);
            }
            catch (Exception ex) when (ex is not InvalidOperationException)
            {
                throw new ApplicationException("Ocurrió un error inesperado al agregar el usuario", ex);
            }
        }
        public async Task<IEnumerable<UserDto>> GetAll()
        {
            try
            {
                var users = await _userRepository.GetAll();
                var userDtos = new List<UserDto>();
                foreach (var u in users)
                {
                    var phones = await _phoneRepository.GetByUserID(u.Id);
                    var phoneDtos = phones.Select(p => new PhoneDto
                    {
                        Number = p.Number,
                        CityCode = p.CityCode,
                        CountryCode = p.CountryCode,
                    }).ToList();


                    userDtos.Add(new UserDto
                    {
                        Id = u.Id,
                        Email = u.Email,
                        Phones = phoneDtos
                    });
                }
                return userDtos;   
            }
            catch (DbUpdateException ex)
            {
                throw new ApplicationException("Error de base de datos al obtener los usuarios", ex);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Ocurrió un error inesperado al obtener los usuarios", ex);
            }
        }
        public string GetToken(string user)
        {
            
            var secretKey = _configuration["Jwt:Key"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var byteKey = Encoding.UTF8.GetBytes(secretKey!);

            var tokenDesc = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, user),
            }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(byteKey), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDesc);
            return tokenHandler.WriteToken(token);
        }
    }
}
