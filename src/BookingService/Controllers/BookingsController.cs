using AutoMapper;
using BookingService.Models.Models.Dtos;
using BookingService.Models.Models.Entities;
using BookingService.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookRoomService _bookRoomService;
        private readonly ILogger<BookingsController> _logger;

        public BookingsController(
            IBookRoomService bookRoomService,
            ILogger<BookingsController> logger)
        {
            _bookRoomService = bookRoomService;
            _logger = logger;
        }

        [HttpPost("create-booking")]
        [Authorize]
        public async Task<ActionResult<Booking>> CreateBooking([FromBody] CreateBookingRequest bookingRequest)
        {
            _logger.LogInformation("BookingsController: Trying to create booking. CreateBooking called. BookingRequest: {BookingRequest}", JsonSerializer.Serialize(bookingRequest));

            return Ok(await _bookRoomService.CreateBookingAsync(bookingRequest));
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<ActionResult<List<Booking>>> GetUserBookings([FromRoute] string userId)
        {
            _logger.LogInformation("BookingsController: Trying to user bookings. GetUserBookings called. UserId: {UserId}", userId);

            return Ok(await _bookRoomService.GetUserBookingsAsync(userId));
        }
    }
}
