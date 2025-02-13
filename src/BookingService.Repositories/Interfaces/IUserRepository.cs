using BookingService.Models.Models.Entities;

namespace BookingService.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email);
}
