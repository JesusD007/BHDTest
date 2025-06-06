using BHDTest.Models;
using Microsoft.EntityFrameworkCore;

namespace BHDTest.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BHDPruebaContext _context;
        public UserRepository(BHDPruebaContext context)
        {
            _context = context;
        }
        public async Task<bool> Add(User user)
        {
            _context.Users.Add(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;

        }

        public async Task<List<User>> GetAll()
        {
            var userList = await _context.Users.Select(b=> new User
            {   
                Id = b.Id,
                Name = b.Name,
                Email = b.Email,
                Password = b.Password,
                Created = b.Created,
                Modified = b.Modified,
                LastLogin = b.LastLogin,
                IsActive = b.IsActive,
                Phones = b.Phones
            }).ToListAsync();
            return userList;
        }
        public async Task<User?> GetByEmail(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

    }
}
