using AutoMapper;
using BookingService.Models.Models.Dtos;
using BookingService.Models.Models.Entities;
using BookingService.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;
    private readonly IMapper _mapper;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger,
        IMapper mapper)
    {
        _userService = userService;
        _logger = logger;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("register/user")]
    public async Task<ActionResult<User>> RegisterUserAsync([FromBody] CreateUserRequest createUserRequest)
    {
        _logger.LogInformation("UsersController: Trying to register user. RegisterUserAsync called");

        var user = _mapper.Map<User>(createUserRequest);

        var userCreated = await _userService.RegisterNewUserAsync(user);

        return Ok(userCreated);
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<UserLoginResponse>> LoginAsync([FromBody] UserLoginRequest userLoginRequest)
    {
        _logger.LogInformation("UsersController: Trying to login. LoginAsync called");

        var response = await _userService.LoginAsync(userLoginRequest);

        return Ok(response);
    }
}
