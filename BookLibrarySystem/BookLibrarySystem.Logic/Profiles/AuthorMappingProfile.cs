using AutoMapper;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;


namespace BookLibrarySystem.Logic.Profiles
{
    public class AuthorMappingProfile : Profile
    {
        public AuthorMappingProfile() 
        {
            CreateMap<AuthorDto, Author>().
            ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            )
            .ForMember(
                dest => dest.Name,
                opt => opt.MapFrom(src => $"{src.Name}")
            )
            .ForMember(
                dest => dest.Country,
                opt => opt.MapFrom(src => $"{src.Country}")
            )
            .ForMember
            (
                dest => dest.Books,
                opt => opt.Ignore()
            )
            .ReverseMap();
        }   

    }
}
