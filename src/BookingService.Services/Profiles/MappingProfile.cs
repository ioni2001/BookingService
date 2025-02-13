using AutoMapper;
using BookingService.Models.Models.Dtos;
using BookingService.Models.Models.Entities;
using System.Globalization;

namespace BookingService.Services.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateUserRequest, User>();
        CreateMap<CreateBookingRequest, Booking>()
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => TimeSpan.ParseExact(src.StartTime, @"hh\:mm", CultureInfo.InvariantCulture)))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => TimeSpan.ParseExact(src.EndTime, @"hh\:mm", CultureInfo.InvariantCulture)));
        CreateMap<CreateBookingRequest, BookingCreatedMessage>()
            .ForMember(dest => dest.Event, opt => opt.MapFrom(src => "Booking_Created"));
    }
}