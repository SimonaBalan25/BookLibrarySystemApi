

using AutoMapper;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;

namespace BookLibrarySystem.Logic.Profiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<UserDto, ApplicationUser>().
            ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => $"{src.Name}")
            )
            .ForMember(
                dest => dest.Address,
                opt => opt.MapFrom(src => $"{src.Address}")
            )
            .ForMember
            (
                dest => dest.Status,
                opt => opt.MapFrom(src => $"{src.Status}")
            )
            .ForMember
            (
                dest => dest.BirthDate,
                opt => opt.MapFrom(src => $"{src.BirthDate}")
            )
            .ForMember
            (
                dest => dest.Email,
                opt => opt.MapFrom(src => $"{src.Email}")
            )
            .ForMember
            (
                dest => dest.UserName,
                opt => opt.MapFrom(src => $"{src.UserName}")
            )
            .ForMember
            (
                dest => dest.PhoneNumber,
                opt => opt.MapFrom(src => $"{src.PhoneNumber}")
            )
            .ForMember
            (
                dest => dest.PhoneNumberConfirmed,
                opt => opt.MapFrom(src => $"{src.PhoneNumberConfirmed}")
            )
            .ForMember
            (
                dest => dest.EmailConfirmed,
                opt => opt.MapFrom(src => $"{src.EmailConfirmed}")
            )
            .ReverseMap();
        }
    }
}
