using BookingService.Models.Models.Entities;

namespace BookingService.Services.Services.Interfaces;

public interface IJwtService
{
    string GenerateJwt(User user);
}
