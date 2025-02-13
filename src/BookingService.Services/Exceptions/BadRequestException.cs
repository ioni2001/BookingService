namespace BookingService.Services.Exceptions;

public class BadRequestException(string message) : Exception(message)
{
}
