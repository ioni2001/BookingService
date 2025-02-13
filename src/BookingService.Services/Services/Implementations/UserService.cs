using BookingService.Models.Models.Dtos;
using BookingService.Models.Models.Entities;
using BookingService.Repositories.Interfaces;
using BookingService.Services.Exceptions;
using BookingService.Services.Services.Interfaces;

namespace BookingService.Services.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public UserService(IJwtService jwtService, IUserRepository userRepository)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
    }

    public async Task<UserLoginResponse> LoginAsync(UserLoginRequest userCredentials)
    {
        await ValidateUserCredentials(userCredentials);

        var user = await _userRepository.GetUserByEmailAsync(userCredentials.Email);

        var jwtToken = _jwtService.GenerateJwt(user!);

        return new UserLoginResponse
        {
            Id = user!.Id,
            JwtToken = jwtToken,
        };
    }

    public async Task<User> RegisterNewUserAsync(User user)
    {
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        await _userRepository.SaveAsync(user);

        return user;
    }

    private async Task ValidateUserCredentials(UserLoginRequest userCredentials)
    {
        var user = await _userRepository.GetUserByEmailAsync(userCredentials.Email) ?? throw new BadRequestException("Invalid credentials!");

        if (!BCrypt.Net.BCrypt.Verify(userCredentials.Password, user.Password))
        {
            throw new BadRequestException("Invalid credentials!");
        }
    }
}
