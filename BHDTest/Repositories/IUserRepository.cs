using BHDTest.Models;

namespace BHDTest.Repositories
{
    public interface IUserRepository
    {
        public Task<List<User>> GetAll();
        Task<User?> GetByEmail(string email);
        public Task<bool> Add(User user);
    }
}
