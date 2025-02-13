using BookingService.Models.Models.Dtos;
using BookingService.Models.Models.Entities;

namespace BookingService.Services.Services.Interfaces;

public interface IUserService
{
    Task<User> RegisterNewUserAsync(User user);

    Task<UserLoginResponse> LoginAsync(UserLoginRequest userCredentials);
}
