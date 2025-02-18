using BookingService.Models.Models.Dtos;
using BookingService.Models.Models.Entities;
using BookingService.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IBookRoomService _bookRoomService;
        private readonly ILogger<RoomsController> _logger;

        public RoomsController(IBookRoomService bookRoomService, ILogger<RoomsController> logger)
        {
            _bookRoomService = bookRoomService;
            _logger = logger;
        }

        [HttpGet("room-availability/{roomId}/{date}")]
        [Authorize]
        public async Task<ActionResult<List<RoomAvailabilityDto>>> GetRoomAvailability([FromRoute] string roomId, [FromRoute] DateOnly date)
        {
            _logger.LogInformation("RoomsController: Trying to get room availability. GetRoomAvailability called. RoomId: {RoomId}. Date: {Date}", roomId, date.ToString());

            return Ok(await _bookRoomService.GetRoomAvailability(roomId, date));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Room>>> GetAllRooms()
        {
            _logger.LogInformation("RoomsController: Trying to get all rooms");

            return Ok(await _bookRoomService.GetAllRooms());
        }
    }
}
