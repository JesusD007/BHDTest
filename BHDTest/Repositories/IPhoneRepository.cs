using BHDTest.Models;

namespace BHDTest.Repositories
{
    public interface IPhoneRepository
    {
        Task<List<Phone>> GetByUserID(Guid id);
        Task<bool> Add(Phone phone);

    }
}
