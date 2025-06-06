using BHDTest.DTOs;

namespace BHDTest.Services
{
    public interface IUserService
    {
        string GetToken(string user, string pass);
        

    }
}
