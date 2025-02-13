namespace BookingService.Models.Models.Dtos;

public class UserLoginResponse
{
    public required string Id { get; set; }

    public required string JwtToken { get; set; }
}
