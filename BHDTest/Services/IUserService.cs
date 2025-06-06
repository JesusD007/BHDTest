using BHDTest.DTOs;

namespace BHDTest.Services
{
    public interface IUserService
    {
        string GetToken(string user);
        Task<(bool, UserCreateResponseDto)> Add(UserCreateRequestDto user);
        Task<IEnumerable<UserDto>> GetAll();


    }
}
