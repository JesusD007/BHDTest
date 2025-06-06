using BHDTest.Models;
using Microsoft.EntityFrameworkCore;

namespace BHDTest.Repositories
{
    public class PhoneRepository : IPhoneRepository
    {
        private BHDPruebaContext _context;
        public PhoneRepository(BHDPruebaContext context)
        {
            _context = context;
        }

        public async Task<bool> Add(Phone phone)
        {
            _context.Phones.Add(phone);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<Phone>> GetByUserID(Guid id)
        {
            return await _context.Phones
                                 .Where(b => b.UserId == id)
                                 .ToListAsync();
        }
    }
}
