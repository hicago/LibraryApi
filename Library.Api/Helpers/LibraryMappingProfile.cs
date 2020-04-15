using AutoMapper;
using Library.Api.Entities;
using Library.Api.Extensions;
using Library.Api.Models;

namespace Library.Api.Helpers
{
    public class LibraryMappingProfile : Profile
    {
        public LibraryMappingProfile()
        {
            CreateMap<Author, AuthorDto>()
                .ForMember(dest => dest.Age, config =>
                    config.MapFrom(src => src.BirthDate.GetCurrentAge()));
            CreateMap<Book, BookDto>();
            CreateMap<AuthorForCreationDto, Author>();
            CreateMap<BookForCreationDto, Book>();
            CreateMap<BookForUpdateDto, Book>();
        }
    }
}
