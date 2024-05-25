

using AutoMapper;
using BookLibrarySystem.Data.Models;
using BookLibrarySystem.Logic.DTOs;

namespace BookLibrarySystem.Logic.Profiles
{
    public class BookMappingProfile : Profile
    {
        public BookMappingProfile() 
        {
            CreateMap<BookDto, Book>()
            //.ForMember(
            //    dest => dest.Id,
            //    opt => opt.MapFrom(src => Guid.NewGuid())
            //)
            .ForMember(
                dest => dest.ISBN,
                opt => opt.MapFrom(src => src.ISBN)
            )
            .ForMember(
                dest => dest.Publisher,
                opt => opt.MapFrom(src => src.Publisher)
            )
            .ForMember(
                dest => dest.NumberOfPages,
                opt => opt.MapFrom(src => src.NumberOfPages)
            )
            .ForMember(
                dest => dest.NumberOfCopies,
                opt => opt.MapFrom(src => src.NumberOfCopies)
            )
            .ForMember(
                dest => dest.LoanedQuantity,
                opt => opt.MapFrom(src => src.LoanedQuantity)
            )
            .ForMember(
                dest => dest.Genre,
                opt => opt.MapFrom(src => src.Genre)
            )
            .ForMember(
                dest => dest.Status,
                opt => opt.MapFrom(src => src.Status)
            )
            .ForMember(
                dest => dest.ReleaseYear,
                opt => opt.MapFrom(src => src.ReleaseYear)
            )
            .ForMember(
                dest => dest.Title,
                opt => opt.MapFrom(src => src.Title)
            )
            .ForMember(
                dest => dest.Version,
                opt => opt.MapFrom(src => src.Version)
            )
            .ForMember(dest => dest.Authors, opt => opt.Ignore())
            .ForMember(
                dest => dest.Id,
                opt => opt.MapFrom(src => src.Id)
            ).ReverseMap();

            //.AfterMap((src, dest) =>
            //{
            //    // Manually map the many-to-many relationship
            //    dest.Authors = src.Authors.Select(authorId => new Author { Id = authorId }).ToList();
            //});
            //.AfterMap((src, dest) => dest.Id = GenerateNewId(dest)); ;
        }
    }
}
